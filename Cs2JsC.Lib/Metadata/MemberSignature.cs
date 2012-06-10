using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cs2JsC.Lib.MetaData
{
    public abstract class MemberSignature
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public MemberSignature(
            string memberName,
            ClassSignature type,
            ClassSignature classSignature)
        {
            this.Type = type;
            this.Name = memberName;
            this.Class = classSignature;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public ClassSignature Class { get; private set; }

        public string Name { get; private set; }

        public ClassSignature Type { get; protected set; }

        public bool IsStatic { get; protected set; }
        #endregion

        #region public functions
        public override bool Equals(object obj)
        {
            MemberSignature rightObj = obj as MemberSignature;
            return rightObj != null &&
                this.Name == rightObj.Name &&
                this.Type == rightObj.Type &&
                this.Class.Equals(rightObj.Class);
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode() + this.Class.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0}::{1}", this.Class, this.Name);
        }

        public static bool operator ==(MemberSignature left, MemberSignature right)
        {
            return object.Equals(left, right);
        }

        public static bool operator !=(MemberSignature left, MemberSignature right)
        {
            return !object.Equals(left, right);
        }
        #endregion
    }
}
