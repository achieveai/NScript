using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Cs2JsC.Lib.MetaData
{
    public class AssemblySignature
    {
        #region member variables
        #endregion

        #region constructors/Factories
        public AssemblySignature(string assemblyName)
        {
            this.Name = assemblyName;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public string Name { get; private set; }
        #endregion

        #region public functions
        public override bool Equals(object obj)
        {
            AssemblySignature right = obj as AssemblySignature;
            return right != null && right.Name == this.Name;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }

        public static bool operator ==(AssemblySignature left, AssemblySignature right)
        {
            return object.Equals(left, right);
        }

        public static bool operator !=(AssemblySignature left, AssemblySignature right)
        {
            return !object.Equals(left, right);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(this.Name);
        }

        public static AssemblySignature Deserialize(BinaryReader reader)
        {
            string name = reader.ReadString();
            return new AssemblySignature(name);
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        #endregion
    }
}
