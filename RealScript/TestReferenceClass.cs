//-----------------------------------------------------------------------
// <copyright file="TestReferenceClass.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{

    /// <summary>
    /// Definition for TestReferenceClass
    /// </summary>
    public class TestReferenceClass
    {
        public int intField = -1;
        public TestReferenceClass objField = null;

        public TestReferenceClass()
        {
        }

        public TestReferenceClass(int intField)
        {
            this.intField = intField;
        }

        public int SetAndAddIntField(int intArg)
        {
            this.intField += intArg;
            return this.intField;
        }

        public TestReferenceClass Property
        {
            get { return this.objField; }
            set { this.objField = value; }
        }

        public int IntProperty
        {
            get { return this.intField; }
            set { this.intField = value; }
        }

        public TestReferenceClass SetObjField(TestReferenceClass objArg)
        {
            this.objField = objArg;

            return this.objField;
        }
    }
}
