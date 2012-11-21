using System;
using NScript.Lib.AsmDeasm.IlBlocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Xml.Linq;
using System.Collections.Generic;

namespace NScriptTest
{
    class BlockInfo
    {
        public static BlockInfo InstructionBlockInfo = new BlockInfo { BlockType = typeof(InstructionBlock), Children = new BlockInfo[] { } };

        public Type BlockType { get; set; }
        public IList<BlockInfo> Children { get; set; }
        public string Id { get; set; }
        public string Label { get; set; }

        public void CheckSame(Block block)
        {
            if (this.BlockType != null)
            {
                Assert.IsInstanceOfType(
                    block,
                    this.BlockType,
                    "testId: {0}, actualId: {1}",
                    this.Id, block.Id);
            }

            if (this.Label != null && block is InstructionBlock)
            {
                Assert.AreEqual(
                    this.Label,
                    ((InstructionBlock)block).Instruction.Label);
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
    }
}
