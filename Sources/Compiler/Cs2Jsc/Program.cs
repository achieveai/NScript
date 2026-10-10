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
            if (args.Length == 0)
            {
                PrintUsage();
                return 0;
            }
            else if (args[0] == "csc")
            {
                return CscCompiler.Main(args.Skip(1).ToArray());
            }
            else if (args[0] == "service")
            {
                // How ServiceLauncher starts a daemon from this (NuGet tool) layout.
                return NScript.Lib.Service.ServiceHost.Run(args.Skip(1).ToArray());
            }
            else if (args[0] == "cs2jsc" && args.Length > 1 && args[1] == "service")
            {
                // The SDK's nscript.cmd runs `Cs2Jsc cs2jsc service --sync ...`.
                return NScript.Lib.Service.ServiceHost.Run(args.Skip(2).ToArray());
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
            Console.WriteLine("Usage: NScript <csc | cs2jsc | service> <arguments>");
        }
    }
}