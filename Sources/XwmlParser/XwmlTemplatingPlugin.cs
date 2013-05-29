namespace XwmlParser
{
    using NScript.Converter;
    using NScript.Converter.TypeSystemConverter;
    using NScript.JST;
    using System;
    using System.Collections.Generic;

    class XwmlTemplatingPlugin : IMethodConverterPlugin
    {
        public IntrestLevel GetInterestLevel(MethodConverter methodConverter)
        {
            return IntrestLevel.None;
        }

        public List<Statement> GetPreInsertionStatements(MethodConverter methodConverter)
        {
            throw new NotImplementedException();
        }

        public List<Statement> GetPostInsertionStatements(MethodConverter methodConverter)
        {
            throw new NotImplementedException();
        }

        public List<Statement> GetEncapsulationStatements(MethodConverter methodConverter, List<Statement> methodStatments)
        {
            throw new NotImplementedException();
        }

        public List<Statement> GetOverwrite(MethodConverter methodConverter)
        {
            throw new NotImplementedException();
        }

        public void Initialize(NScript.CLR.ClrContext clrContext, RuntimeScopeManager runtimeScopeManager)
        {
            throw new NotImplementedException();
        }

        public void ParseArgs(IList<Tuple<string, string>> args)
        {
        }
    }
}
