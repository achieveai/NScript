using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Cs2JsC.Lib.MetaData
{
    public class Method
    {
        #region member variables

        #endregion

        #region constructors/Factories
        public Method(
            Class ownerClass,
            MethodSignature signature,
            MethodAttributes attributes,
            List<AttributeInfo> customAttributes)
        {
            this.OwnerClass = ownerClass;
            this.CustomAttributes = customAttributes;
            this.Attributes = attributes;
            this.Signature = signature;

            this.OwnerClass.Methods.Add(this);
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public Class OwnerClass
        { get; private set; }

        public IList<AttributeInfo> CustomAttributes
        { get; private set; }

        public MethodSignature Signature
        { get; private set; }

        public MethodAttributes Attributes
        {
            get;
            private set;
        }

        public AttachedMethodAttribute AttachedAttribute
        { get; internal set; }

        public bool IsIntrinsic
        { get; internal set; }

        public bool IsImported
        { get; internal set; }

        public bool IsDelegateConstructor
        { get { return this.Signature.IsConstructor && this.OwnerClass.IsDelegate; } }
        #endregion

        #region public functions
        public void Serialize(BinaryWriter writer)
        {
            this.Signature.Serialize(writer);
            writer.Write((int)this.Attributes);

            writer.Write(this.CustomAttributes.Count);
            for (int i = 0; i < this.CustomAttributes.Count; i++)
            {
                this.CustomAttributes[i].Serialize(writer);
            }

            writer.Write((int)this.AttachedAttribute);
            writer.Write(IsIntrinsic);
        }

        public static Method Deserialize(
            Class owner,
            BinaryReader reader)
        {
            var signature = MethodSignature.Deserialize(reader);
            var attributes = (MethodAttributes)reader.ReadInt32();

            List<AttributeInfo> customAttributes = new List<AttributeInfo>();
            for (int i = reader.ReadInt32() - 1; i >= 0; i--)
            {
                customAttributes.Add(AttributeInfo.Deserialize(reader));
            }

            return new Method(
                owner,
                signature,
                attributes,
                customAttributes)
                {
                    AttachedAttribute = (AttachedMethodAttribute)reader.ReadInt32(),
                    IsIntrinsic = reader.ReadBoolean(),
                };
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        #endregion
    }
}
