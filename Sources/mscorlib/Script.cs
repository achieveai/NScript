namespace System
{
    using System.Runtime.CompilerServices;

    [GlobalMethods, Extended, IgnoreNamespace]
    public static class Script
    {
        public static void Alert(bool b)
        {
        }

        public static void Alert(DateTime d)
        {
        }

        public static void Alert(Number n)
        {
        }

        public static void Alert(string message)
        {
        }

        public static bool Boolean(object o)
        {
            return false;
        }

        public static bool Confirm(string message)
        {
            return false;
        }

        public static object Eval(string s)
        {
            return null;
        }

        [ScriptAlias("ss.isNull")]
        public static bool IsNull(object o)
        {
            return false;
        }

        [ScriptAlias("ss.isNullOrUndefined")]
        public static bool IsNullOrUndefined(object o)
        {
            return false;
        }

        [ScriptAlias("ss.isUndefined")]
        public static bool IsUndefined(object o)
        {
            return false;
        }

        public static object Literal(string script, params object[] args)
        {
            return null;
        }

        [ScriptAlias("ss.loadScripts")]
        public static void LoadScripts(string[] scriptURLs)
        {
        }

        [ScriptAlias("ss.loadScripts")]
        public static void LoadScripts(string[] scriptURLs, Callback callback)
        {
        }

        [ScriptAlias("ss.loadScripts")]
        public static void LoadScripts(string[] scriptURLs, ContextualCallback callback, object context)
        {
        }

        [ScriptAlias("ss.onDomReady")]
        public static void OnDOMReady(Callback callback)
        {
        }

        [ScriptAlias("ss.onReady")]
        public static void OnReady(Callback callback)
        {
        }

        public static string Prompt(string message)
        {
            return null;
        }

        public static string Prompt(string message, string defaultValue)
        {
            return null;
        }

        [ScriptAlias("ss.require")]
        public static void RequireScripts(string[] scripts)
        {
        }

        [ScriptAlias("ss.require")]
        public static void RequireScripts(string[] scripts, Callback callback)
        {
        }

        [ScriptAlias("ss.require")]
        public static void RequireScripts(string[] scripts, ContextualCallback callback, object context)
        {
        }
    }
}

