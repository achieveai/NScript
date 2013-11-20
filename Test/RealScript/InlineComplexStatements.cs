//-----------------------------------------------------------------------
// <copyright file="InlineComplexStatements.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;

    /// <summary>
    /// Definition for InlineComplexStatements
    /// </summary>
    public class InlineComplexStatements
    {
        /// <summary>
        /// backing field for IntProperty
        /// </summary>
        private int intProperty;

        /// <summary>
        /// Backing field StringProperty.
        /// </summary>
        private string strProperty;

        /// <summary>
        /// Backing field for TestReference
        /// </summary>
        private TestReferenceClass testReference;

        /// <summary>
        /// Public int field.
        /// </summary>
        public int PublicIntField;

        /// <summary>
        /// Initializes a new instance of the <see cref="InlineComplexStatements"/> class.
        /// </summary>
        public InlineComplexStatements()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InlineComplexStatements"/> class.
        /// </summary>
        /// <param name="foo">The foo.</param>
        public InlineComplexStatements(double foo)
        {
        }

        /// <summary>
        /// Gets or sets the int property.
        /// </summary>
        /// <value>The int property.</value>
        public int IntProperty
        {
            get { return this.intProperty; }
            set { this.intProperty = value; }
        }

        /// <summary>
        /// Gets or sets the string property.
        /// </summary>
        /// <value>The string property.</value>
        public string StringProperty
        {
            get { return this.strProperty; }
            set { this.strProperty = value; }
        }

        /// <summary>
        /// Gets or sets the test reference.
        /// </summary>
        /// <value>The test reference.</value>
        public TestReferenceClass TestReference
        {
            get { return this.testReference; }
            set { this.testReference = value; }
        }

        /// <summary>
        /// Returns the inline object array.
        /// </summary>
        /// <returns></returns>
        public InlineComplexStatements[] ReturnInlineObjectArray()
        {
            return new InlineComplexStatements[]
               {
                   new InlineComplexStatements(),
                   null,
                   new InlineComplexStatements(10.0), 
                   new InlineComplexStatements(15.0), 
               };
        }

        public InlineComplexStatements[] ReturnInlineObjectArrayWithPropInit()
        {
            return new InlineComplexStatements[]
               {
                   new InlineComplexStatements(){IntProperty = 10},
                   null,
                   new InlineComplexStatements() { StringProperty = "test"}, 
                   new InlineComplexStatements(10.1) { TestReference = null}, 
               };
        }

        public void Take2Arrays(string foo, string[] strA, InlineComplexStatements[] iCS)
        {
        }

        public void TestInlineSingleElementArray(
            string foo,
            InlineComplexStatements v)
        {
            this.Take2Arrays(
                foo,
                new string[] { foo },
                new InlineComplexStatements[] { v });
        }

        /// <summary>
        /// Processes the items.
        /// </summary>
        /// <param name="foo">The foo.</param>
        /// <param name="objs">The objs.</param>
        public void ProcessItems(string foo, params InlineComplexStatements[] objs)
        {
        }

        /// <summary>
        /// Tests the args.
        /// </summary>
        public void TestVarArgs()
        {
            this.ProcessItems(
                "string",
                new InlineComplexStatements(10),
                new InlineComplexStatements(10.2),
                new InlineComplexStatements(1),
                new InlineComplexStatements(15));
        }

        /// <summary>
        /// Creates the element.
        /// </summary>
        /// <returns></returns>
        public InlineComplexStatements ReturnInlineSettersElement()
        {
            return new InlineComplexStatements()
                       {
                           StringProperty = "fooProp",
                           intProperty = 10,
                           TestReference =  null,
                       };
        }

        /// <summary>
        /// Creates the element.
        /// </summary>
        /// <returns></returns>
        public void CallMethodWithInlineSettersElement()
        {
            TmpC.Foo(
                "foo",
                new InlineComplexStatements()
                    {
                        StringProperty = "fooProp",
                        intProperty = 10,
                        TestReference = null,
                    });
        }

        public int[] ReturnInlineConstIntArray()
        {
            return new int[]
            {
                10,
                11,
                12,
                14,
                15
            };
        }
    }
}
