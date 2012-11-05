namespace System
{
    using System.Collections;
    using System.Runtime.CompilerServices;

    [ScriptName("Error"), IgnoreNamespace, Extended]
    public class Exception
    {
        public extern Exception(string message);

        [ScriptAlias("Error.createError")]
        public extern static Exception Create(string message, Dictionary errorInfo);

        [ScriptAlias("Error.createError")]
        public extern static Exception Create(string message, Dictionary errorInfo, Exception innerException);

        [IntrinsicProperty]
        public extern Exception InnerException
        { get; }

        [IntrinsicProperty]
        public extern object this[string key]
        { get; }

        [IntrinsicProperty]
        public extern string Message
        { get; }
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