
namespace NScript.Csc.Lib
{
    using Microsoft.CodeAnalysis;

    internal class SerializationContext
    {
        public SerializationContext(SymbolSerializer symbolSerializer)
        {
            SymbolSerializer = symbolSerializer;
        }

        public SymbolSerializer SymbolSerializer
        { get; }

        public int Depth
        { get; set; }
    }
}