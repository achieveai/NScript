namespace NScript.CLR.Test
{
    using System;
    using System.Xml;
    using NScript.CLR.AST;
    using NScript.CLR.Decompiler;
    using NScript.CLR.Decompiler.Blocks;
    using NScript.Utils;
    using NUnit.Framework;
    using Mono.Cecil;
    using System.Text;
    using System.Collections.Generic;
    using System.IO;
    using System.Diagnostics;
    using XmlUnit.Xunit;

    internal static class TestHelpers
    {
        public static void RunBlockInfoTest(
            string className,
            string methodName,
            bool isDebug,
            string templateName,
            params Action<RootBlock>[] processors)
        {
            MethodDefinition methodDefinition = TestAssemblyLoader.GetMethodDefinition(
                className,
                methodName,
                isDebug);

            MethodExecutionBlock executionBlock = new MethodExecutionBlock(
                TestAssemblyLoader.Context,
                methodDefinition);

            RootBlock rootBlock = RootBlock.Create(
                executionBlock.Instructions,
                executionBlock.LabelInstructionMap,
                executionBlock.Method,
                executionBlock.Context);

            rootBlock.ProccessThroughPipeline(processors);

            BlockTests.BlockInfo.CheckUsingTemplate(
                templateName,
                rootBlock);
        }

        public static void TestILBlocks(
            string testVerificationDataFile,
            string className,
            string methodName,
            bool isDebug,
            params Action<RootBlock>[] processors)
        {
            MethodDefinition methodDefinition = TestAssemblyLoader.GetMethodDefinition(
                className,
                methodName,
                isDebug);

            MethodExecutionBlock executionBlock = new MethodExecutionBlock(
                TestAssemblyLoader.Context,
                methodDefinition);

            RootBlock rootBlock;
            if (processors.Length > 0)
            {
                rootBlock = RootBlock.Create(
                    executionBlock.Instructions,
                    executionBlock.LabelInstructionMap,
                    methodDefinition,
                    TestAssemblyLoader.Context);
                rootBlock.ProccessThroughPipeline(processors);
            }
            else
            {
                rootBlock = executionBlock.GetRootBlock();
            }

            BlockTests.BlockInfo.CheckUsingTemplate(testVerificationDataFile, rootBlock);
        }

        /// <summary>
        /// Tests the print IL blocks.
        /// </summary>
        /// <param name="testVerificationDataFile">The test verification data file.</param>
        /// <param name="className">Name of the class.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="isDebug">if set to <c>true</c> [is debug].</param>
        /// <param name="processors">The processors.</param>
        public static void TestPrintILBlocks(
            string testVerificationDataFile,
            string className,
            string methodName,
            bool isDebug,
            params Action<RootBlock>[] processors)
        {
            MethodDefinition methodDefinition = TestAssemblyLoader.GetMethodDefinition(
                className,
                methodName,
                isDebug);

            MethodExecutionBlock executionBlock = new MethodExecutionBlock(
                TestAssemblyLoader.Context,
                methodDefinition);

            RootBlock rootBlock;
            if (processors.Length > 0)
            {
                rootBlock = RootBlock.Create(
                    executionBlock.Instructions,
                    executionBlock.LabelInstructionMap,
                    methodDefinition,
                    TestAssemblyLoader.Context);
                rootBlock.ProccessThroughPipeline(processors);
            }
            else
            {
                rootBlock = executionBlock.GetRootBlock();
            }

            Debug.Write(TestHelpers.Serialize(rootBlock));
        }

        /// <summary>
        /// Gets the AST.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="isDebug">if set to <c>true</c> [is debug].</param>
        /// <returns></returns>
        public static TopLevelBlock GetAST(
            string className,
            string methodName,
            bool isDebug,
            bool isMcs = false)
        {
            MethodDefinition methodDefinition = TestAssemblyLoader.GetMethodDefinition(
                className,
                methodName,
                isDebug);

            if (isMcs)
            {
                return TestAssemblyLoader.DllBuilder.GetTopLevelBlock(methodDefinition);
            }

            MethodExecutionBlock executionBlock = new MethodExecutionBlock(
                TestAssemblyLoader.Context,
                methodDefinition);

            return executionBlock.ToAST();
        }

        /// <summary>
        /// Gets the AST.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        public static TopLevelBlock[] GetAST(
            string className,
            string methodName)
        {
            return new TopLevelBlock[]
            {
                TestHelpers.GetAST(className, methodName, true),
                TestHelpers.GetAST(className, methodName, false)
            };
        }

        public static TopLevelBlock[] GetMcsAst(
            string className,
            string methodName)
        {
            return new TopLevelBlock[]
            {
                TestHelpers.GetAST(className, methodName, true),
                TestHelpers.GetAST(className, methodName, false)
            };
        }

        /// <summary>
        /// Tests the specified manifest file name.
        /// </summary>
        /// <param name="manifestFileName">Name of the manifest file.</param>
        /// <param name="nodes">The nodes.</param>
        public static void Test(
            string manifestFileName,
            params Node[] nodes)
        {
            string xmlString = TestHelpers.ReadResourceFile(manifestFileName);

            for (int nodeIndex = 0; nodeIndex < nodes.Length; nodeIndex++)
            {
                string str = TestHelpers.Serialize(
                    nodes[nodeIndex],
                    Path.GetFileNameWithoutExtension(manifestFileName).EndsWith("Mcs")
                        ? 2
                        : 1);

                if (string.IsNullOrWhiteSpace(xmlString))
                {
                    Debug.Write(str);
                }

                try
                {
                    XmlInput xmlInput1 = new XmlInput(xmlString);
                    XmlInput xmlInput2 = new XmlInput(str);
                    DiffConfiguration config = new DiffConfiguration(
                        "testDiff",
                        true,
                        WhitespaceHandling.Significant,
                        true);

                    XmlDiff diff = new XmlDiff(
                        xmlInput1,
                        xmlInput2,
                        config);

                    XmlAssertion.AssertXmlEquals(diff);
                }
                catch
                {
                    Debug.WriteLine("Expected");
                    Debug.WriteLine(xmlString);
                    Debug.WriteLine("*-*-*-*-*-*-*-*-**-*-*-*-*-*-*-*-*--*-*-*-*-*-*-*-*-*-");
                    Debug.WriteLine("Actual");
                    Debug.WriteLine(str);

                    throw;
                }
            }
        }

        public static void Test(
            string manifestFileName,
            string className,
            string methodName,
            bool isDebug,
            bool isMcs)
        {
            var rootBlock = TestHelpers.GetAST(className, methodName, isDebug, isMcs).RootBlock;
            try
            {
                TestHelpers.Test(
                    manifestFileName,
                    TestHelpers.GetAST(className, methodName, isDebug, isMcs).RootBlock);
            }
            catch
            {
                if (isMcs
                    && !Path.GetFileNameWithoutExtension(manifestFileName).EndsWith("Mcs"))
                {
                    string fileName = Path.GetFileNameWithoutExtension(manifestFileName.Substring("NScript.Clr.Test.".Length)).Replace('.', '\\');
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Encoding = System.Text.UTF8Encoding.Default;
                    settings.OmitXmlDeclaration = true;
                    settings.Indent = true;
                    settings.IndentChars = "  ";
                    using (XmlWriter writer = XmlWriter.Create(
                        @"D:\Users\Gautam\Documents\Visual Studio 2010\Projects\NScript_Mono\NScript.CLR.Test\"
                        + fileName + "Mcs.xml",
                        settings))
                    {
                        CustomXmlSerializer serializer = new CustomXmlSerializer(writer);
                        serializer.Version = 2;

                        serializer.AddValue("RootBlock", rootBlock);
                    }
                }

                throw;
            }
        }

        /// <summary>
        /// Serializes the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        public static string Serialize(Node node, int version = 1)
        {
            System.IO.StringWriter stringStream = new System.IO.StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = System.Text.UTF8Encoding.Default;
            settings.OmitXmlDeclaration = true;
            XmlWriter writer = XmlWriter.Create(stringStream, settings);
            CustomXmlSerializer serializer = new CustomXmlSerializer(writer);
            serializer.Version = version;
            serializer.AddValue("RootBlock", node);
            writer.Close();
            return stringStream.ToString();
        }

        /// <summary>
        /// Serializes the specified block.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <returns>string with info about the root block.</returns>
        public static string Serialize(RootBlock block)
        {
            return BlockTests.BlockInfo.PrintInfo(block);
        }

        private static string ReadResourceFile(string fileName)
        {
            using(var stream = typeof (TestHelpers).Assembly.GetManifestResourceStream(fileName))
            {
                if (stream != null)
                {
                    System.IO.TextReader reader = new System.IO.StreamReader(stream);
                    return reader.ReadToEnd().Trim();
                }

                return null;
            }
        }
    }
}