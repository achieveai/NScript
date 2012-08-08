using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cs2JsC.Lib.ILParser
{
    public class ILAssemblyParser
    {
        #region member variables
        #endregion

        #region constructors/Factories
        private ILAssemblyParser()
        { }

        public static ILAssemblyParser ParseFromHeader(string header)
        {
            var word = ParseUtils.GetNextWord(header, 0);

            if (word != IlStrings.ScopeNames.AssemblyType)
            {
                throw new ArgumentException("header should belong to Assembly");
            }

            word = ParseUtils.GetNextWord(word);

            bool isExtern = false;
            if (word == "extern")
            {
                isExtern = true;
                word = ParseUtils.GetNextWord(word);
            }

            string assemblyName = (string)word;

            return new ILAssemblyParser()
            {
                IsExtern = isExtern,
                Name = assemblyName,
            };
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public bool IsExtern
        { get; private set; }

        public string Name
        { get; private set; }
        #endregion

        #region public functions
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        #endregion
    }
}
