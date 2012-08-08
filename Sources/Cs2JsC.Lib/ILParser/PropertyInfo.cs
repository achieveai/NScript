using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cs2JsC.Lib.MetaData;

namespace Cs2JsC.Lib.ILParser
{
    public class PropertyInfo : ScopeInfo
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public PropertyInfo(
            ClassInfo classInfo,
            Scope scope,
            List<string> typeGenericParams = null)
            : base(scope)
        {
            this.Class = classInfo;
            var property = ILMethod.ParseFromHeader(scope.Header, true, typeGenericParams);

            for (int i = 0; i < this.Scope.ScopedLines.Count; i++)
            {
                string line = this.Scope.ScopedLines[i];

                var word = ParseUtils.GetNextWord(line, 0);
                switch ((string)word)
                {
                    case ".get":
                        this.Getter = ParseUtils.ParseMethodSignature(
                            word.RemainingParentPart());
                        break;
                    case ".set":
                        this.Setter = ParseUtils.ParseMethodSignature(
                            word.RemainingParentPart());
                        break;
                    default:
                        break;
                }
            }

            this.Property = new Property(
                classInfo.Class,
                new MethodSignature(
                    property.MethodName,
                    ParseUtils.ParseTypeSignature((StringFragment)property.ReturnType),
                    (property.Attributes & MethodAttributes.Static) == MethodAttributes.Static,
                    property.Arguments,
                    classInfo.Class.Signature),
                (property.Attributes & MethodAttributes.Static) == MethodAttributes.Static,
                this.Getter,
                this.Setter,
                AttributeParser.GetAttributes(this.Scope.ScopedLines, 0));
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public ClassInfo Class
        { get; private set; }

        public Property Property
        { get; private set; }

        public MethodSignature Setter
        { get; private set; }

        public MethodSignature Getter
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
