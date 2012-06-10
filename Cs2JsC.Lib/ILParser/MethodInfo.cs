using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cs2JsC.Lib.MetaData;

namespace Cs2JsC.Lib.ILParser
{
    public class MethodInfo : ScopeInfo
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public MethodInfo(
            ClassInfo classInfo,
            Scope scope,
            bool parseMetadataOnly,
            List<string> typeGenericParams = null)
            : base(scope)
        {
            this.Class = classInfo;
            var method = ILMethod.ParseFromHeader(scope.Header, false, typeGenericParams);

            this.Method = new Method(
                this.Class.Class,
                new MethodSignature(
                    method.MethodName,
                    ParseUtils.ParseTypeSignature((StringFragment)method.ReturnType),
                    (method.Attributes & MethodAttributes.Static) == MethodAttributes.Static,
                    method.Arguments,
                    this.Class.Class.Signature),
                    method.Attributes,
                    AttributeParser.GetAttributes(this.Scope.ScopedLines, 0));

            if (!parseMetadataOnly)
            {
                this.ExecutionBlock = new ExecutionBlock(
                    this,
                    scope.ScopedLines);
            }
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public Method Method
        {
            get;
            private set;
        }

        public ClassInfo Class
        {
            get;
            private set;
        }

        internal ExecutionBlock ExecutionBlock
        {
            get;
            private set;
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
