// Class1.cs
//

using System;

namespace RealScript
{
    public class TempAttribute : Attribute
    {
    }

    public interface Foo
    {
        int Foo();
    }

    public interface FooBar
    {
        void Foo();
        int Bar(int foo);
    }

    public interface Fooo : Foo
    {
        void Fooo();
    }

    public delegate int FooFunction(int testc);

    public static class TmpC
    {
        public static void Foo(string str)
        {
            string.Format(str);
        }

        public static void Foo(string str, object obj)
        {
            string.Format(str, obj);
        }
    }

    public class Class1 : IDisposable, FooBar
    {

        public int instanceFoo = 10;
        public static int staticBar;
        public const string constString = "const string";

        public bool Func1(int i1, int i2)
        {
            int result = i1 + this.GetFooFunction(false)(i2);

            //Console.WriteLine("this is a test", result);

            return result > 10;
        }

        public void FuncRef(ref int i1, int i2, int i3)
        {
            // i2 = TestGeneric<int>.TestSubClass<int>.Foo<int>(i2, i3);
            if ((i2 == 3 && i3 == 10) &&
                (i3 == 2 || i2 == (i3 == 10 || i3 < 4 ? 4 : 10)) ||
                (i3 == 4 && i2 == 3))
            {
                i1 = Func2 (i2, 15);
            }
            else
            {
                i1 = Func2 (i2, 25);
            }
        }

        public int FuncIncrement(int i1, int i2)
        {
            i2 += Func2(i1++, 10);

            i2 += Func2(i1, i2++);

            return i2;
        }

        public int GetMore(int i)
        {
            return i + this.instanceFoo;
        }

        public static int GetMoreStatic(int i)
        {
            return i + Class1.staticBar;
        }

        public FooFunction GetFooFunction(bool needsStatic)
        {
            if (needsStatic)
            {
                return delegate(int i) { return Class1.GetMoreStatic(i + 1); };
            }
            else
            {
                return delegate(int i) { return this.Bar(i + (needsStatic ? 1 : 2)); };
            }
        }

        public int this[int tmp, int foo]
        {
            get
            {
                return tmp + foo;
            }

            set
            {
                this.instanceFoo = tmp;
                Class1.staticBar = foo;
            }
        }

        public void Swap<T>(ref T r1, ref T r2)
        {
            T tmp = r1;
            r1 = r2;
            r2 = tmp;
        }

        public string Print<T, U>(T i1, U i2) where T : struct where U : struct
        {
            return string.Format("{0}-{1}", i1, i2);
        }

        public string Print2<T, U>(T i1, U i2)
            where T : Foo
            where U : FooBar
        {
            return string.Format("{0}-{1}", i1, i2);
        }

        public long Func2(long i1, long i2)
        {
            return 0;
        }

        public int Func2(int i1, int i2)
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

        public int Foo1()
        {
            int i = 10;
            int j = 11;

            int k = Func2(this.instanceFoo, Class1.staticBar);

            FuncRef(ref k, i++, j);

            // Console.WriteLine(k);

            return k;
        }

        public void Foo(int i)
        {
            string.Format("Starting");

            for (int j = 0; j < i; j += (i == 10 || i == 20) ? 1 : 0, i -= 1)
            {
                if (j == 10)
                {
                    string.Format("{0}", j);
                    break;
                }
                else
                {
                    j += i;
                }
            }

            string.Format("Ending");

            int k = 0;
            while (k < i + 10)
            {
                if (k == 10)
                {
                    string.Format("{0}", k);
                }
                else
                {
                    k += i;
                }

                k += i == 10 ? 1 : 2;
                i -= 1;
            }

            string.Format("{0}", "Ending");
            string.Format("{0}", "Starting");

            for (int j = 0; j < i; j += i == 10 ? 1 : 2, i -= 1)
            {
                if (j == 10)
                {
                    if (i == 10)
                    {
                        string.Format("{0}", j);
                        break;
                    }
                    else
                    {
                        string.Format("{0}", 10);
                    }
                }
                else
                {
                    j += i;
                }
                string.Format("{0}", "foo");
            }

            string.Format("{0}", "Ending");

            int l = 0;
            while (l < i + 10)
            {
                if (l == 10)
                {
                    if (i == 10)
                    {
                        string.Format("{0}", l);
                        continue;
                    }
                    else
                    {
                        string.Format("{0}", 10);
                    }
                }
                else
                {
                    l += i;
                }

                string.Format("{0}", "foo");

                l += i == 10 ? 1 : 2;
                i -= 1;
            }

            string.Format("{0}", "Ending");
        }

