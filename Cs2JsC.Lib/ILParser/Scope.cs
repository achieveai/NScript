using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cs2JsC.Lib.ILParser
{
    public enum LineType
    {
        Blank,
        Comment,
        BeginScope,
        EndScope,
        ScopeHeader,
        Unknown,
        CustomAttribute
    }

    public enum ScopeType
    {
        Unknown,
        Assembly,
        Class,
        Field,
        Method,
        Property,
        CustomAttributeScope,
        Custom,
    }

    public class Scope
    {
        #region member variables
        string header = null;
        private readonly List<string> scopedLines = new List<string>();
        private readonly List<Scope> nestedScopes = new List<Scope>();
        private readonly List<string> customAttributeLines = new List<string>();
        #endregion

        #region constructors/Factories
        private Scope()
        {
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public string Header
        {
            get { return this.header; }
        }

        public List<string> CustomAttributes
        { get { return this.customAttributeLines; } }

        public List<string> ScopedLines
        {
            get { return this.scopedLines; }
        }

        public List<Scope> NestedScopes
        {
            get { return this.nestedScopes; }
        }

        public ScopeType ScopeType
        {
            get;
            private set;
        }
        #endregion

        #region public functions
        public static Scope CreateScope(IList<string> strings, ref int startIndex)
        {
            Scope returnValue = null;
            int nestedScopes = 0;
            int scopeStartIndex = 0;

            for (int iIndex = startIndex; iIndex < strings.Count; iIndex++)
            {
                switch (Scope.GetLineType(strings[iIndex]))
                {
                    case LineType.Blank:
                    case LineType.Comment:
                        if (nestedScopes == 0)
                        {
                            startIndex = iIndex + 1;
                            return returnValue;
                        }
                        break;
                    case LineType.BeginScope:
                        if (nestedScopes == 0)
                        {
                            scopeStartIndex = iIndex;
                        }
                        nestedScopes++;
                        break;
                    case LineType.EndScope:
                        nestedScopes--;
                        if (nestedScopes == 0)
                        {
                            for (int i = scopeStartIndex + 1; i < iIndex; i++)
                            {
                                returnValue.scopedLines.Add(strings[i]);
                            }

                            startIndex = iIndex + 1;
                            returnValue.Initialize();
                            return returnValue;
                        }
                        break;
                    case LineType.ScopeHeader:
                        if (returnValue == null)
                        {
                            returnValue = new Scope();
                            returnValue.header = strings[iIndex];
                        }
                        else if (nestedScopes == 0)
                        {
                            startIndex = iIndex;

                            if (returnValue != null)
                            {
                                returnValue.Initialize();
                            }
                            return returnValue;
                        }
                        break;
                    case LineType.CustomAttribute:
                        if (nestedScopes == 0 &&
                            returnValue != null)
                        {
                            int attributesStartIndex = iIndex;
                            startIndex = AttributeParser.GetAttributesLastLineIndex(
                                strings,
                                iIndex);

                            for (int i = attributesStartIndex; i < iIndex; i++)
                            {
                                returnValue.scopedLines.Add(strings[i]);
                            }
                            
                            returnValue.Initialize();
                            return returnValue;
                        }
                        break;
                    case LineType.Unknown:
                        if (returnValue != null &&
                            nestedScopes == 0)
                        {
                            returnValue.header = string.Format("{0} {1}", returnValue.header, strings[iIndex].Trim());
                        }
                        break;
                    default:
                        break;
                }
            }

            startIndex = strings.Count;

            if (returnValue != null)
            {
                returnValue.Initialize();
            }
            return returnValue;
        }

        public static List<Scope> CreateScopes(IList<string> strings)
        {
            int startIndex = 0;
            List<Scope> returnValue = new List<Scope>();

            while (startIndex < strings.Count)
            {
                Scope scopeInfo = Scope.CreateScope(
                    strings,
                    ref startIndex);

                if (scopeInfo != null)
                {
                    returnValue.Add(scopeInfo);
                }
            }

            return returnValue;
        }
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        private void Initialize()
        {
            if (this.scopedLines.Count > 0)
            {
                nestedScopes.AddRange(
                    Scope.CreateScopes(
                        this.scopedLines));
            }

            this.InitializeScopeType();
        }

        private void InitializeScopeType()
        {
            string scopeType = (string)ParseUtils.GetNextWord(header, 0);

            switch (scopeType)
            {
                case IlStrings.ScopeNames.ClassType:
                    this.ScopeType = ScopeType.Class;
                    break;
                case IlStrings.ScopeNames.MethodType:
                    this.ScopeType = ScopeType.Method;
                    break;
                case IlStrings.ScopeNames.AssemblyType:
                    this.ScopeType = ScopeType.Assembly;
                    break;
                case IlStrings.ScopeNames.FieldType:
                    this.ScopeType = ScopeType.Field;
                    break;
                case IlStrings.ScopeNames.PropertyType:
                    this.ScopeType = ScopeType.Property;
                    break;
                case IlStrings.ScopeNames.CustomType:
                    this.ScopeType = ScopeType.Custom;
                    break;
                default:
                    this.ScopeType = ScopeType.Unknown;
                    break;
            }

        }

        private static LineType GetLineType(string line)
        {
            var word = ParseUtils.GetNextWord(line, 0);

            if (!StringFragment.IsNull(word) &&
                word.Length != 0)
            {
                switch (word[0])
                {
                    case '/':
                        return LineType.Comment;
                    case '.':
                        if (word == ".custom")
                        {
                            return LineType.CustomAttribute;
                        }
                        else
                        {
                            return LineType.ScopeHeader;
                        }
                    case '}':
                        return LineType.EndScope;
                    case '{':
                        return LineType.BeginScope;
                    default:
                        return LineType.Unknown;
                }
            }

            return LineType.Blank;
        }
        #endregion
    }
}
