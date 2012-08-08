using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Cs2JsC.Lib.MetaData
{
    public class Field
    {
        #region constructors/Factories
        public Field(
            Class ownerClass,
            FieldSignature signature,
            FieldAttributes attributes,
            List<AttributeInfo> customAttributes)
        {
            this.OwnerClass = ownerClass;
            this.Signature = signature;
            this.Attributes = attributes;
            this.CustomAttributes = customAttributes;

            this.OwnerClass.Fields.Add(this);
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public Class OwnerClass
        { get; private set; }

        public IList<AttributeInfo> CustomAttributes
        { get; private set; }

        public FieldSignature Signature
        { get; private set; }

        public FieldAttributes Attributes
        { get; private set; }
        #endregion

        #region public functions
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        #endregion

        internal void Serialize(BinaryWriter writer)
        {
            this.Signature.Serialize(writer);
            writer.Write((int)this.Attributes);

            for (int i = 0; i < this.CustomAttributes.Count; i++)
            {
                this.CustomAttributes[i].Serialize(writer);
            }
        }

        internal static Field Deserialize(
            Class ownerClass,
            System.IO.BinaryReader reader)
        {
            var signature = FieldSignature.Deserialize(reader);
            var attributes = (FieldAttributes)reader.ReadInt32();

            var customAttributes = new List<AttributeInfo>();
            for (int i = reader.ReadInt32() - 1; i >= 0; i--)
            {
                customAttributes.Add(AttributeInfo.Deserialize(reader));
            }

            return new Field(
                ownerClass,
                signature,
                attributes,
                customAttributes);
        }
    }
}
