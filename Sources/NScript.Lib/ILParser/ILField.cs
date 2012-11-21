using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NScript.Lib.MetaData;

namespace NScript.Lib.ILParser
{

    public class ILField
    {
        #region member variables
        #endregion

        #region constructors/Factories
        private ILField()
        { }

        public static ILField ParseFromHeader(
            string header)
        {
            var head = ParseUtils.GetNextWord(header, 0);

            if (head != IlStrings.ScopeNames.FieldType)
            {
                // Note a header for ILType.
                return null;
            }

            var word = head;

            // Read all the attributes.
            //
            FieldAttributes attributes = ILField.GetAttributes(ref word);
            string fieldType = ILMethod.GetReturnType(ref word);
            string fieldId = (string)word;

            string initValue = null;
            if ((word = ParseUtils.GetNextWord(word)) == "=")
            {
                initValue = (string)ParseUtils.GetNextWord(word);
            }

            return new ILField()
            {
                Attributes = attributes,
                Type = fieldType,
                Id = fieldId,
                InitValue = initValue
            };
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public FieldAttributes Attributes
        { get; private set; }

        public string Type
        { get; private set; }

        public string Id
        { get; private set; }

        public string InitValue
        { get; private set; }
        #endregion

        #region public functions
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        private static FieldAttributes GetAttributes(ref StringFragment word)
        {
            FieldAttributes attributes = FieldAttributes.None;

            bool attributesDone = false;
            while (!attributesDone &&
                !Object.Equals(
                    null,
                    (word = ParseUtils.GetNextWord(word))))
            {
                switch ((string)word)
                {
                    case IlStrings.FieldAttributes.Public:
                        attributes |= FieldAttributes.Public;
                        break;
                    case IlStrings.FieldAttributes.Private:
                        attributes |= FieldAttributes.Private;
                        break;
                    case IlStrings.FieldAttributes.Family:
                        attributes |= FieldAttributes.Family;
                        break;
                    case IlStrings.FieldAttributes.Assembly:
                        attributes |= FieldAttributes.Assembly;
                        break;
                    case IlStrings.FieldAttributes.Famandassem:
                        attributes |= FieldAttributes.Famandassem;
                        break;
                    case IlStrings.FieldAttributes.Famorassem:
                        attributes |= FieldAttributes.Famorassem;
                        break;
                    case IlStrings.FieldAttributes.PrivateScope:
                        attributes |= FieldAttributes.PrivateScope;
                        break;
                    case IlStrings.FieldAttributes.Static:
                        attributes |= FieldAttributes.Static;
                        break;
                    case IlStrings.FieldAttributes.Initonly:
                        attributes |= FieldAttributes.Initonly;
                        break;
                    case IlStrings.FieldAttributes.RtSpecialName:
                        attributes |= FieldAttributes.RtSpecialName;
                        break;
                    case IlStrings.FieldAttributes.SpecialName:
                        attributes |= FieldAttributes.SpecialName;
                        break;
                    case IlStrings.FieldAttributes.Literal:
                        attributes |= FieldAttributes.Literal;
                        break;
                    case IlStrings.FieldAttributes.NotSerialized:
                        attributes |= FieldAttributes.NotSerialized;
                        break;
                    default:
                        attributesDone = true;
                        break;
                }
            }

            return attributes;
        }
        #endregion
    }
}
