namespace NScript.CLR.Test.BlockTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
    using NScript.CLR.Decompiler.Blocks;
    using System.Xml.Linq;
    using NUnit.Framework;

    internal class BlockInfo
    {
        public static BlockInfo InstructionBlockInfo = new BlockInfo { BlockType = typeof(InstructionBlock), Children = new BlockInfo[] { } };

        public Type BlockType
        {
            get;
            set;
        }

        public IList<BlockInfo> Children
        {
            get;
            set;
        }

        public string Id
        {
            get;
            set;
        }

        public string Label
        {
            get;
            set;
        }

        public static void CheckUsingTemplate(string templateFileName, RootBlock block)
        {
            // Gallio.Framework.DiagnosticLog.WriteLine(BlockInfo.PrintInfo(block));
            BlockInfo blockInfo = BlockInfo.ReadFromResourceFile(templateFileName);
            blockInfo.CheckSame(block);
        }

        public static string PrintInfo(RootBlock rootBlock)
        {
            StringBuilder strBuilder = new StringBuilder();
            int id = 1;

            strBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            BlockInfo.PrintInfo(rootBlock, strBuilder, "\n", ref id);

            return strBuilder.ToString();
        }

        public void CheckSame(Block block)
        {
            if (this.BlockType != null)
            {
                Assert.IsInstanceOf(
                    this.BlockType,
                    block,
                    "testId: {0}, actualId: {1}",
                    this.Id, block.Id);
            }

            if (this.Children != null)
            {
                Assert.AreEqual(
                    this.Children.Count,
                    block.Children.Count,
                    "ChildrenCount, testId: {0}, actualId: {1}",
                    this.Id, block.Id);

                for (int iBlock = 0; iBlock < Math.Min(block.Children.Count, this.Children.Count); iBlock++)
                {
                    this.Children[iBlock].CheckSame(block.Children[iBlock]);
                }
            }
        }

        public static BlockInfo ReadFromResourceFile(string resourceFileName)
        {
            using(var stream = typeof (BlockInfo).Assembly.GetManifestResourceStream(resourceFileName))
            {
                return BlockInfo.ReadFromXmlStream(stream);
            }
        }

        public static BlockInfo ReadFromXmlStream(Stream stream)
        {
            XDocument xDocument = XDocument.Load(stream);

            return BlockInfo.ReadFromXmlElement(xDocument.Element("BlockInfo"));
        }

        private static BlockInfo ReadFromXmlElement(XElement element)
        {
            var returnValue = new BlockInfo();

            var attribute = element.FirstAttribute;
            while(attribute != null)
            {
                switch (attribute.Name.LocalName)
                {
                    case "type":
                        switch (attribute.Value)
                        {
                            case "RootBlock":
                                returnValue.BlockType = typeof(RootBlock);
                                break;
                            case "BasicBlock":
                                returnValue.BlockType = typeof(BasicBlock);
                                break;
                            case "ConditionalBlock":
                                returnValue.BlockType = typeof(ConditionalBlock);
                                break;
                            case "ConditionalStatement":
                                returnValue.BlockType = typeof (ConditionalStatement);
                                break;
                            case "InstructionBlock":
                                returnValue.BlockType = typeof(InstructionBlock);
                                break;
                            case "StackedBlock":
                                returnValue.BlockType = typeof (StackedBlock);
                                break;
                            case "SwitchBlock":
                                returnValue.BlockType = typeof (SwitchBlock);
                                break;
                            case "PostfixBlock":
                                returnValue.BlockType = typeof (PostfixBlock);
                                break;
                            case "Block":
                                returnValue.BlockType = typeof (Block);
                                break;
                            case "WhileBlock":
                                returnValue.BlockType = typeof (WhileBlock);
                                break;
                            case "DoWhileBlock":
                                returnValue.BlockType = typeof (DoWhileBlock);
                                break;
                            case "ForBlock":
                                returnValue.BlockType = typeof (ForBlock);
                                break;
                            case "IfElseBlock":
                                returnValue.BlockType = typeof (IfElseBlock);
                                break;
                            case "JumpBlock":
                                returnValue.BlockType = typeof (JumpBlock);
                                break;
                            case "SerialBlock":
                                returnValue.BlockType = typeof (SerialBlock);
                                break;
                            case "InlineObjectArrayCreateBlock":
                                returnValue.BlockType = typeof (InlineObjectArrayCreateBlock);
                                break;
                            case "InlinePropertyInitializerBlock":
                                returnValue.BlockType = typeof (InlinePropertyInitializerBlock);
                                break;
                            case "InlineAssignmentBlock":
                                returnValue.BlockType = typeof (InlineAssignmentBlock);
                                break;
                            case "NullConditionalBlock":
                                returnValue.BlockType = typeof (NullConditionalBlock);
                                break;
                            case "OpAssignmentBlock":
                                returnValue.BlockType = typeof (OpAssignmentBlock);
                                break;
                            case "InlineNumberArrayCreateBlock":
                                returnValue.BlockType = typeof(InlineNumberArrayCreateBlock);
                                break;
                            default:
                                throw new InvalidDataException();
                        }
                        break;
                    case "id":
                        returnValue.Id = attribute.Value;
                        break;
                    case "label":
                        returnValue.Label = attribute.Value;
                        break;
                }

                attribute = attribute.NextAttribute;
            }

            foreach (var childElement in element.Elements("BlockInfo"))
            {
                if (returnValue.Children == null)
                {
                    returnValue.Children = new List<BlockInfo>();
                }

                returnValue.Children.Add(BlockInfo.ReadFromXmlElement(childElement));
            }

            return returnValue;
        }

        private static void PrintInfo(
            Block block,
            StringBuilder strBuilder,
            string indent,
            ref int lastid)
        {
            strBuilder.AppendFormat("{0}<BlockInfo id=\"{1}\" type=\"", indent, lastid++);

            if (block.GetType() == typeof(InstructionBlock))
            {
                strBuilder.Append("InstructionBlock\"");

                string str = string.Format(
                    " label=\"{0}\" opcode=\"{1}\" argument=\"{2}\"",
                    ((InstructionBlock)block).Instruction.Label,
                    ((InstructionBlock)block).Instruction.OpCode,
                    ((InstructionBlock)block).Instruction.OpCodeArgument);

                str = str.Replace("<", "&lt;").Replace(">", "&gt;");

                strBuilder.Append(str);
            }
            else
            {
                strBuilder.AppendFormat("{0}\"", block.GetType().Name);
            }

            if (block.Children.Count > 0)
            {
                strBuilder.Append(">");

                foreach (Block child in block.Children)
                {
                    BlockInfo.PrintInfo(child, strBuilder, indent + "  ", ref lastid);
                }

                strBuilder.AppendFormat("{0}</BlockInfo>", indent);
            }
            else
            {
                strBuilder.Append("/>");
            }
        }
    }
}
