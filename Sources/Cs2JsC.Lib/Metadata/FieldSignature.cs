using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Cs2JsC.Lib.MetaData
{
    public class FieldSignature : MemberSignature
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public FieldSignature(
            string name,
            ClassSignature typeSignature,
            bool isStatic,
            ClassSignature classSignature)
            : base(name, typeSignature, classSignature)
        {
            this.IsStatic = isStatic;
        }

        public FieldSignature(
            string name,
            ClassSignature typeSignature,
            ClassSignature classSignature)
            : base(name, typeSignature, classSignature)
        { }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        #endregion

        #region public functions
        public void SetStatic(bool isStatic)
        {
            this.IsStatic = isStatic;
        }

        public override bool Equals(object obj)
        {
            return obj is FieldSignature && base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(this.Name);
            this.Type.Serialize(writer);
            this.Class.Serialize(writer);
            writer.Write(this.IsStatic);
        }

        public static FieldSignature Deserialize(BinaryReader reader)
        {
            var returnValue = new FieldSignature(
                reader.ReadString(),
                ClassSignature.Deserialize(reader),
                ClassSignature.Deserialize(reader));
            returnValue.IsStatic = reader.ReadBoolean();

            return returnValue;
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        #endregion
    }
}
