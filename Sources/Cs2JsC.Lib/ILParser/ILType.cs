using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cs2JsC.Lib.MetaData;

namespace Cs2JsC.Lib.ILParser
{
    public class ILType
    {
        #region member variables
        private TypeAttributes typeAttributes;
        private string name;
        private string extendsClassName;
        private List<string> implements;

        List<ILField> fields = new List<ILField>();
        List<ILMethod> methods = new List<ILMethod>();
        List<string> genericParameters = new List<string>();
        #endregion

        #region constructors/Factories
        private ILType()
        {
        }

        public static ILType ParseFromHeader(
            string header)
        {
            var head = ParseUtils.GetNextWord(header, 0);

            if (head != IlStrings.ScopeNames.ClassType)
            {
                // Note a header for ILType.
                return null;
            }

            var word = head;

            // Read all the attributes.
            //
            var attribute = ILType.GetTypeAttributes(ref word);

            StringFragment classNameFragment;

            var genericFragments = ParseUtils.GetGenericParams(word, out classNameFragment);

            string className = (string)classNameFragment;
            string extendsClass = null;

            word = ParseUtils.GetNextWord(word);

            if (word == "extends")
            {
                word = ParseUtils.GetNextWord(word);
                extendsClass = (string)word;
            }

            List<string> implementsInterfaces = new List<string>();
            if (!StringFragment.IsNull(word))
            {
                word = ParseUtils.GetNextWord(word);

                if (word == "implements")
                {
                    for (word = ParseUtils.GetNextWord(word);
                        !StringFragment.IsNull(word);
                        word = ParseUtils.GetNextWord(word))
                    {
                        if (word[word.Length - 1] == ',')
                        {
                            implementsInterfaces.Add(
                                word.ParentString.Substring(word.StartIndex, word.Length - 1));
                        }
                        else
                        {
                            implementsInterfaces.Add((string)word);
                        }
                    }
                }
            }

            return new ILType()
            {
                typeAttributes = attribute,
                extendsClassName = extendsClass,
                name = className,
                implements = implementsInterfaces,
                genericParameters = genericFragments,
            };
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public TypeAttributes Attributes
        {
            get { return this.typeAttributes; }
        }

        public List<string> GenericParameters
        {
            get { return this.genericParameters; }
        }

        public string Name
        { get { return this.name; } }

        public string ExtendsClassName
        { get { return this.extendsClassName; } }

        public List<ILField> Fields
        { get { return this.fields; } }

        public List<ILMethod> Methods
        { get { return this.methods; } }

        public List<string> Implements
        { get { return this.implements; } }
        #endregion

        #region public functions
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        private static TypeAttributes GetTypeAttributes(ref StringFragment word)
        {
            TypeAttributes attribute = TypeAttributes.None;
            bool previousIsNested = false;
            bool attributesDone = false;

            while (!attributesDone &&
                !Object.Equals(
                    null,
                    (word = ParseUtils.GetNextWord(word))))
            {
                switch ((string)word)
                {
                    case IlStrings.ClassAttributes.Public:
                        if (previousIsNested)
                        {
                            attribute |= TypeAttributes.NestedPublic;
                            previousIsNested = false;
                        }
                        else
                        {
                            attribute |= TypeAttributes.Public;
                        }
                        break;
                    case IlStrings.ClassAttributes.Private:
                        if (previousIsNested)
                        {
                            attribute |= TypeAttributes.NestedPrivate;
                            previousIsNested = false;
                        }
                        else
                        {
                            attribute |= TypeAttributes.Private;
                        }
                        break;
                    case IlStrings.ClassAttributes.Nested:
                        previousIsNested = true;
                        break;
                    case IlStrings.ClassAttributes.Family:
                        attribute |= TypeAttributes.NestedFamily;
                        previousIsNested = false;
                        break;
                    case IlStrings.ClassAttributes.Assembly:
                        attribute |= TypeAttributes.NestedAssembly;
                        previousIsNested = false;
                        break;
                    case IlStrings.ClassAttributes.Famandassem:
                        attribute |= TypeAttributes.NestedFamandassem;
                        previousIsNested = false;
                        break;
                    case IlStrings.ClassAttributes.Famorassem:
                        attribute |= TypeAttributes.NestedFamorassem;
                        previousIsNested = false;
                        break;
                    case IlStrings.ClassAttributes.Value:
                        attribute |= TypeAttributes.Value;
                        break;
                    case IlStrings.ClassAttributes.Enum:
                        attribute |= TypeAttributes.Enum;
                        break;
                    case IlStrings.ClassAttributes.Interface:
                        attribute |= TypeAttributes.Interface;
                        break;
                    case IlStrings.ClassAttributes.Sealed:
                        attribute |= TypeAttributes.Sealed;
                        break;
                    case IlStrings.ClassAttributes.Abstract:
                        attribute |= TypeAttributes.Abstract;
                        break;
                    case IlStrings.ClassAttributes.Auto:
                        attribute |= TypeAttributes.Auto;
                        break;
                    case IlStrings.ClassAttributes.Sequential:
                        attribute |= TypeAttributes.Sequential;
                        break;
                    case IlStrings.ClassAttributes.Explicit:
                        attribute |= TypeAttributes.Explicit;
                        break;
                    case IlStrings.ClassAttributes.Ansi:
                        attribute |= TypeAttributes.Ansi;
                        break;
                    case IlStrings.ClassAttributes.Unicode:
                        attribute |= TypeAttributes.Unicode;
                        break;
                    case IlStrings.ClassAttributes.Autochar:
                        attribute |= TypeAttributes.Autochar;
                        break;
                    case IlStrings.ClassAttributes.Import:
                        attribute |= TypeAttributes.Import;
                        break;
                    case IlStrings.ClassAttributes.Serializable:
                        attribute |= TypeAttributes.Serializable;
                        break;
                    case IlStrings.ClassAttributes.BeforeFieldInit:
                        attribute |= TypeAttributes.BeforeFieldInit;
                        break;
                    case IlStrings.ClassAttributes.SpecialName:
                        attribute |= TypeAttributes.SpecialName;
                        break;
                    case IlStrings.ClassAttributes.RtSpecialName:
                        attribute |= TypeAttributes.RtSpecialName;
                        break;
                    default:
                        attributesDone = true;
                        break;
                }
            }

            return attribute;
        }
        #endregion
    }
}
