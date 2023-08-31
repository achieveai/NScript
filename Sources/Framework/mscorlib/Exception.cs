namespace System
{
    using System.Collections;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    public interface IAsyncDisposable
    {
        ValueTask DisposeAsync();
    }

    [ScriptName("Error"), IgnoreNamespace, Extended]
    public class Exception
    {
        public extern Exception(string message);

        [ScriptAlias("Error.createError")]
        public extern static Exception Create(string message, Dictionary errorInfo);

        [ScriptAlias("Error.createError")]
        public extern static Exception Create(string message, Dictionary errorInfo, Exception innerException);

        [IntrinsicProperty]
        public static extern int? StackTraceLimit
        { get; set; }

        [IntrinsicProperty]
        public extern Exception InnerException
        { get; }

        [IntrinsicProperty]
        public extern object this[string key]
        { get; }

        [IntrinsicProperty]
        public extern string Message
        { get; }

        [IntrinsicProperty]
        public extern string ColumnNumber
        { get; }

        [IntrinsicProperty]
        public extern string LineNumber
        { get; }

        [IntrinsicProperty]
        public extern string FileName
        { get; }

        [IntrinsicProperty]
        public extern string Stack
        { get; }
    }

    public class ArgumentException : Exception
    {
        public ArgumentException()
            : base(null)
        {
        }

        public ArgumentException(string message)
            : base(message)
        {
        }
    }

    public class ArgumentNullException : ArgumentException
    {
        public ArgumentNullException()
        {
        }

        public ArgumentNullException(string message)
            : base(message)
        {
        }
    }

    public class InvalidProgramException : Exception
    {
        public InvalidProgramException()
            : base(null)
        {
        }

        public InvalidProgramException(string message)
            : base(message)
        {
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

    public class EventBasedException : Exception
    {
        private readonly Event _evt;

        public EventBasedException(Event evt)
            :base(evt.Type)
        { _evt = evt; }

        public EventBasedException(
            string message,
            Event evt)
            :base(message)
        { _evt = evt; }

        public Event Event
        { get { return _evt; } }
    }
}