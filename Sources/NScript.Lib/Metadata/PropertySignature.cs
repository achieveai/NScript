using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NScript.Lib.MetaData
{
    public class PropertySignature : MemberSignature
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public PropertySignature(
            string name,
            ClassSignature typeSignature,
            bool isStatic,
            ClassSignature classSignature)
            : base(name, typeSignature, classSignature)
        {
            this.IsStatic = isStatic;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        #endregion

        #region public functions
        public override bool Equals(object obj)
        {
            return obj is PropertySignature && base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void Serialize(System.IO.BinaryWriter writer)
        {
            writer.Write(this.Name);
            this.Type.Serialize(writer);
            writer.Write(this.IsStatic);
            this.Class.Serialize(writer);
        }

        public static PropertySignature Deserialize(System.IO.BinaryReader reader)
        {
            return new PropertySignature(
                reader.ReadString(),
                ClassSignature.Deserialize(reader),
                reader.ReadBoolean(),
                ClassSignature.Deserialize(reader));
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        #endregion
    }
}
