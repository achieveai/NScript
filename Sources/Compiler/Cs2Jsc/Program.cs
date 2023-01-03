namespace NScript
{
    using System;
    using System.Linq;
    using NScript.Csc.Lib;
    using NScript.Lib;

    public static class Program
    {
        public static int Main(string[] args)
        {
            if (args[0] == "csc")
            {
                return CscCompiler.Main(args.Skip(1).ToArray());
            }
            else if (args[0] == "cs2jsc")
            {
                return NScriptCompiler.Compile(args.Skip(1).ToArray());
            }
            else
            {
                PrintUsage();
                return 0;
            }
        }

        public static void PrintUsage()
        {
            Console.WriteLine("Usage: NScript <csc | cs2jsc> <arguments>");
        }
    }
}