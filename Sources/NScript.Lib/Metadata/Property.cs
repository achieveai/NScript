using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NScript.Lib.MetaData
{
    public class Property
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public Property(
            Class ownerClass,
            MethodSignature signature,
            bool isStatic,
            MethodSignature getterSignature,
            MethodSignature setterSignature,
            List<AttributeInfo> customAttributes)
        {
            this.OwnerClass = ownerClass;
            this.Signature = signature;
            this.IsStatic = isStatic;
            this.GetMethod = getterSignature;
            this.SetMethod = setterSignature;
            this.CustomAttributes = customAttributes;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public MethodSignature SetMethod
        { get; internal set; }

        public MethodSignature GetMethod
        { get; internal set; }

        public Class OwnerClass
        { get; private set; }

        public MethodSignature Signature
        { get; private set; }

        public bool IsStatic
        { get; private set; }

        public List<AttributeInfo> CustomAttributes
        { get; private set; }
        #endregion

        #region public functions

        internal void Serialize(
            BinaryWriter writer)
        {
            this.Signature.Serialize(writer);
            writer.Write(this.IsStatic);

            writer.Write(this.GetMethod != null);
            if (this.GetMethod != null)
            {
                this.GetMethod.Serialize(writer);
            }

            writer.Write(this.SetMethod != null);
            if (this.SetMethod != null)
            {
                this.SetMethod.Serialize(writer);
            }

            writer.Write(this.CustomAttributes.Count);
            for (int i = 0; i < this.CustomAttributes.Count; i++)
            {
                this.CustomAttributes[i].Serialize(writer);
            }
        }

        internal static Property Deserialize(
            Class ownerClass,
            BinaryReader reader)
        {
            MethodSignature signature = MethodSignature.Deserialize(reader);
            var isStatic = reader.ReadBoolean();

            var getter = reader.ReadBoolean() ? MethodSignature.Deserialize(reader) : null;
            var setter = reader.ReadBoolean() ? MethodSignature.Deserialize(reader) : null;

            var customAttributes = new List<AttributeInfo>();
            for (int i = reader.ReadInt32() - 1; i >= 0; i--)
            {
                customAttributes.Add(AttributeInfo.Deserialize(reader));
            }

            return new Property(
                ownerClass,
                signature,
                isStatic,
                getter,
                setter,
                customAttributes);
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        #endregion
    }
}
