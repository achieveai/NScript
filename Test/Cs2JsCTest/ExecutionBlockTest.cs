using Cs2JsC.Lib.ILParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Cs2JsC.Lib.MetaData;

namespace Cs2JsCTest
{
    
    
    /// <summary>
    ///This is a test class for ExecutionBlockTest and is intended
    ///to contain all ExecutionBlockTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ExecutionBlockTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        public MethodSignature MethodSignature
        {
            get;
            private set;
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        /// Test instruction count on simple int return.
        ///</summary>
        [TestMethod()]
        public void TestSimpleMethodReturnInstructionCount()
        {
            var exectionBlock = TestILFunctions.GetExecutionBlock(TestILFunctions.SimpleIntReturnFunction);

            Assert.AreEqual(exectionBlock.Instructions.Count, 3);
        }

        /// <summary>
        /// Test block count on simple int return.
        ///</summary>
        [TestMethod()]
        public void TestSimpleMethodReturnBlockCount()
        {
            var exectionBlock = TestILFunctions.GetExecutionBlock(TestILFunctions.SimpleIntReturnFunction);

            Assert.AreEqual(exectionBlock.Decompiler.RootBlock.Children.Count, 1);
        }
    }
}
