using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NScript.Lib.MetaData
{
    public class AttributeInfo
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public AttributeInfo(
            MethodSignature attributeConstructor,
            byte[] attributeConstructorData)
        {
            this.AttributeConstructor = attributeConstructor;
            this.AttributeData = attributeConstructorData;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public MethodSignature AttributeConstructor
        { get; private set; }

        public byte[] AttributeData
        { get; private set; }
        #endregion

        #region public functions
        public void Serialize(BinaryWriter writer)
        {
            this.AttributeConstructor.Serialize(writer);
            writer.Write(this.AttributeData.Length);
            for (int i = 0; i < this.AttributeData.Length; i++)
            {
                writer.Write(this.AttributeData[i]);
            }
        }

        public static AttributeInfo Deserialize(BinaryReader reader)
        {
            var constructor = MethodSignature.Deserialize(reader);
            byte[] data = new byte[reader.ReadInt32()];

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = reader.ReadByte();
            }

            return new AttributeInfo(
                constructor,
                data);
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        #endregion

    }
}
