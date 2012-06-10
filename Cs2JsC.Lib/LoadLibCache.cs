using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace Cs2JsC.Lib
{
    public static class LoadLibCache
    {
        static Dictionary<string, LoadLib> loadLibCache = new Dictionary<string, LoadLib>();

        public static LoadLib LoadAssembly(string assemblyName)
        {
            var assemblyNameOnly = System.IO.Path.GetFileName(assemblyName);

            LoadLib returnValue = null;

            if (!LoadLibCache.loadLibCache.TryGetValue(assemblyNameOnly, out returnValue))
            {
                returnValue = new LoadLib(assemblyName);
                LoadLibCache.loadLibCache.Add(assemblyNameOnly, returnValue);
            }

            return returnValue;
        }

        public static LoadLib LoadAssembly(AssemblyName assemblyName, string currentAssemblyPath)
        {
            return LoadLibCache.LoadAssembly(
                LoadLibCache.ResolveAssemblyName(
                    assemblyName.Name,
                    currentAssemblyPath));
        }

        private static string ResolveAssemblyName(string shortAssemblyName, string currentAssemblyPath)
        {
            string[] assemblyExtensions = { ".dll", ".exe" };

            foreach (var extension in assemblyExtensions)
            {
                if (File.Exists(
                    Path.Combine(
                        Path.GetDirectoryName(currentAssemblyPath),
                        shortAssemblyName + extension)))
                {
                    return Path.Combine(
                        Path.GetDirectoryName(
                            currentAssemblyPath),
                        shortAssemblyName + extension);
                }
            }

            return shortAssemblyName + ".dll";
        }
    }
}
