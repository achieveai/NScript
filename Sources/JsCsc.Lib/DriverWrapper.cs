//-----------------------------------------------------------------------
// <copyright file="Driver.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JsCsc.Lib
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Text;
    using Mono.CSharp;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Definition for Driver
    /// </summary>
    public class DriverWrapper
    {
        private MemoryStream astStream = new MemoryStream();
        private MonoAstVisitor visitor = new MonoAstVisitor();
        private List<Method> methods = new List<Method>();
        private AssemblyDefinition assembly;
        private Dictionary<object, ToplevelBlock> methodBlocks = new Dictionary<object, ToplevelBlock>();
        private Dictionary<TypeContainer, List<PropertyBase>> typeToPropertyMapping = new Dictionary<TypeContainer, List<PropertyBase>>();

        public void Compile(string[] args)
        {
            var errorStream = Console.Error;
            var messageStream = Console.Out;
            var printer = new StreamReportPrinter(errorStream);
            CommandLineParser cmd = new CommandLineParser(errorStream, messageStream);

            List<string> fixedArgs = new List<string>(args);
            var setting = cmd.ParseArguments(args);
            setting.StdLibRuntimeVersion = RuntimeVersion.v2;
            if (setting == null)
                return;

            if (setting.Resources == null)
            {
                setting.Resources = new List<AssemblyResource>();
            }

            setting.Resources.Add(
                new AssemblyResource(
                    this.astStream,
                    "$$AstInfo$$",
                    true)
                    {
                        IsEmbeded = true
                    });

            var context = new CompilerContext(setting, printer);

            CompilerCallableEntryPoint.InvokeCompiler(
                context,
                this.OnResolveComplete,
                this.OnEmitComplete);

            errorStream.Flush();
            errorStream.Close();

            if (setting.GenerateDebugInfo)
            {
                string pdbFileName = Path.Combine(
                    Path.GetDirectoryName(setting.OutputFile),
                    Path.GetFileNameWithoutExtension(setting.OutputFile) + ".pdb");

                try
                {
                    MdbToPdb.Convert(setting.OutputFile, pdbFileName);
                }
                catch { }
            }
        }

        private void OnResolveComplete(AssemblyDefinition assemblyDefinition)
        {
            foreach (var container in GetNestedContainers(assemblyDefinition.Module.Containers))
            {
                TypeDefinition typeDef = container as TypeDefinition;
                if (typeDef == null)
                {
                    continue;
                }

                if (container.Kind == MemberKind.Interface) continue;

                foreach (var member in typeDef.Members)
                {
                    Property property = member as Property;
                    if (property != null)
                    {
                        if (property.Get != null && property.Get.Block != null)
                        {
                            this.methodBlocks.Add(property.Get, property.Get.Block);
                        }

                        if (property.Set != null && property.Set.Block != null)
                        {
                            this.methodBlocks.Add(property.Set, property.Set.Block);
                        }

                        continue;
                    }

                    Event evt = member as Event;
                    if (evt != null)
                    {
                        if (evt.Add != null)
                        {
                            this.methodBlocks.Add(evt.Add, evt.Add.Block);
                        }

                        if (evt.Remove != null)
                        {
                            this.methodBlocks.Add(evt.Remove, evt.Remove.Block);
                        }

                        continue;
                    }

                    Method method = member as Method;
                    if (method != null)
                    {
                        if (method.Block != null)
                        {
                            this.methodBlocks.Add(method, method.Block);
                        }

                        continue;
                    }

                    Constructor constructor = member as Constructor;
                    if (constructor != null)
                    {
                        if (constructor.Block != null)
                        {
                            this.methodBlocks.Add(constructor, constructor.Block);
                        }
                    }
                }
            }
        }

        private void OnEmitComplete(AssemblyDefinition assemblyDefinition)
        {
            this.assembly = assemblyDefinition;

            JArray jarray = new JArray();
            AstToJObject toJObject = new AstToJObject();

            foreach (var container in GetNestedContainers(assemblyDefinition.Module.Containers))
            {
                TypeDefinition typeDef = container as TypeDefinition;
                if (typeDef == null)
                {
                    continue;
                }

                if (container.Kind == MemberKind.Interface) continue;

                foreach (var member in typeDef.Members)
                {
                    Property property = member as Property;
                    if (property != null)
                    {
                        if (property.Get != null && !property.Get.IsCompilerGenerated)
                        {
                            if (!this.methodBlocks.ContainsKey(property.Get))
                            {
                                continue;
                            }

                            jarray.Add(
                                toJObject.SerializeMethodBody(
                                    property.Get,
                                    this.methodBlocks[property.Get]));
                        }
                        else if (property.Get != null && !property.IsCompilerGenerated)
                        {
                            jarray.Add(
                                toJObject.SerializeMethodBody(
                                    property.Get,
                                    null));
                        }

                        if (property.Set != null && !property.Set.IsCompilerGenerated)
                        {
                            if (!this.methodBlocks.ContainsKey(property.Set))
                            {
                                continue;
                            }

                            jarray.Add(
                                toJObject.SerializeMethodBody(
                                    property.Set,
                                    this.methodBlocks[property.Set]));
                        }
                        else if (property.Set != null && !property.IsCompilerGenerated)
                        {
                            jarray.Add(
                                toJObject.SerializeMethodBody(
                                    property.Set,
                                    null));
                        }

                        continue;
                    }

                    Event evt = member as Event;
                    if (evt != null)
                    {
                        if (evt.Add != null && !evt.Add.IsCompilerGenerated)
                        {
                            if (!this.methodBlocks.ContainsKey(evt.Add))
                            {
                                continue;
                            }

                            jarray.Add(
                                toJObject.SerializeMethodBody(
                                    evt.Add,
                                    this.methodBlocks[evt.Add]));
                        }
                        else if (evt.IsCompilerGenerated && evt.Add != null)
                        {
                            jarray.Add(
                                toJObject.SerializeMethodBody(
                                    evt.Add,
                                    null));
                        }

                        if (evt.Remove != null && !evt.Remove.IsCompilerGenerated)
                        {
                            if (!this.methodBlocks.ContainsKey(evt.Remove))
                            {
                                continue;
                            }

                            jarray.Add(
                                toJObject.SerializeMethodBody(
                                    evt.Remove,
                                    this.methodBlocks[evt.Remove]));
                        }
                        else if (evt.IsCompilerGenerated && evt.Remove != null)
                        {
                            jarray.Add(
                                toJObject.SerializeMethodBody(
                                    evt.Remove,
                                    null));
                        }

                        continue;
                    }

                    Method method = member as Method;
                    if (method != null)
                    {
                        if (method.IsCompilerGenerated
                            || !this.methodBlocks.ContainsKey(method))
                        {
                            continue;
                        }

                        jarray.Add(
                            toJObject.SerializeMethodBody(
                                (Method)method,
                                this.methodBlocks[method]));

                        continue;
                    }

                    Constructor constructor = member as Constructor;
                    if (constructor != null)
                    {
                        if (constructor.IsCompilerGenerated)
                        {
                            continue;
                        }

                        if (this.methodBlocks.ContainsKey(constructor)
                            || (constructor.IsStatic && typeDef.InitializedStaticFields.Count > 0)
                            || (!constructor.IsStatic && typeDef.InitializedFields.Count > 0))
                        {
                            jarray.Add(
                                toJObject.SerializeMethodBody(
                                    constructor,
                                    constructor.IsStatic
                                        ? typeDef.InitializedStaticFields
                                        : typeDef.InitializedFields,
                                    this.methodBlocks.ContainsKey(constructor)
                                        ? this.methodBlocks[constructor]
                                        : null));
                        }
                    }
                }
            }

            using (var gzipStream = new GZipStream(this.astStream, CompressionMode.Compress, true))
            {
                using (var stream = new StreamWriter(gzipStream, Encoding.UTF8))
                using (var jsWriter = new JsonTextWriter(stream))
                {
                    // jsWriter.Indentation = 2;
                    // jsWriter.IndentChar = ' ';
                    jsWriter.Formatting = Formatting.None;
                    jsWriter.CloseOutput = false;

                    jarray.WriteTo(jsWriter);
                    jsWriter.Close();
                }
            }

            this.astStream.Position = 0;

            /*
            using (var stream = new FileStream(this.astFileName, FileMode.Truncate))
            using (var jsWriter = new BsonWriter(stream))
            {
                // jsWriter.Indentation = 2;
                // jsWriter.IndentChar = ' ';
                // jsWriter.Formatting = Formatting.Indented;
                jarray.WriteTo(jsWriter);
                jsWriter.Close();
            }
            */
        }

        private static IEnumerable<TypeContainer> GetNestedContainers(IEnumerable<TypeContainer> containers)
        {
            if (containers == null)
            {
                yield break;
            }

            foreach (var container in containers)
            {
                yield return container;

                foreach (var nestedContainer in GetNestedContainers(container.Containers))
                {
                    yield return nestedContainer;
                }
            }
        }
    }
}