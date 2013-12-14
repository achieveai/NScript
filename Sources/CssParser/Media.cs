//-----------------------------------------------------------------------
// <copyright file="Media.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace CssParser
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for Media
    /// </summary>
    public class Media
    {
        public Media(
            IList<MediaQuery> mediaQueries,
            IList<CssRule> ruleSet,
            IList<CssKeyframes> keyFrames)
        {
            this.MediaQueires = mediaQueries;
            this.RuleSet = ruleSet;
            this.KeyFrames = keyFrames;
        }

        public IList<CssRule> RuleSet { get; set; }

        public IList<MediaQuery> MediaQueires { get; set; }

        public IList<CssKeyframes> KeyFrames { get; set; }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("@media ");
            for (int iQuery = 0; iQuery < this.MediaQueires.Count; iQuery++)
            {
                if (iQuery > 0)
                {
                    sb.Append(", ");
                }

                sb.Append(this.MediaQueires[iQuery]);
            }

            sb.Append('{');
            for (int iCssRule = 0; iCssRule < this.RuleSet.Count; iCssRule++)
            {
                sb.Append(this.RuleSet[iCssRule]);
            }

            sb.Append('}');
            return sb.ToString();
        }
    }
}
