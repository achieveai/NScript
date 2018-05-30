namespace NScript.CLR.Test
{
    using System;
    using System.Xml;
    using NScript.CLR.AST;
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
            bool isDebug)
        {
            MethodDefinition methodDefinition = TestAssemblyLoader.GetMethodDefinition(
                className,
                methodName,
                isDebug);

            return TestAssemblyLoader.DllBuilder.GetTopLevelBlock(methodDefinition);
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
                null
            };
        }

        public static TopLevelBlock[] GetMcsAst(
            string className,
            string methodName)
        {
            return new TopLevelBlock[]
            {
                TestHelpers.GetAST(className, methodName, true),
                null
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
            string xmlString = TestHelpers.ReadResourceFile(manifestFileName).Trim();

            for (int nodeIndex = 0; nodeIndex < nodes.Length; nodeIndex++)
            {
                string str = new ClrAstToStringVisitor().Visit(nodes[nodeIndex], 0).Trim();

                if (string.IsNullOrWhiteSpace(xmlString))
                {
                    Debug.Write(str);
                }

                try
                {
                    Assert.AreEqual(
                        xmlString,
                        str);
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
                /*
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
                */
            }
        }

        public static void Test(
            string manifestFileName,
            string className,
            string methodName,
            bool isDebug,
            bool isMcs)
        {
            var rootBlock = TestHelpers.GetAST(
                className,
                methodName,
                isDebug).RootBlock;
            try
            {
                TestHelpers.Test(
                    manifestFileName,
                    TestHelpers.GetAST(className, methodName, isDebug).RootBlock);
            }
            catch
            {
                /*
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
                */

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