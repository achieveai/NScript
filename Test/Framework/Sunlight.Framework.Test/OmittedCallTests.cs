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
        public static void LeadingOmittedForClausesKeepRemainingExpressions(Assert assert)
        {
            OmittedCallTests.Reset();
            int a = 0;
            int b = 0;
            int loops = 0;

            for (OmittedCallTests.Undefined(OmittedCallTests.Arg()), a = 1, b = 10;
                loops < 3;
                OmittedCallTests.Undefined(OmittedCallTests.Arg()), a += 1, b += 10)
            {
                loops++;
            }

            assert.Equal(loops, 3, "loop count");
            assert.Equal(a, 4, "first kept expression");
            assert.Equal(b, 40, "second kept expression");
            assert.Equal(OmittedCallTests.argEvals, 0, "no argument was evaluated");
        }

        // Each "after" statement must run once: it must not become the empty loop's body.
        [Test]
        public static void SoleOmittedCallLeavesAnEmptyLoopBody(Assert assert)
        {
            OmittedCallTests.Reset();
            int unbraced = 0;
            int braced = 0;
            int doUnbraced = 0;
            int doBraced = 0;
            int forUnbraced = 0;
            int afterUnbraced = 0;
            int afterBraced = 0;
            int afterDo = 0;
            int afterFor = 0;

            while (unbraced++ < 2)
                OmittedCallTests.Undefined(OmittedCallTests.Arg());
            afterUnbraced++;

            while (braced++ < 2)
            {
                OmittedCallTests.Undefined(OmittedCallTests.Arg());
            }
            afterBraced++;

            do
                OmittedCallTests.Undefined(OmittedCallTests.Arg());
            while (doUnbraced++ < 2);
            afterDo++;

            do
            {
                OmittedCallTests.Undefined(OmittedCallTests.Arg());
            }
            while (doBraced++ < 2);
            afterDo++;

            for (int i = 0; i < 2; i++, forUnbraced++)
                OmittedCallTests.Undefined(OmittedCallTests.Arg());
            afterFor++;

            assert.Equal(unbraced, 3, "unbraced while ran its condition");
            assert.Equal(afterUnbraced, 1, "unbraced while kept the next statement outside");
            assert.Equal(braced, 3, "braced while ran its condition");
            assert.Equal(afterBraced, 1, "braced while kept the next statement outside");
            assert.Equal(doUnbraced, 3, "unbraced do-while ran its condition");
            assert.Equal(doBraced, 3, "braced do-while ran its condition");
            assert.Equal(afterDo, 2, "do-while kept the next statements outside");
            assert.Equal(forUnbraced, 2, "for ran its iterator");
            assert.Equal(afterFor, 1, "for kept the next statement outside");
            assert.Equal(OmittedCallTests.argEvals, 0, "no argument was evaluated");
        }

        [Test]
        public static void SoleOmittedCallLeavesAnEmptyTryOrFinally(Assert assert)
        {
            OmittedCallTests.Reset();
            int caught = 0;
            int finallyRan = 0;
            int tryRan = 0;

            try
            {
                OmittedCallTests.Undefined(OmittedCallTests.Arg());
            }
            catch (Exception)
            {
                caught++;
            }

            try
            {
                OmittedCallTests.Undefined(OmittedCallTests.Arg());
            }
            finally
            {
                finallyRan++;
            }

            try
            {
                tryRan++;
            }
            finally
            {
                OmittedCallTests.Undefined(OmittedCallTests.Arg());
            }

            try
            {
                OmittedCallTests.Undefined(OmittedCallTests.Arg());
            }
            catch (Exception)
            {
                OmittedCallTests.Undefined(OmittedCallTests.Arg());
            }
            finally
            {
                OmittedCallTests.Undefined(OmittedCallTests.Arg());
            }

            assert.Equal(caught, 0, "nothing threw");
            assert.Equal(finallyRan, 1, "the finally ran after an empty try");
            assert.Equal(tryRan, 1, "the try ran before an empty finally");
            assert.Equal(OmittedCallTests.argEvals, 0, "no argument was evaluated");
        }

        [Test]
        public static void SoleOmittedCallWorksInOtherBlocks(Assert assert)
        {
            OmittedCallTests.Reset();
            int after = 0;
            int kept = 0;
            bool flag = OmittedCallTests.argEvals == 0;

            // The dropped call is the only reader of each loop variable.
            int iterations = 0;
            foreach (int item in new int[] { 1, 2 })
                OmittedCallTests.Undefined(item);
            after++;

            foreach (int item in new int[] { 1, 2 })
            {
                OmittedCallTests.Undefined(item);
                iterations++;
            }

            if (flag)
                while (kept++ < 1)
                    OmittedCallTests.Undefined(OmittedCallTests.Arg());
            else
                after = 99;

            switch (after)
            {
                case 1:
                    OmittedCallTests.Undefined(OmittedCallTests.Arg());
                    break;
                default:
                    after = 99;
                    break;
            }

            {
                OmittedCallTests.Undefined(OmittedCallTests.Arg());
            }

            assert.Equal(iterations, 2, "a foreach with an unread variable still iterates");
            assert.Equal(after, 1, "the next statements stayed outside");
            assert.Equal(kept, 2, "a nested empty while ran");
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
