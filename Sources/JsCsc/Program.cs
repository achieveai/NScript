using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JsCsc.Lib;

namespace JsCsc
{
    class Program
    {
        static void Main(string[] args)
        {
            DriverWrapper wrapper = new DriverWrapper();
            wrapper.Compile(args);
            System.Environment.Exit(0);
        }
    }
}
