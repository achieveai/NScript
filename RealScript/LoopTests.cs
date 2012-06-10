using System;

namespace RealScript
{
    public class LoopTests
    {
        public void SimpleWhileLoop(int i)
        {
            TmpC.Foo("tmp");

            while(i > 10)
            {
                TmpC.Foo("{0}", i);
                i--;
            }

            TmpC.Foo("tmp");
        }

        public void SimpleDoWhileLoop(int i)
        {
            TmpC.Foo("tmp");

            do
            {
                TmpC.Foo("{0}", i);
                i--;
            } while (i > 10);

            TmpC.Foo("tmp");
        }

        public void SimpleForLoop(int i)
        {
            TmpC.Foo("tmp");

            for (int j = 0; j < i; j++)
            {
                TmpC.Foo("{0}", j);
            }

            TmpC.Foo("tmp");
        }

        public void NestedWhileDoWhile(int i)
        {
            TmpC.Foo("tmp");

            while (i > 10)
            {
                do
                {
                    i++;
                } while (i%2 == 0);

                i--;
            }

            TmpC.Foo("tmp");
        }

        public void OnlyWhileLoop(int i)
        {
            while(i > 10)
            {
                TmpC.Foo("{0}", i);
                i--;
            }
        }

        public void OnlyForLoop(int i)
        {
            for (int j = 0; j < i; j++)
            {
                TmpC.Foo("{0}", j);
            }
        }
    }
}
