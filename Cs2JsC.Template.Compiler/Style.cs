using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Cs2JsC.Template.Compiler
{
    public class StyleMapping
    {
        private static int nextStyleId = 1;
        private readonly List<string> styleNames = new List<string>();
        private readonly Dictionary<string, int> styleMappings = new Dictionary<string,int>();
        private readonly Dictionary<string, int> animationMappings = new Dictionary<string,int>();
        private readonly string templateName;
        private List<string> globalStyleNames;

        public StyleMapping(string templateFileName, List<string> globalStyleNames)
        {
            this.templateName = templateFileName.Replace(' ', '1').Replace('.', '2');
            this.globalStyleNames = globalStyleNames;            
        }

        public string GetStyleId(string styleClass)
        {
            if (this.globalStyleNames.Contains(styleClass))
            {
                return styleClass;
            }

            return this.GetId(styleClass, this.styleMappings);
        }

        public string GetAnimationId(string animationId)
        {
            return this.GetId(animationId, this.animationMappings);
        }

        public bool ContainsStyle(string styleName)
        {
            return this.styleMappings.ContainsKey(styleName);
        }

        public string GetUniqueName(int id)
        {
            return string.Format("_uq{0}", id);
        }

        public static void Reset()
        {
            StyleMapping.nextStyleId = 1;
        }

        private string GetId(string id, Dictionary<string, int> map)
        {
            int returnValue;
            if (!map.TryGetValue(id, out returnValue))
            {
                returnValue = System.Threading.Interlocked.Increment(ref StyleMapping.nextStyleId);
                map.Add(id, returnValue);
            }

            return TypeManager.GetIdString("_" + id + "_" + this.templateName + "_", returnValue);
        }
    }
    
    public class Style
    {
        const string classNameGroup = "className";
        const string propertyNameGroup = "propertyName";
        const string propertyValueGroup = "propertyValue";
        const string keyframesBlockGroup = "keyframesBlock";
        const string keyframesNameGroup = "keyframesName";
        const string webkitKeyframesKeyword = "@-webkit-keyframes";
        const string webkitAnimiationNameKeyword = "-webkit-animation-name";
        const string bodyGroup = "body";
        const string headGroup = "head";
        const string cssNameRegexStr = @"\.(?<className>[a-zA-Z0-9][a-zA-Z0-9-_]*)";
        const string cssPropertyRegexStr = @"(?<propertyName>[a-zA-Z-][a-zA-Z0-9-]*)\s*:\s*(?<propertyValue>[^;]*);";
        const string cssBodyRegexStr = @"(?<body>\{\s*(" + cssPropertyRegexStr + @"\s*)*\})";
        const string cssHeadRegexStr = @"(?<head>[^\{]+)";
        const string cssSingleStyleBlockRegexStr = @"(?<cssBlock>" + cssHeadRegexStr + @"\s*" + cssBodyRegexStr + @")";
        const string cssKeyframesNameRegexStr = @"(?<keyframesName>[a-zA-Z0-9][a-zA-Z0-9-_]*)";
        const string cssKeyframesRegexStr = @"(?<keyframesBlock>\s*" + webkitKeyframesKeyword + @"\s+" + cssKeyframesNameRegexStr + @"\s*\{" + cssSingleStyleBlockRegexStr + @"+\s*\})"; 
        const string cssBlockRegexStr = @"(" + cssKeyframesRegexStr + @"|" + cssSingleStyleBlockRegexStr + @")";
        const string removeCommentsRegexStr = @"/\*.*?\*/";        
        
        static readonly Regex cssBlockParser =
            new Regex(
                @"\G" + cssBlockRegexStr,
                RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
        static readonly Regex cssSingleStyleBlockParser =
            new Regex(
                cssSingleStyleBlockRegexStr,
                RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnoreCase);

        static readonly Regex cssNameParser =
            new Regex(
                cssNameRegexStr,
                RegexOptions.Compiled);

        static readonly Regex cssPropertyParser =
            new Regex(
                cssPropertyRegexStr,
                RegexOptions.Multiline | RegexOptions.Compiled);

        static readonly Regex removeCommentsRegEx =
            new Regex(
                removeCommentsRegexStr,
                RegexOptions.Compiled | RegexOptions.Singleline);
                
        private List<SingleStyle> singleStyles = new List<SingleStyle>();
        private List<KeyframesBlock> keyframesBlocks = new List<KeyframesBlock>();

        private Style(List<SingleStyle> singleStyles, List<KeyframesBlock> keyframesBlocks)
        {
            this.singleStyles = singleStyles;
            this.keyframesBlocks = keyframesBlocks;
        }

        public static Style ParseStyles(
            string styleBlob,
            StyleMapping mapping)
        {
            // Remove /* */comments
            styleBlob = removeCommentsRegEx.Replace(styleBlob, String.Empty);
            
            MatchCollection matches = Style.cssBlockParser.Matches(styleBlob);
            
            List<SingleStyle> singleStyles = new List<SingleStyle>();
            List<KeyframesBlock> keyframesBlocks = new List<KeyframesBlock>();

            for (int iMatch = 0; iMatch < matches.Count; iMatch++)
            {
                Match match = matches[iMatch];

                Group keyframesBlockMatchGroup = match.Groups[Style.keyframesBlockGroup];
                if (keyframesBlockMatchGroup != null && keyframesBlockMatchGroup.Success)
                {
                    // matches a @-webkit-keyframes block
                    string keyframesString = keyframesBlockMatchGroup.Value;

                    MatchCollection styleMatches = Style.cssSingleStyleBlockParser.Matches(keyframesString);
                    List<SingleStyle> keyframes = new List<SingleStyle>();
                    for (int i = 0; i < styleMatches.Count; i++)
                    {
                        keyframes.Add(Style.CreateSingleStyle(styleMatches[i], mapping));
                    }

                    string keyframeName = match.Groups[Style.keyframesNameGroup].Value;
                    keyframeName = mapping.GetAnimationId(keyframeName);

                    keyframesBlocks.Add(new KeyframesBlock(keyframeName, keyframes));
                }
                else
                {
                    // matches a regular style
                    singleStyles.Add(Style.CreateSingleStyle(match, mapping));
                }
            }

            return new Style(singleStyles, keyframesBlocks);
        }
        
        /// <summary>
        /// Writes the CSS.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void WriteCss(System.IO.StreamWriter writer)
        {
            for (int styleIndex = 0; styleIndex < this.keyframesBlocks.Count; styleIndex++)
            {
                KeyframesBlock block = this.keyframesBlocks[styleIndex];

                writer.Write(Style.webkitKeyframesKeyword);
                writer.Write(" ");
                writer.Write(block.Name);
                writer.WriteLine(" {");

                for (int j = 0; j < block.Keyframes.Count; j++)
                {
                    Style.WriteSingleStyle(block.Keyframes[j], writer);
                }

                writer.WriteLine("}");
                writer.WriteLine();
            }

            for (int styleIndex = 0; styleIndex < this.singleStyles.Count; styleIndex++)
            {
                Style.WriteSingleStyle(this.singleStyles[styleIndex], writer);
            }
        }

        /// <summary>
        /// Creates a SingleStyle object
        /// </summary>
        /// <param name="match">A Match using the Regex Style.cssSingleStyleBlockParser</param>
        /// <returns>a new SingleStyle object</returns>
        private static SingleStyle CreateSingleStyle(Match match, StyleMapping mapping)
        {
            Group headStr = match.Groups[Style.headGroup];
            Group bodyStr = match.Groups[Style.bodyGroup];

            List<string> classIds = new List<string>();
            Dictionary<string, string> styleProperties = new Dictionary<string, string>();
                
            string[] classSections = headStr.Value.Split(',');

            for (int i = 0; i < classSections.Length; i++)
            {
                string classSection = classSections[i].Trim();

                classSection = Style.cssNameParser.Replace(classSection, new MatchEvaluator(
                    delegate (Match thisMatch) 
                    {
                        string str = thisMatch.ToString();
                        return "." + mapping.GetStyleId(str.Substring(1));
                    }
                ));

                classIds.Add(classSection);
            }
                
            MatchCollection propertyMatches = Style.cssPropertyParser.Matches(bodyStr.Value);
            for (int iProperty = 0; iProperty < propertyMatches.Count; iProperty++)
            {
                string name = propertyMatches[iProperty].Groups[Style.propertyNameGroup].Value;
                string value = propertyMatches[iProperty].Groups[Style.propertyValueGroup].Value;

                if (Style.webkitAnimiationNameKeyword.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    // If this property is an animation name, get the name from the mapping.
                    value = mapping.GetAnimationId(value);
                }

                styleProperties.Add(name, value);
            }

            return new SingleStyle(classIds, styleProperties);
        }
        
        /// <summary>
        /// Writes the CSS.
        /// </summary>
        /// <param name="style">The SingleStyle to be written out.</param>
        /// <param name="writer">The writer.</param>
        private static void WriteSingleStyle(SingleStyle style, System.IO.StreamWriter writer)
        {
            List<string> styleNames = style.StyleNameIds;
            for (int nameIndex = 0; nameIndex < styleNames.Count; nameIndex++)
            {
                if (nameIndex > 0)
                {
                    writer.Write(", ");
                }

                writer.Write(styleNames[nameIndex]);
            }

            writer.WriteLine(" {");

            Dictionary<string, string> styleProperties = style.StyleProperties;

            foreach (var stylePropertyValue in style.StyleProperties)
            {
                writer.Write("  ");
                writer.Write(stylePropertyValue.Key);
                writer.Write(":");
                writer.Write(stylePropertyValue.Value);
                writer.WriteLine(";");
            }

            writer.WriteLine("}");
            writer.WriteLine();
        }        
        
        /// <summary>
        /// Represents a single CSS style declaration in the form 
        /// name {
        ///   propertyName:propertyValue;
        ///   propertyName:propertyValue;
        /// }
        /// </summary>
        private class SingleStyle
        {        
            List<string> styleNames;
            Dictionary<string, string> styleProperties;

            public SingleStyle(List<string> styleNames, Dictionary<string, string> styleProperties)
            {
                this.styleNames = styleNames;
                this.styleProperties = styleProperties;
            }

            public List<string> StyleNameIds
            {
                get { return this.styleNames; }
            }

            public Dictionary<string, string> StyleProperties
            {
                get { return this.styleProperties; }
            }
        }
        
        /// <summary>
        /// Represents a @-webkit-keyframes declaration.
        /// </summary>
        private class KeyframesBlock
        {
            string name;
            List<SingleStyle> keyframes;

            public KeyframesBlock(string name, List<SingleStyle> keyframes)
            {
                this.name = name;
                this.keyframes = keyframes;
            }

            public string Name
            {
                get { return this.name; }
            }

            public List<SingleStyle> Keyframes
            {
                get { return this.keyframes; }
            }
        }
    }
}

