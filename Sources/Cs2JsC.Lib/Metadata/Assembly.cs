using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.IO;

namespace Cs2JsC.Lib.MetaData
{
    public class Assembly
    {
        #region member variables
        private readonly List<Class> classes = new List<Class>();
        #endregion

        #region constructors/Factories
        public Assembly(
            AssemblySignature signature,
            bool isExtern,
            List<AttributeInfo> attributes)
        {
            this.Signature = signature;
            this.Classes = new ReadOnlyCollection<Class>(this.classes);
            this.IsExtern = isExtern;
            this.Attributes = attributes;
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public IList<AttributeInfo> Attributes
        { get; private set; }

        public ReadOnlyCollection<Class> Classes
        { get; private set; }

        public AssemblySignature Signature
        { get; private set; }

        public bool IsExtern { get; private set; }
        #endregion

        #region public functions
        internal void Add(Class type)
        {
            this.classes.Add(type);
        }

        public Class GetType(string typeName)
        {
            return null;
        }

        public void Serialize(BinaryWriter writer)
        {
            this.Signature.Serialize(writer);
            writer.Write(this.Attributes.Count);
            for (int i = 0; i < this.Attributes.Count; i++)
            {
                this.Attributes[i].Serialize(writer);
            }

            writer.Write(this.Classes.Count);
            for (int i = 0; i < this.Classes.Count; i++)
            {
                this.Classes[i].Serialize(writer);
            }
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        #endregion
    }
}
