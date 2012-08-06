namespace System.Net
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml;

    [Imported, ScriptName("XMLHttpRequest"), IgnoreNamespace]
    public sealed class XmlHttpRequest
    {
        public void Abort()
        {
        }

        public string GetAllResponseHeaders()
        {
            return null;
        }

        public string GetResponseHeader(string name)
        {
            return null;
        }

        public void Open(string method, string url)
        {
        }

        public void Open(string method, string url, bool async)
        {
        }

        public void Open(string method, string url, bool async, string userName, string password)
        {
        }

        public void Send()
        {
        }

        public void Send(string body)
        {
        }

        public void SetRequestHeader(string name, string value)
        {
        }

        [IntrinsicProperty, ScriptName("onreadystatechange")]
        public Callback OnReadyStateChange
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        [IntrinsicProperty]
        public System.Net.ReadyState ReadyState
        {
            get
            {
                return System.Net.ReadyState.Uninitialized;
            }
        }

        [IntrinsicProperty]
        public string ResponseText
        {
            get
            {
                return null;
            }
        }

        [ScriptName("responseXML"), IntrinsicProperty]
        public XmlDocument ResponseXml
        {
            get
            {
                return null;
            }
        }

        [IntrinsicProperty]
        public int Status
        {
            get
            {
                return 0;
            }
        }

        [IntrinsicProperty]
        public string StatusText
        {
            get
            {
                return null;
            }
        }
    }
}

