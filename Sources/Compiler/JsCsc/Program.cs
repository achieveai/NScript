using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JsCsc.Lib;
using NScript.Csc.Lib;

namespace JsCsc
{
    class Program
    {
        static int Main(string[] args)
        {
            return CscCompiler.Main(args);
            // DriverWrapper wrapper = new DriverWrapper();
            // wrapper.Compile(args);
            // System.Environment.Exit(0);
        }
    }
}
