//-----------------------------------------------------------------------
// <copyright file="CssKeyFrames.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace CssParser
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Definition for CssKeyFrames
    /// </summary>
    public class CssKeyframes
    {
        public CssKeyframes(string name, IList<CssKeyframe> frames)
        {
            this.Name = name;
            this.Frames = frames;
        }

        public IList<CssKeyframe> Frames { get; private set; }

        public string Name { get; private set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("@keyframes ");
            sb.Append(this.Name);
            for (int iFrame = 0; iFrame < this.Frames.Count; iFrame++)
            {
                sb.Append(this.Frames[iFrame].ToString());
            }

            return sb.ToString();
        }
    }
}
