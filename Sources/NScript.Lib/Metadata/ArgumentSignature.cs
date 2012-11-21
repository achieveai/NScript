using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NScript.Lib.MetaData
{
    public class ArgumentSignature
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public ArgumentSignature(
            string type,
            string name,
            int index,
            bool isRef,
            bool isOut)
        {
            this.Type = type;
            this.Name = name;
            this.Index = index;
            this.IsRefArgument = isRef;
            this.IsOutArgument = isOut;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public bool IsOutArgument { get; private set; }

        public bool IsRefArgument { get; private set; }

        public int Index { get; private set; }

        public string Name { get; private set; }

        public string Type { get; private set; }
        #endregion

        #region public functions
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(this.Type);
            writer.Write(this.Name != null);
            if (this.Name != null)
            {
                writer.Write(this.Name);
            }
            writer.Write(this.Index);
            writer.Write(this.IsRefArgument);
            writer.Write(this.IsOutArgument);
        }

        public static ArgumentSignature Deserialize(BinaryReader reader)
        {
            return new ArgumentSignature(
                reader.ReadString(),
                reader.ReadBoolean() ? reader.ReadString() : null,
                reader.ReadInt32(),
                reader.ReadBoolean(),
                reader.ReadBoolean());
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        #endregion
    }
}
