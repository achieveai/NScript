using System;
using System.Runtime.CompilerServices;

namespace System
{
    [ImportedType]
    public class NativeGenerator
    {
        [ScriptAlias("next")]
        public extern NextObject Next();

        public class NextObject
        {
            [IntrinsicProperty]
            public extern object Value
            {
                get;
            }

            [IntrinsicProperty]
            public extern bool Done
            {
                get;
            }
        }
    }
}