        public void FooWhileTest(int i)
        {
            while (i > 10)
            {
                string.Format("{0}", i);
                i -= 10;
            }
            while (i > 4)
            {
                string.Format("{0}", i + 10);
                i -= 2;
            }

            do
            {
                string.Format("{0}", i - 1);
                if (i == 3)
                {
                    i += 1;
                    continue;
                }
                i += 2;
            } while (i < 10);

            if (i == 2)
            {
                while (i < 40)
                {
                    string.Format("{0}", i);
                    i += 10;
                }
            }
        }

        public void SimpleWhileLoop(int i)
        {
            string.Format("tmp");

            while(i > 10)
            {
                string.Format("{0}", i);

                i--;
            }

            string.Format("tmp");
        }

        public void SimpleDoWhileLoop(int i)
        {
            string.Format("tmp");

            do
            {
                string.Format("{0}", i);

                i--;
            } while (i > 10);

            string.Format("tmp");
        }

        public void SimpleForLoop(int i)
        {
            string.Format("tmp");

            for (int j = i; j > 10; j--)
            {
                string.Format("{0}", i);

                i--;
            }

            string.Format("tmp");
        }

        public void NestedWhileDoWhile(int i)
        {
            string.Format("tmp");

            while (i > 10)
            {
                do
                {
                    i++;
                } while (i%2 == 0);

                i--;
            }

            string.Format("tmp");
        }

        public void LastWhileBlock(int i)
        {
            string.Format("tmp");

            while (i > 10)
            {
                string.Format("{0}", i);

                i--;
            }
        }

        public int WhileLoopBreak(int i, int j)
        {
            j += i;

            do
            {
                i += j;

                if (i % 10 == 0)
                {
                    j++;

                    if (j == 10)
                    {
                        break;
                    }
                }

            } while (i < 100);

            return j;
        }

        public int WhileLoopContinue(int i, int j)
        {
            j += i;

            do
            {
                i += j;

                if (i % 10 == 0)
                {
                    j++;

                    if (j == 10)
                    {
                        continue;
                    }

                    i++;
                }

                j--;

            } while (i < 100);

            return j;
        }

        public int ForLoopBreak(int i, int j)
        {
            j += i;

            for(;i < 100; i++)
            {
                i += j;

                if (i % 10 == 0)
                {
                    j++;

                    if (j == 10)
                    {
                        break;
                    }
                }
            }

            return j;
        }

        public int ForLoopContinue(int i, int j)
        {
            j += i;

            for(;i < 100; i++)
            {
                i += j;

                if (i % 10 == 0)
                {
                    j++;

                    if (j == 10)
                    {
                        continue;
                    }

                    i++;
                }

                j--;
            }

            return j;
        }

        public void SimpleIfTest(int i)
        {
            string returnValue = "tmp";
            
            if (i == 10)
            {
                returnValue = "tmp" + "Foo";
            }

            returnValue = returnValue + "bar";

            Foo(returnValue.Length);
        }

        public void SimpleIfElseTest(int i)
        {
            string returnValue = "tmp";

            if (i == 10)
            {
                returnValue = "tmp" + "Foo";
            }
            else
            {
                returnValue = "tmp" + "bar";
            }

            returnValue = returnValue + "bar";

            Foo(returnValue.Length);
        }

