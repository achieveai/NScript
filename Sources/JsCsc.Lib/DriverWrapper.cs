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
        private MemoryStream resourceInfoStream = new MemoryStream();
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
            else
            {
                JObject jObject = new JObject();
                foreach (var res in setting.Resources)
                {
                    var fileName = Path.GetFullPath(res.FileName);
                    jObject.Add(res.Name, fileName);
                }

                TextWriter txtWriter = new StreamWriter(this.resourceInfoStream);
                JsonWriter writer = new JsonTextWriter(txtWriter);
                jObject.WriteTo(writer);
                writer.Flush();
                txtWriter.Flush();
                this.resourceInfoStream.Position = 0;
            }

            setting.Resources.Add(
                new AssemblyResource(
                    this.resourceInfoStream,
                    "$$ResInfo$$",
                    true)
                    {
                        IsEmbeded = true
                    });

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
            HashSet<TypeDefinition> typesProcessed = new HashSet<TypeDefinition>();
            foreach (var container in GetNestedContainers(assemblyDefinition.Module.Containers))
            {
                TypeDefinition typeDef = container.PartialContainer as TypeDefinition;
                if (typeDef == null || typesProcessed.Contains(typeDef))
                { continue; }

                typesProcessed.Add(typeDef);

                if (container.Kind == MemberKind.Interface) continue;

                foreach (var member in typeDef.Members)
                {
                    Property property = member as Property;
                    if (property != null)
                    {
                        if (property.Get != null
                            && property.Get.Block != null)
                            // && (!typeDef.IsPartial || !this.methodBlocks.ContainsKey(property.Get)))
                        {
                            this.methodBlocks.Add(property.Get, property.Get.Block);
                        }

                        if (property.Set != null && property.Set.Block != null)
                            // && (!typeDef.IsPartial || !this.methodBlocks.ContainsKey(property.Set)))
                        {
                            this.methodBlocks.Add(property.Set, property.Set.Block);
                        }

                        continue;
                    }

                    Event evt = member as Event;
                    if (evt != null)
                    {
                        if (evt.Add != null)
                            // && (!typeDef.IsPartial || !this.methodBlocks.ContainsKey(evt.Add)))
                        {
                            this.methodBlocks.Add(evt.Add, evt.Add.Block);
                        }

                        if (evt.Remove != null)
                            // && (!typeDef.IsPartial || !this.methodBlocks.ContainsKey(evt.Remove)))
                        {
                            this.methodBlocks.Add(evt.Remove, evt.Remove.Block);
                        }

                        continue;
                    }

                    Indexer indexer = member as Indexer;
                    if (indexer != null)
                    {
                        if (indexer.Get != null)
                            // && (!typeDef.IsPartial || !this.methodBlocks.ContainsKey(indexer.Get)))
                        {
                            this.methodBlocks.Add(indexer.Get, indexer.Get.Block);
                        }

                        if (indexer.Set != null)
                            // && (!typeDef.IsPartial || !this.methodBlocks.ContainsKey(indexer.Set)))
                        {
                            this.methodBlocks.Add(indexer.Set, indexer.Set.Block);
                        }

                        continue;
                    }

                    Method method = member as Method;
                    if (method != null)
                    {
                        if (method.Block != null)
                            // && (!typeDef.IsPartial || !this.methodBlocks.ContainsKey(method)))
                        {
                            this.methodBlocks.Add(method, method.Block);
                        }

                        continue;
                    }

                    Constructor constructor = member as Constructor;
                    if (constructor != null)
                    {
                        if (constructor.Block != null)
                            // && (!typeDef.IsPartial || !this.methodBlocks.ContainsKey(constructor)))
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
            HashSet<TypeDefinition> typesProcessed = new HashSet<TypeDefinition>();

            foreach (var container in GetNestedContainers(assemblyDefinition.Module.Containers))
            {
                TypeDefinition typeDef = container.PartialContainer as TypeDefinition;

                // Partials will appear multiple times, so skip if already processed
                if (typeDef == null || typesProcessed.Contains(typeDef))
                { continue; }

                typesProcessed.Add(typeDef);
                if (container.Kind == MemberKind.Interface) continue;

                foreach (var member in typeDef.Members)
                {
                    Property property = member as Property;
                    if (property != null)
                    {
                        if (property.Get != null && !property.Get.IsCompilerGenerated)
                        {
                            if (!this.methodBlocks.ContainsKey(property.Get))
                            { continue; }

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

                    Indexer indexer = member as Indexer;
                    if (indexer != null)
                    {
                        if (indexer.Get != null && !indexer.Get.IsCompilerGenerated)
                        {
                            if (!this.methodBlocks.ContainsKey(indexer.Get))
                            {
                                continue;
                            }

                            jarray.Add(
                                toJObject.SerializeMethodBody(
                                    indexer.Get,
                                    this.methodBlocks[indexer.Get]));
                        }
                        else if (indexer.Get != null && !indexer.IsCompilerGenerated)
                        {
                            jarray.Add(
                                toJObject.SerializeMethodBody(
                                    indexer.Get,
                                    null));
                        }

                        if (indexer.Set != null && !indexer.Set.IsCompilerGenerated)
                        {
                            if (!this.methodBlocks.ContainsKey(indexer.Set))
                            {
                                continue;
                            }

                            jarray.Add(
                                toJObject.SerializeMethodBody(
                                    indexer.Set,
                                    this.methodBlocks[indexer.Set]));
                        }
                        else if (indexer.Set != null && !indexer.IsCompilerGenerated)
                        {
                            jarray.Add(
                                toJObject.SerializeMethodBody(
                                    indexer.Set,
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
                            || (constructor.IsStatic && typeDef.InitializedStaticFields != null && typeDef.InitializedStaticFields.Count > 0)
                            || (!constructor.IsStatic && typeDef.InitializedFields != null && typeDef.InitializedFields.Count > 0))
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

            // using (var gzipStream = new GZipStream(this.astStream, CompressionMode.Compress, true))
            {
                using (var stream = new StreamWriter(this.astStream, Encoding.UTF8, 4069, true))
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