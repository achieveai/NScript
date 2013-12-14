//-----------------------------------------------------------------------
// <copyright file="MediaQuery.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace CssParser
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for MediaQuery
    /// </summary>
    public class MediaQuery
    {
        public MediaQuery(
            IList<MediaRule> mediaRules,
            bool isNot = false)
        {
            this.MediaRules = mediaRules;
            this.IsNot = isNot;
        }

        public IList<MediaRule> MediaRules { get; set; }

        public bool IsNot { get; set; }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (this.IsNot)
            {
                sb.Append("not ");
            }

            for (int iRule = 0; iRule < this.MediaRules.Count; iRule++)
            {
                var rule = this.MediaRules[iRule];
                if (iRule > 0)
                {
                    sb.Append(" and ");
                }

                sb.Append(rule);
            }

            return sb.ToString();
        }
    }
}
