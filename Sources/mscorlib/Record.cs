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

        public static implicit operator Record(Dictionary d)
        {
            return null;
        }

        public static implicit operator Dictionary(Record r)
        {
            return null;
        }
    }
}

