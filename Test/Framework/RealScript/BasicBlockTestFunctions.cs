using System;

namespace RealScript
{
    public class BasicBlockTestFunctions
    {
        public static long ReturnInt(long l1, long l2)
        {
            long retVal = 0;

            return retVal;
        }

        public static string Call2ArgFormat(long l1, long l2)
        {
            string retVal = string.Format("{0}-{1}", l1, l2);

            return retVal;
        }

        public void Complex1ConditionalArgument(ref int i1, int i2, int i3)
        {
            i1 = Func (
                i2,
                (i2 == 3 && i1 == 10) && (i3 == 2 || i2 == 8) || (i3 == 4 && i2 == 3)
                    ? 25 : 14);
        }

        public void Complex2ConditionalArgument(ref int i1, int i2, int i3)
        {
            i1 = Func(i2,
                (i2 == 3 && i3 == 10) && (i3 == 2 || i2 == (i3 == 10 || i3 < 4 ? 4 : 10)) || (i3 == 4 && i2 == 3)
                ? 24 : 14);
        }

        public void Complex2IfCondition(ref int i1, int i2, int i3)
        {
            if ((i2 == 3 && i3 == 10) &&
                (i3 == 2 || i2 == (i3 == 10 || i3 < 4 ? 4 : 10)) ||
                (i3 == 4 && i2 == 3))
            {
                i1 = Func(i2, 15);
            }
            else
            {
                i1 = Func(i2, 25);
            }
        }

        public int Complex3IfCondition(int i1, int i2)
        {
            bool b = false;
            int returnValue = 0;
            if ((i2 == 3 && i1 == 10) &&
                (i1 == 1 || i2 == 9) &&
                (i1 == 2 || i2 == 3) ||
                (i1 == 4 && i2 == 3))
            {
                returnValue = i1 + i2 * 2;
            }
            else if (i1 > 2 && i2 < 3)
            {
                returnValue = i1 + i2;
            }
            else if (b)
            {
                returnValue = i2 - i1;
            }

            returnValue += i2;

            return returnValue;
        }

        public int FuncIncrement(int i1, int i2)
        {
            i2 += Func(i1++, 10);
            i2 += Func(i1, i2++);
            return i2;
        }

        public int FuncPostIncrementWithProperty(TestReferenceClass c1, int i2)
        {
            i2 += Func(c1.IntProperty++, 10);
            return i2;
        }

        public void SimpleConditionalArgument(ref int l1, int i2, int i3)
        {
            l1 = this.Func(i2, i3 == 10 ? 25 : 14);
        }

        private int Func(int i1, int i2)
        {
            return i1 + i2;
        }
    }
}
