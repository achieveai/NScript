using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Cs2JsC.Lib.ILParser
{
    public class SourceCodeBlock
    {
        #region member variables
        static Regex regEx = new Regex(@".line\s+(?<stLine>\d+),(?<endLine>\d+)\s+:\s+(?<stCol>\d+),(?<endCol>\d+)\s+'(?<fileName>[^']*)'");
        #endregion

        #region constructors/Factories
        public SourceCodeBlock(string lineInfo)
        {
            var match = SourceCodeBlock.regEx.Match(lineInfo);
            if (match.Success)
            {
                this.StartPosition =
                    new KeyValuePair<int, int>(
                        int.Parse(match.Groups["stLine"].Value),
                        int.Parse(match.Groups["stCol"].Value));

                this.EndPosition =
                    new KeyValuePair<int, int>(
                        int.Parse(match.Groups["endLine"].Value),
                        int.Parse(match.Groups["endCol"].Value));

                this.FileName = match.Groups["fileName"].Value;
            }
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties

        public KeyValuePair<int, int> StartPosition
        { get; private set; }

        public KeyValuePair<int, int> EndPosition
        { get; private set; }

        public string FileName
        { get; private set; }
        #endregion

        #region public functions
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        #endregion
    }
}
