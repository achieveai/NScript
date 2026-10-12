namespace NScript
{
    using System.Linq;
    using NScript.Lib;
    using NScript.Lib.Service;

    public class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "service")
            {
                return ServiceHost.Run(args.Skip(1).ToArray());
            }

            return NScriptCompiler.Compile(args);
        }
    }
}