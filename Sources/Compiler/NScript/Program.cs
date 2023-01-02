namespace NScript
{
    using System;
    using System.Linq;
    using NScript.Csc.Lib;

    public static class Program
    {
        public static int Main(string[] args)
        {
            Console.WriteLine($"Tool args (len: {args.Length}): ");
            for (var i = 0; i < args.Length; i++)
            {
                Console.Write($"arg{i}: ");
                Console.WriteLine(args[i]);
            }
            if (args[0] == "csc")
            {
                args[0] = "dotnet.exe";
                return CscCompiler.Main(args.Skip(1).ToArray());
            }
            else
            {
                return CommandLine.RunCs2jsc(args.Skip(1).ToArray());
            }
        }
    }
}