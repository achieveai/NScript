namespace System
{
    using System.Collections;
    using System.Runtime.CompilerServices;

    [ScriptName("Error"), IgnoreNamespace, Imported]
    public class Exception
    {
        public Exception(string message)
        {
        }

        [ScriptAlias("Error.createError")]
        public extern static Exception Create(string message, Dictionary errorInfo);

        [ScriptAlias("Error.createError")]
        public extern static Exception Create(string message, Dictionary errorInfo, Exception innerException);

        [IntrinsicProperty]
        public Exception InnerException
        {
            get
            {
                return null;
            }
        }

        [IntrinsicProperty]
        public object this[string key]
        {
            get
            {
                return null;
            }
        }

        [IntrinsicProperty]
        public string Message
        {
            get
            {
                return null;
            }
        }
    }

    public class NotSupportedException : Exception
    {
        public NotSupportedException()
            : base(null)
        {
        }

        public NotSupportedException(string message)
            : base(message)
        {
        }
    }

    public class NotImplementedException : Exception
    {
        public NotImplementedException()
            : base(null)
        {
        }

        public NotImplementedException(string message)
            : base(message)
        {
        }
    }
}