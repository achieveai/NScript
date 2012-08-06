//-----------------------------------------------------------------------
// <copyright file="MdbToPdb.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JsCsc.Lib
{
    using Mono.Cecil;
    using Mono.Cecil.Mdb;
    using Mono.Cecil.Pdb;

    internal static class MdbToPdb
    {
        private static MdbReaderProvider readerProvider = new MdbReaderProvider();
        private static PdbWriterProvider writerProvider = new PdbWriterProvider();
        private static ModuleDefinition moduleDefinition;

        public static void Convert(string dllFileName, string pdbFileName)
        {
            moduleDefinition = ModuleDefinition.ReadModule(
                dllFileName,
                new ReaderParameters()
                {
                    ReadSymbols = true,
                    SymbolReaderProvider = readerProvider
                });

            var writer = writerProvider.GetSymbolWriter(moduleDefinition, pdbFileName);

            foreach (var type in moduleDefinition.Types)
                foreach (var method in type.Methods)
                {
                    if (method.HasBody)
                    {
                        try
                        {
                            writer.Write(method.Body);
                        }
                        catch { }
                    }
                }

            writer.Dispose();
        }
    }
}