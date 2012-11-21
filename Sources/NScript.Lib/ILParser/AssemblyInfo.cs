using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NScript.Lib.MetaData;

namespace NScript.Lib.ILParser
{
    public class AssemblyInfo : ScopeInfo
    {
        #region member variables
        public const string AssemblyInfoTag = ".assembly";
        #endregion

        #region constructors/Factories
        public AssemblyInfo(Scope scope)
            : base(scope)
        {
            this.AssemblyParser = ILAssemblyParser.ParseFromHeader(scope.Header);
            AssemblySignature assemblySignature = new AssemblySignature(this.AssemblyParser.Name);

            if (!this.AssemblyParser.IsExtern)
            {
                NameResolver.Instance.CurrentAssembly = assemblySignature;
            }

            var attributes = AttributeParser.GetAttributes(scope.ScopedLines, 0);
            this.Assembly = new Assembly(
                assemblySignature,
                this.AssemblyParser.IsExtern,
                attributes);
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public ILAssemblyParser AssemblyParser
        {
            get;
            private set;
        }

        public Assembly Assembly
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
