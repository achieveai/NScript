using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Cs2JsC.Lib.MetaData
{
    public class Class
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public Class(
            Assembly assembly,
            ClassSignature signature,
            TypeAttributes typeAttributes,
            ClassSignature extends,
            List<AttributeInfo> customAttributes)
        {
            this.Assembly = assembly;
            this.Signature = signature;
            this.Attributes = typeAttributes;
            this.Extends = extends;
            this.CustomAttributes = customAttributes;

            this.EventProeprties = new List<EventProperty>();
            this.Properties = new List<Property>();
            this.Methods = new List<Method>();
            this.Fields = new List<Field>();
            this.Implements = new List<ClassSignature>();

            if (this.Extends != null &&
                this.Extends.Assembly != null &&
                this.Extends.Assembly.Name == "sscorlib" &&
                this.Extends.Name == "System.MulticastDelegate")
            {
                this.IsDelegate = true;
            }

            this.Assembly.Add(this);
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public List<AttributeInfo> CustomAttributes
        { get; private set; }

        public List<EventProperty> EventProeprties
        { get; private set; }

        public List<Property> Properties
        { get; private set; }

        public List<Method> Methods
        { get; private set; }

        public ClassSignature Extends
        { get; private set; }

        public List<Field> Fields
        { get; private set; }

        public Assembly Assembly
        { get; private set; }

        public ClassSignature Signature
        { get; private set; }

        public TypeAttributes Attributes
        { get; private set; }

        public List<ClassSignature> Implements
        { get; private set; }

        public bool IsDelegate
        { get; private set; }
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

            writer.Write(this.Extends != null);
            if (this.Extends != null)
            {
                this.Extends.Serialize(writer);
            }

            writer.Write(this.Implements.Count);
            for (int i = 0; i < this.Implements.Count; i++)
            {
                this.Implements[i].Serialize(writer);
            }

            writer.Write(this.Fields.Count);
            for (int i = 0; i < this.Fields.Count; i++)
            {
                this.Fields[i].Serialize(writer);
            }

            writer.Write(this.Properties.Count);
            for (int i = 0; i < this.Properties.Count; i++)
            {
                this.Properties[i].Serialize(writer);
            }

            writer.Write(this.EventProeprties.Count);
            for (int i = 0; i < this.EventProeprties.Count; i++)
            {
                this.EventProeprties[i].Serialize(writer);
            }

            writer.Write(this.Methods.Count);
            for (int i = 0; i < this.Methods.Count; i++)
            {
                this.Methods[i].Serialize(writer);
            }
        }

        public static Class Deserialize(
            Assembly assembly,
            BinaryReader reader)
        {
            var classSignature = ClassSignature.Deserialize(reader);
            TypeAttributes attribute = (TypeAttributes)reader.ReadInt32();

            var customAttributes = new List<AttributeInfo>();
            for (int i = reader.ReadInt32() - 1; i >= 0; i--)
            {
                customAttributes.Add(AttributeInfo.Deserialize(reader));
            }

            ClassSignature extends = null;
            if (reader.ReadBoolean())
            {
                extends = ClassSignature.Deserialize(reader);
            }

            var returnValue = new Class(
                assembly,
                classSignature,
                attribute,
                extends,
                customAttributes);

            for (int i = reader.ReadInt32() - 1; i >= 0; i--)
            {
                returnValue.Implements.Add(ClassSignature.Deserialize(reader));
            }

            for (int i = reader.ReadInt32() - 1; i >= 0; i--)
            {
                Field.Deserialize(returnValue, reader);
            }

            for (int i = reader.ReadInt32() - 1; i >= 0; i--)
            {
                Property.Deserialize(returnValue, reader);
            }

            for (int i = reader.ReadInt32() - 1; i >= 0; i--)
            {
                EventProperty.Deserialize(returnValue, reader);
            }

            for (int i = reader.ReadInt32() - 1; i >= 0; i--)
            {
                Method.Deserialize(returnValue, reader);
            }

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
