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
    using JsCsc.Lib.Serialization;
    using System.Linq;

    /// <summary>
    /// Definition for Driver
    /// </summary>
    public class DriverWrapper
    {
        private MemoryStream bstStream = new MemoryStream();
        private MemoryStream resourceInfoStream = new MemoryStream();
        private MonoAstVisitor visitor = new MonoAstVisitor();
        private List<Method> methods = new List<Method>();
        private AssemblyDefinition assembly;
        private Dictionary<object, ToplevelBlock> methodBlocks = new Dictionary<object, ToplevelBlock>();
        private Dictionary<TypeContainer, List<PropertyBase>> typeToPropertyMapping = new Dictionary<TypeContainer, List<PropertyBase>>();

        public void Compile(string[] args)
        {
            var stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();

            var errorStream = Console.Error;
            var messageStream = Console.Out;
            var printer = new StreamReportPrinter(errorStream);
            var langverRegex = new System.Text.RegularExpressions.Regex("[/-]langversion:[^\\s]*");
            CommandLineParser cmd = new CommandLineParser(errorStream, messageStream);

            var fixedArgs = args
                .Where(arg => !arg.StartsWith("/langversion") && !arg.StartsWith("-langverion"))
                .ToArray();

            foreach (var arg in fixedArgs)
            {
                if (arg.StartsWith("@"))
                {
                    var argLines = File
                        .ReadAllLines(arg.Substring(1))
                        .Where(line =>
                            !line.StartsWith("/langversion")
                            && !line.StartsWith("-langversion"))
                        .Select(line => langverRegex.Replace(line, ""))
                        .ToList();

                    Console.WriteLine("RspOutput: \r\n{0}", string.Join("\r\n", argLines));

                    File.WriteAllLines(
                        arg.Substring(1),
                        argLines);
                }
            }

            var setting = cmd.ParseArguments(fixedArgs);
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
                    this.bstStream,
                    "$$BstInfo$$",
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

            stopWatch.Stop();
            System.Console.WriteLine("TimeTaken: {0}ms", stopWatch.ElapsedMilliseconds);
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
            // var stopWatch = System.Diagnostics.Stopwatch.StartNew();
            // this.EmitAstUsingJson(assemblyDefinition);
            // stopWatch.Stop();
            // double jsonTime = stopWatch.Elapsed.TotalSeconds;
            // stopWatch.Reset();
            // stopWatch.Start();
            this.EmitAstUsingBond(assemblyDefinition);
            // stopWatch.Stop();
            // double bondTime = stopWatch.Elapsed.TotalSeconds;

            // Console.WriteLine("Json: {0}, Bond: {1}", jsonTime, bondTime);
        }

        private void EmitAstUsingBond(AssemblyDefinition assemblyDefinition)
        {
            this.assembly = assemblyDefinition;

            var fullAst = new FullAst() { Methods = new List<MethodBody>() };
            var toSer = new AstToSerialization();
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

                            fullAst.Methods.Add(
                                toSer.SerializeMethodBody(
                                    property.Get,
                                    this.methodBlocks[property.Get]));
                        }
                        else if (property.Get != null && !property.IsCompilerGenerated)
                        {
                            fullAst.Methods.Add(
                                toSer.SerializeMethodBody(
                                    property.Get,
                                    null));
                        }

                        if (property.Set != null && !property.Set.IsCompilerGenerated)
                        {
                            if (!this.methodBlocks.ContainsKey(property.Set))
                            {
                                continue;
                            }

                            fullAst.Methods.Add(
                                toSer.SerializeMethodBody(
                                    property.Set,
                                    this.methodBlocks[property.Set]));
                        }
                        else if (property.Set != null && !property.IsCompilerGenerated)
                        {
                            fullAst.Methods.Add(
                                toSer.SerializeMethodBody(
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

                            fullAst.Methods.Add(
                                toSer.SerializeMethodBody(
                                    indexer.Get,
                                    this.methodBlocks[indexer.Get]));
                        }
                        else if (indexer.Get != null && !indexer.IsCompilerGenerated)
                        {
                            fullAst.Methods.Add(
                                toSer.SerializeMethodBody(
                                    indexer.Get,
                                    null));
                        }

                        if (indexer.Set != null && !indexer.Set.IsCompilerGenerated)
                        {
                            if (!this.methodBlocks.ContainsKey(indexer.Set))
                            {
                                continue;
                            }

                            fullAst.Methods.Add(
                                toSer.SerializeMethodBody(
                                    indexer.Set,
                                    this.methodBlocks[indexer.Set]));
                        }
                        else if (indexer.Set != null && !indexer.IsCompilerGenerated)
                        {
                            fullAst.Methods.Add(
                                toSer.SerializeMethodBody(
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

                            fullAst.Methods.Add(
                                toSer.SerializeMethodBody(
                                    evt.Add,
                                    this.methodBlocks[evt.Add]));
                        }
                        else if (evt.IsCompilerGenerated && evt.Add != null)
                        {
                            fullAst.Methods.Add(
                                toSer.SerializeMethodBody(
                                    evt.Add,
                                    null));
                        }

                        if (evt.Remove != null && !evt.Remove.IsCompilerGenerated)
                        {
                            if (!this.methodBlocks.ContainsKey(evt.Remove))
                            {
                                continue;
                            }

                            fullAst.Methods.Add(
                                toSer.SerializeMethodBody(
                                    evt.Remove,
                                    this.methodBlocks[evt.Remove]));
                        }
                        else if (evt.IsCompilerGenerated && evt.Remove != null)
                        {
                            fullAst.Methods.Add(
                                toSer.SerializeMethodBody(
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

                        fullAst.Methods.Add(
                            toSer.SerializeMethodBody(
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
                            fullAst.Methods.Add(
                                toSer.SerializeMethodBody(
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

            fullAst.TypeInfo = toSer.TypeSerializationInfo;
            Serializer.Serialize(this.bstStream, fullAst, Serializer.SerializationKind.NetSerializer);

            this.bstStream.Position = 0;
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