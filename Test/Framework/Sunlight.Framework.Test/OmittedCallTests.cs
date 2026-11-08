#define NSCRIPT_OMIT_DEFINED

namespace Sunlight.Framework.Test
{
    using System;
    using System.Diagnostics;
    using SunlightUnit;

    /// <summary>
    /// #152: a call to a [Conditional] method whose symbol is undefined is gone, with its
    /// argument evaluation, in every statement shape.
    /// </summary>
    [TestFixture]
    public class OmittedCallTests
    {
        private sealed class Receiver
        {
            [Conditional("NSCRIPT_OMIT_UNDEFINED")]
            public void Mark()
            {
                OmittedCallTests.marks++;
            }
        }

        private static int marks;
        private static int argEvals;

        [Conditional("NSCRIPT_OMIT_UNDEFINED")]
        private static void Undefined(int value)
        {
            OmittedCallTests.marks++;
        }

        [Conditional("NSCRIPT_OMIT_DEFINED")]
        private static void Defined()
        {
            OmittedCallTests.marks++;
        }

        private static int Arg()
        {
            OmittedCallTests.argEvals++;
            return 1;
        }

        private static void ExpressionBodied() => OmittedCallTests.Undefined(OmittedCallTests.Arg());

        private static void Reset()
        {
            OmittedCallTests.marks = 0;
            OmittedCallTests.argEvals = 0;
        }

        [Test]
        public static void UndefinedSymbolDropsTheCallAndItsArguments(Assert assert)
        {
            OmittedCallTests.Reset();
            OmittedCallTests.Undefined(OmittedCallTests.Arg());
            assert.Equal(OmittedCallTests.marks, 0, "the method did not run");
            assert.Equal(OmittedCallTests.argEvals, 0, "its argument was not evaluated");
        }

        [Test]
        public static void OmittedCallWorksInEveryStatementShape(Assert assert)
        {
            OmittedCallTests.Reset();
            bool flag = OmittedCallTests.argEvals == 0;
            int loops = 0;

            if (flag) OmittedCallTests.Undefined(OmittedCallTests.Arg());
            else OmittedCallTests.Undefined(OmittedCallTests.Arg());

            for (int i = 0; i < 2; i++, OmittedCallTests.Undefined(OmittedCallTests.Arg()))
                loops++;

            int onlyOmitted = 0;
            for (OmittedCallTests.Undefined(OmittedCallTests.Arg()); onlyOmitted < 3; OmittedCallTests.Undefined(OmittedCallTests.Arg()))
                onlyOmitted++;

            Action lambda = () => OmittedCallTests.Undefined(OmittedCallTests.Arg());
            lambda();
            OmittedCallTests.ExpressionBodied();

            var receiver = new Receiver();
            receiver?.Mark();
            receiver.Mark();

            assert.Equal(loops, 2, "the loop still runs");
            assert.Equal(onlyOmitted, 3, "a loop whose only clauses were omitted still runs");
            assert.Equal(OmittedCallTests.marks, 0, "no call ran");
            assert.Equal(OmittedCallTests.argEvals, 0, "no argument was evaluated");
        }

        [Test]
        public static void DefinedSymbolKeepsTheCall(Assert assert)
        {
            OmittedCallTests.Reset();
            OmittedCallTests.Defined();
            assert.Equal(OmittedCallTests.marks, 1, "the method ran");
        }
    }
}
