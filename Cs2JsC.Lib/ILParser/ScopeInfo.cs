using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cs2JsC.Lib.ILParser
{
    public class ScopeInfo
    {
        #region member variables
        private readonly Scope scope;
        #endregion

        #region constructors/Factories
        protected ScopeInfo(Scope scope)
        {
            this.scope = scope;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public Scope Scope
        {
            get { return this.scope; }
        }
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
