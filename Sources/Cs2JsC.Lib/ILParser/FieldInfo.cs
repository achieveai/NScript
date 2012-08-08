using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cs2JsC.Lib.MetaData;

namespace Cs2JsC.Lib.ILParser
{
    public class FieldInfo : ScopeInfo
    {
        #region member variables
        private readonly ILField field = null;
        #endregion

        #region constructors/Factories
        public FieldInfo(
            ClassInfo classInfo,
            Scope scope)
            : base(scope)
        {
            this.Class = classInfo;
            this.field = ILField.ParseFromHeader(this.Scope.Header);
            this.Field = new Field(
                this.Class.Class,
                new MetaData.FieldSignature(
                    this.field.Id,
                    ParseUtils.ParseTypeSignature((StringFragment)this.field.Type),
                    this.Class.Class.Signature),
                this.field.Attributes, null);
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public ClassInfo Class
        {
            get;
            private set;
        }

        public Field Field
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
