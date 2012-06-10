using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Cs2JsC.Lib.MetaData
{
    public class ClassSignature
    {
        #region constructors/Factories
        public ClassSignature(
            string className,
            AssemblySignature assemblySignature,
            int genericParameterCount)
        {
            this.Name = className;

            this.GenericParameterCount = genericParameterCount;

            // GenericTypeNames don't really have assembly associated with them
            //
            if (assemblySignature == null &&
                !className.StartsWith("!"))
            {
                switch (this.Name)
                {
                    case "string":
                    case "int32":
                    case "int64":
                    case "int16":
                    case "byte":
                    case "float":
                    case "double":
                        assemblySignature = new AssemblySignature("sscorlib");
                        break;
                    default:
                        assemblySignature = NameResolver.Instance.CurrentAssembly;
                        break;
                }
            }
            this.Assembly = assemblySignature;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public AssemblySignature Assembly { get; private set; }

        public string Name { get; private set; }

        public int GenericParameterCount
        { get; private set; }
        #endregion

        #region public functions
        public override bool Equals(object obj)
        {
            ClassSignature rightObj = obj as ClassSignature;
            if (rightObj != null && this.Name == rightObj.Name && this.GenericParameterCount == rightObj.GenericParameterCount)
            {
                if (this.Assembly == null)
                {
                    return rightObj.Assembly == null;
                }

                return this.Assembly.Equals(rightObj.Assembly);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode() + this.GenericParameterCount;
        }

        public override string ToString()
        {
            if (this.GenericParameterCount == 0)
            {
                return string.Format(
                    "{0}{1}",
                    this.Assembly != null ? string.Format("[{0}]", this.Assembly) : string.Empty,
                    this.Name);
            }
            else
            {
                return string.Format(
                    "{0}{1}`{2}",
                    this.Assembly != null ? string.Format("[{0}]", this.Assembly) : string.Empty,
                    this.Name,
                    this.GenericParameterCount);
            }
        }

        public static bool operator ==(ClassSignature left, ClassSignature right)
        {
            return object.Equals(left, right);
        }

        public static bool operator !=(ClassSignature left, ClassSignature right)
        {
            return !object.Equals(left, right);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(this.Name);
            writer.Write(this.Assembly != null);
            if (this.Assembly != null)
            {
                this.Assembly.Serialize(writer);
            }
            writer.Write(this.GenericParameterCount);
        }

        public static ClassSignature Deserialize(BinaryReader reader)
        {
            return new ClassSignature(
                reader.ReadString(),
                reader.ReadBoolean() ? AssemblySignature.Deserialize(reader) : null,
                reader.ReadInt32());
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        #endregion
    }
}
