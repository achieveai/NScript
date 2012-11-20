using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NScript.Lib.MetaData
{
    public class EventProperty : MetadataBase
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public EventProperty(
            Class owner,
            EventPropertySignature signature,
            bool isStatic,
            ClassSignature eventType,
            MethodSignature addMethod,
            MethodSignature removeMethod,
            List<AttributeInfo> customAttributes)
        {
            this.OwnerClass = owner;
            this.Signature = signature;
            this.IsStatic = isStatic;
            this.DelegateClass = eventType;
            this.AddEventMethod = addMethod;
            this.RemoveEventMethod = removeMethod;
            this.CustomAttributes = customAttributes;

            this.OwnerClass.EventProeprties.Add(this);
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public IList<AttributeInfo> CustomAttributes
        { get; set; }

        public MethodSignature AddEventMethod
        { get; private set; }

        public MethodSignature RemoveEventMethod
        { get; private set; }

        public ClassSignature DelegateClass
        { get; private set; }

        public Class OwnerClass
        { get; private set; }

        public EventPropertySignature Signature
        { get; private set; }

        public bool IsStatic
        { get; private set; }
        #endregion

        #region public functions
        public void Serialize(System.IO.BinaryWriter writer)
        {
            this.Signature.Serialize(writer);
            writer.Write(this.IsStatic);
            this.DelegateClass.Serialize(writer);

            writer.Write(this.AddEventMethod != null);
            if (this.AddEventMethod != null)
            {
                this.AddEventMethod.Serialize(writer);
            }

            writer.Write(this.RemoveEventMethod != null);
            if (this.RemoveEventMethod != null)
            {
                this.RemoveEventMethod.Serialize(writer);
            }

            writer.Write(this.CustomAttributes.Count);
            for (int i = 0; i < this.CustomAttributes.Count; i++)
            {
                this.CustomAttributes[i].Serialize(writer);
            }
        }

        public static EventProperty Deserialize(
            Class ownerClass,
            BinaryReader reader)
        {
            EventPropertySignature signature = EventPropertySignature.Deserialize(reader);
            var isStatic = reader.ReadBoolean();
            var delegateClass = ClassSignature.Deserialize(reader);

            var addMethod = reader.ReadBoolean() ? MethodSignature.Deserialize(reader) : null;
            var removeMethod = reader.ReadBoolean() ? MethodSignature.Deserialize(reader) : null;

            var customAttributes = new List<AttributeInfo>();
            for (int i = reader.ReadInt32() - 1; i >= 0; i--)
            {
                customAttributes.Add(AttributeInfo.Deserialize(reader));
            }

            return new EventProperty(
                ownerClass,
                signature,
                isStatic,
                delegateClass,
                addMethod,
                removeMethod,
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
