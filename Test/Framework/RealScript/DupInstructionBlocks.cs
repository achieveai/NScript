//-----------------------------------------------------------------------
// <copyright file="DupInstructionBlocks.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    /// <summary>
    /// Definition for DupInstructionBlocks
    /// </summary>
    public class DupInstructionBlocks
    {
        static int intField = 0;

        public static int[] PostfixOperation(int i)
        {
            return new int[]
                       {
                           i++,
                           i,
                       };
        }

        public static void PostfixPropertyOperation(TestReferenceClass referenceClass)
        {
            referenceClass.intField = referenceClass.IntProperty++;
        }

        public static void PostfixSecondOrderPropertyOperation(TestReferenceClass referenceClass)
        {
            referenceClass.intField = referenceClass.Property.IntProperty++;
        }

        public static int PostfixPropertyInConditionOperation(TestReferenceClass referenceClass)
        {
            int returnValue = 0;
            while (0 == referenceClass.IntProperty++)
            {
                returnValue = 2 * returnValue;
            }

            return returnValue;
        }

        public static void PropertyInlineEquals(TestReferenceClass referenceClass)
        {
            referenceClass.intField = (referenceClass.IntProperty = referenceClass.Property.intField);
        }

        public static int FunctionCallWithInlineAssignment(int i, Class1 cl1)
        {
            return cl1.Func2((i = cl1.Foo1()), i + 10);
        }

        public static int LocalInlineEquals(TestReferenceClass referenceClass)
        {
            int loc;
            referenceClass.intField = loc = referenceClass.IntProperty;
            loc = 10 + loc;
            return loc;
        }

        public static void PostfixPropertyOnPropertyOperation(TestReferenceClass referenceClass)
        {
            referenceClass.Property.intField = referenceClass.Property.IntProperty++;
        }

        public static void LocalOpAssignmentOperation(int i1, int i2, Class1 cl1)
        {
            i1 += i2;
            cl1.Func2(i1, i2);
        }

        public static void PropertyOpAssignmentOperation(TestReferenceClass trc1, int i2)
        {
            trc1.IntProperty += i2;
        }

        public static void PropertyOnPropertyOpAssignmentOperation(TestReferenceClass trc1, int i2)
        {
            trc1.Property.IntProperty *= i2;
        }

        public static void NullCheckAssignment(TestReferenceClass trc1, TestReferenceClass dest)
        {
            dest.Property = trc1.Property ?? trc1;
        }

        public static void InlineOpAssignment(TestReferenceClass trc1)
        {
            trc1.intField = trc1.IntProperty += 10;
        }

        public static void PrefixFieldIncrement(TestReferenceClass trc1)
        {
            ++trc1.intField;
        }

        public static void PrefixStaticFieldIncrement(TestReferenceClass trc1)
        {
            ++DupInstructionBlocks.intField;
        }

        public static void PrePostfixFieldIncrementAssignment(TestReferenceClass trc1)
        {
            trc1.IntProperty = ++trc1.intField;
            trc1.IntProperty = trc1.intField++;
        }

        public static void PrePostfixStaticFieldIncrementAssignment(TestReferenceClass trc1)
        {
            trc1.IntProperty = ++DupInstructionBlocks.intField;
            trc1.IntProperty = DupInstructionBlocks.intField++;
        }

        public static bool RegressionPreIncrementInCompare(TestReferenceClass trc1)
        {
            return ++trc1.intField < trc1.objField.intField;
        }
    }
}