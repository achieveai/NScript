namespace System
{
    using System.Collections;
    using System.Runtime.CompilerServices;

    [ScriptNamespace("ss"), Extended]
    public abstract class Record
    {
        protected Record()
        {
        }

        [IntrinsicOperator]
        public extern static implicit operator Record(Dictionary d);

        [IntrinsicOperator]
        public extern static implicit operator Dictionary(Record r);
    }
}

