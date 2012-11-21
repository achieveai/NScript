using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using NScript.Lib.ILParser;
using System.IO;

namespace NScript.Lib.MetaData
{
    public class MethodSignature : MemberSignature
    {
        #region member variables
        List<ArgumentSignature> argumentSignature = new List<ArgumentSignature>();
        #endregion

        #region constructors/Factories
        public MethodSignature(
            string name,
            ClassSignature typeSignature,
            bool isStatic,
            List<ArgumentSignature> signature,
            ClassSignature classSignature)
            : base(name, typeSignature, classSignature)
        {
            this.IsStatic = isStatic;
            this.argumentSignature.AddRange(signature);
            this.Arguments = new ReadOnlyCollection<ArgumentSignature>(this.argumentSignature);
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public bool IsConstructor
        { get { return this.Name == ".ctor"; } }
        public ReadOnlyCollection<ArgumentSignature> Arguments
        { get; private set; }
        #endregion

        #region public functions
        public override bool Equals(object obj)
        {
            return obj is MethodSignature && base.Equals(obj) && this.DoesArgumentSignatureMatch((MethodSignature)obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + this.Arguments.Count;
        }

        public override string ToString()
        {
            return string.Format("{0}::{1}({2})",
                this.Class,
                this.Name,
                this.GetArgumentString());
        }

        public string GetArgumentId(int argumentIndex)
        {
            if (!this.IsStatic)
            {
                if (argumentIndex == 0)
                {
                    return "this";
                }
                else
                {
                    argumentIndex--;
                }
            }

            return this.Arguments[argumentIndex].Name;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(this.Name);
            writer.Write(this.Type != null);
            if (this.Type != null)
            {
                this.Type.Serialize(writer);
            }
            writer.Write(this.IsStatic);

            writer.Write(this.Arguments.Count);
            for (int i = 0; i < this.Arguments.Count; i++)
            {
                this.Arguments[i].Serialize(writer);
            }
            this.Class.Serialize(writer);
        }

        public static MethodSignature Deserialize(BinaryReader reader)
        {
            var name = reader.ReadString();
            ClassSignature returnType = reader.ReadBoolean() ? ClassSignature.Deserialize(reader) : null;
            var isStatic = reader.ReadBoolean();
            List<ArgumentSignature> arguments = new List<ArgumentSignature>();
            for (int i = reader.ReadInt32() - 1; i >= 0; i--)
            {
                arguments[i] = ArgumentSignature.Deserialize(reader);
            }
            var ownerClass = ClassSignature.Deserialize(reader);

            return new MethodSignature(
                name,
                returnType,
                isStatic,
                arguments,
                ownerClass);
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        private bool DoesArgumentSignatureMatch(MethodSignature right)
        {
            if (this.Arguments.Count != right.Arguments.Count)
            {
                return false;
            }

            for (int i = 0; i < this.Arguments.Count; i++)
            {
                if (this.Arguments[i].Type != right.Arguments[i].Type ||
                    (this.Arguments[i].IsOutArgument != right.Arguments[i].IsOutArgument) ||
                    (this.Arguments[i].IsRefArgument != right.Arguments[i].IsRefArgument))
                {
                    return false;
                }
            }

            return true;
        }

        private string GetArgumentString()
        {
            StringBuilder strBuilder = new StringBuilder();

            for (int i = 0; i < this.Arguments.Count; i++)
            {
                if (i > 0)
                {
                    strBuilder.Append(", ");
                }

                if (this.Arguments[i].IsRefArgument)
                {
                    strBuilder.Append("ref ");
                }
                else if (this.Arguments[i].IsOutArgument)
                {
                    strBuilder.Append("out ");
                }

                strBuilder.Append(this.Arguments[i].Type);

                if (!string.IsNullOrEmpty(this.Arguments[i].Name))
                {
                    strBuilder.AppendFormat(" {0}", this.Arguments[i].Name);
                }
            }

            return strBuilder.ToString();
        }
        #endregion
    }
}
