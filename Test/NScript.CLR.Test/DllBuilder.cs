//-----------------------------------------------------------------------
// <copyright file="DllBuilder.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using NScript.CLR.AST;
    using JsCsc.Lib;
    using Mono.Cecil;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using JsCsc.Lib.Serialization;

    /// <summary>
    /// Definition for DllBuilder
    /// </summary>
    public class DllBuilder
    {
        static readonly string[] commonOptions = new string[] {
            "/noconfig",
            "/unsafe+",
            "/nowarn:0824,0169,0649,0626,0444,0414,3001,3002,0660,0661,1701,1702",
            "/nostdlib+",
            "/errorreport:prompt",
            "/warn:4",
            "/filealign:512",
            "/target:library"
        };

        static readonly string[] debugOptions = new string[] {
            "/define:DEBUG;TRACE",
            "/debug:full",
            "/debug+",
            "/optimize-"
        };

        static readonly string[] retailOptions = new string[] {
            "/optimize+"
        };

        HashSet<string> loadedDlls = new HashSet<string>();
        ClrContext context = new ClrContext();
        Dictionary<MethodDefinition, Func<TopLevelBlock>> blockMaps =
            new Dictionary<MethodDefinition, Func<TopLevelBlock>>(new MemberReferenceComparer());

        public DllBuilder()
        {
        }

        public ClrContext ClrContext
        {
            get { return this.context; }
        }

        public TopLevelBlock GetTopLevelBlock(MethodDefinition methodDefinition)
        {
            Func<TopLevelBlock> rv;
            this.blockMaps.TryGetValue(methodDefinition, out rv);
            return rv();
        }

        public void Build(string outFileName, string sourceFilesPath, string[] sourceFiles, string[] references, bool isDebug, string keyFile = null)
        {
            List<string> args = new List<string>();
            args.AddRange(DllBuilder.commonOptions);

            if (isDebug)
            {
                args.AddRange(DllBuilder.debugOptions);
            }
            else
            {
                args.AddRange(DllBuilder.retailOptions);
            }

            args.Add(string.Format("/out:\"{0}\"", outFileName));

            if (references != null)
            {
                foreach (var reference in references)
                {
                    if (!this.loadedDlls.Contains(reference.ToLowerInvariant()))
                    {
                        this.context.LoadAssembly(reference);
                        this.loadedDlls.Add(reference.ToLowerInvariant());
                    }

                    args.Add(string.Format("/r:\"{0}\"", reference));
                }
            }

            foreach (var file in sourceFiles)
            {
                args.Add(string.Format("\"{0}\\{1}\"", sourceFilesPath, file));
            }

            if (keyFile != null)
            {
                args.Add(string.Format("/keyfile:\"{0}\\{1}\"", sourceFilesPath, keyFile));
            }

            string backupDir = Environment.CurrentDirectory;
            try
            {
                Environment.CurrentDirectory = System.IO.Path.GetTempPath();
                JsCsc.Lib.DriverWrapper driverWrapper = new JsCsc.Lib.DriverWrapper();
                string tempFileName = Path.GetTempFileName();
                File.WriteAllLines(tempFileName, args, System.Text.Encoding.UTF8);
                driverWrapper.Compile(new string[] { "@" + tempFileName });
                File.Delete(tempFileName);

                string generatedFileName = System.IO.Path.Combine(Environment.CurrentDirectory, outFileName);

                this.LoadAst(generatedFileName);
            }
            finally
            {
                Environment.CurrentDirectory = backupDir;
            }
        }

        private void LoadAst(string assemblyFullFileName)
        {
            string assemblyFileName = Path.GetFileName(assemblyFullFileName);
            assemblyFileName = assemblyFileName.ToLowerInvariant();
            if (this.loadedDlls.Contains(assemblyFileName))
            {
                return;
            }

            this.context.LoadAssembly(assemblyFullFileName, false);
            this.loadedDlls.Add(assemblyFileName);

            ModuleDefinition module;
            if (!context.TryGetModuleDefinition(assemblyFileName, out module))
            {
                throw new InvalidOperationException();
            }

            JArray jsonAstArray = null;
            FullAst fullAst = null;
            foreach (var resource in module.Resources)
            {
                if (resource.Name == "$$AstInfo$$")
                {
                    // Mono.Cecil.EmbeddedResource embededResource = (Mono.Cecil.EmbeddedResource)resource;

                    // using (var stream = embededResource.GetResourceStream())
                    // using (var unzipStream = new GZipStream(stream, CompressionMode.Decompress))
                    // {
                    //     JsonTextReader reader = new JsonTextReader(new StreamReader(unzipStream));
                    //     jsonAstArray = JArray.Load(reader);
                    // }

                    // break;
                }
                else if (resource.Name == "$$BstInfo$$")
                {
                    EmbeddedResource embededResource = (EmbeddedResource)resource;

                    // stopWatch.Restart();
                    using (var stream = embededResource.GetResourceStream())
                    {
                        fullAst = ProtoBuf.Serializer.Deserialize<FullAst>(stream);
                    }
                    // stopWatch.Stop();
                    // bondCost += stopWatch.Elapsed.TotalSeconds;
                }
            }

            MemberReferenceConverter referenceConverter = new MemberReferenceConverter(context);
            var toAst = new BondToAst(fullAst.TypeInfo, context);

            foreach (var methodBody in fullAst.Methods)
            {
                var topLevelBlock = toAst.ParseMethodBody(
                    methodBody);

                this.blockMaps.Add(
                    topLevelBlock.Item1,
                    topLevelBlock.Item2);
            }

            // JObjectToCsAst toAst = new BondToAst(context);

            // for (int iAst = 0; iAst < jsonAstArray.Count; iAst++)
            // {
            //     var topLevelBlock = toAst.ParseMethodBody(
            //         jsonAstArray.Value<JObject>(iAst));

            //     this.blockMaps.Add(
            //         topLevelBlock.Item1,
            //         topLevelBlock.Item2);
            // }
        }
    }
}