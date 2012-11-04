namespace System.Testing
{
    using System;
    using System.Runtime.CompilerServices;

    [IgnoreNamespace, Extended]
    public static class TestEngine
    {
        [ScriptAlias("QUnit.log")]
        public static void Log(string message)
        {
        }

        [ScriptAlias("QUnit.start")]
        public static void ResumeOnAsyncCompleted()
        {
        }

        [ScriptAlias("QUnit.triggerEvent")]
        public static void TriggerEvent(object element, string eventName)
        {
        }

        [ScriptAlias("QUnit.stop")]
        public static void WaitForAsyncCompletion()
        {
        }

        [ScriptAlias("QUnit.stop")]
        public static void WaitForAsyncCompletion(int timeout)
        {
        }
    }
}