        public int SimpleIfElseIfTest(int i)
        {
            string returnValue = "tmp";

            if (i == 10)
            {
                returnValue = "tmp" + "Foo";
            }
            else if (i == 20)
            {
                returnValue = "tmp" + "bar";
            }
            else
            {
                returnValue = returnValue + "bar";
            }

            return returnValue.Length;
        }

        public int NestedIfElseTest(int i)
        {
            int returnValue = 0;

            if (i > 10)
            {
                returnValue += 10;

                if (i == 15)
                {
                    returnValue += 5;
                }
                else
                {
                    returnValue += 2;
                }
            }
            else
            {
                returnValue = 11;
            }

            return returnValue;
        }

        public void FooSwitch(int i)
        {
            string.Format("{}");

            switch (i)
            {
                case 100:
                    string.Format("OneHundred");
                    break;
                case 10:
                    string.Format("Ten");
                    break;
                case 2:
                case 16:
                case 32:
                    string.Format("Power Of Two");
                    break;
                case 101:
                    string.Format("Contigous Test");
                    break;
                case 104:
                    string.Format("Contigous Test 2");
                    break;
                case 102:
                    string.Format("Contigous Test 3");
                    break;
                case 103:
                    string.Format("Contigous Test 4");
                    break;
                default:
                    string.Format("Default");
                    break;
            }

            //switch (i.ToString())
            //{
            //    case "100":
            //        Console.WriteLine("OneHundred");
            //        break;
            //    case "10":
            //        Console.WriteLine("Ten");
            //        break;
            //    case "101":
            //        Console.WriteLine("Contigous Test");
            //        break;
            //    case "104":
            //        Console.WriteLine("Contigous Test 2");
            //        break;
            //    case "102":
            //        Console.WriteLine("Contigous Test 3");
            //        break;
            //    case "103":
            //        Console.WriteLine("Contigous Test 4");
            //        break;
            //    default:
            //        Console.WriteLine("Default");
            //        break;
            //}

            //switch (i)
            //{
            //    case -100:
            //        Console.WriteLine("OneHundred");
            //        break;
            //    case -10:
            //        Console.WriteLine("Ten");
            //        break;
            //    case 2:
            //    case 16:
            //    case 32:
            //        Console.WriteLine("Power Of Two");
            //        break;
            //    case -101:
            //        Console.WriteLine("Contigous Test");
            //        break;
            //    case -104:
            //        Console.WriteLine("Contigous Test 2");
            //        break;
            //    case -102:
            //        Console.WriteLine("Contigous Test 3");
            //        break;
            //    case -103:
            //        Console.WriteLine("Contigous Test 4");
            //        break;
            //    default:
            //        Console.WriteLine("Default");
            //        break;
            //}

            //switch (i)
            //{
            //    case -100:
            //        string.Format("{0}", "OneHundred");
            //        break;
            //    case -10:
            //        string.Format("{0}", "Ten");
            //        break;
            //    case 32:
            //        string.Format("{0}", "Power Of Two");
            //        break;
            //    case 31:
            //        string.Format("{0}", "Power Of Three");
            //        break;
            //    case 132:
            //        string.Format("{0}", "Power Of Tfou");
            //        break;
            //}

            string.Format("{0}", "done with NoDefault stuff");

            //switch (i)
            //{
            //    case -100:
            //        string.Format("{0}", "OneHundred");
            //        break;
            //    case -10:
            //        string.Format("{0}", "Ten");
            //        break;
            //    case 32:
            //        string.Format("{0}", "Power Of Two");
            //        break;
            //    default:
            //        string.Format("{0}", "Default");
            //        break;
            //}
        }

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

        #region FooBar Members

        public void Foo()
        {
        }

        public int Bar(int foo)
        {
            if (foo.GetType() == typeof(int))
            {
                foo.Format("temp");
                return foo;
            }
            else
            {
                return 0;
            }
        }

        #endregion
    }
}
