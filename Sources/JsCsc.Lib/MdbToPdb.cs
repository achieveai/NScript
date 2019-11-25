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
        }
    }
}