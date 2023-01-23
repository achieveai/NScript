namespace NScript
{
    using NScript.Lib;

    public class Program
    {
        public static int Main(string[] args)
        {
            return NScriptCompiler.Compile(args);
        }
    }
}