//-----------------------------------------------------------------------
// <copyright file="VisualTransition.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html.Filters
{
    using System.Runtime.CompilerServices;

    public enum VisualTransitionState
    {
        Stopped,
        Applied,
        Playing
    }

    /// <summary>
    /// Definition for VisualTransition
    /// </summary>
    [IgnoreNamespace]
    public sealed class VisualTransition : VisualFilter
    {
        private extern VisualTransition();

        public extern double Duration { get; set; }

        public extern int Percent { get; }

        public VisualTransitionState Status
        {
            get
            {
                switch (this.status)
                {
                    default:
                    case "stopped":
                        return VisualTransitionState.Stopped;
                    case "applied":
                        return VisualTransitionState.Applied;
                    case "playing":
                        return VisualTransitionState.Playing;
                }
            }
        }

        public extern void Apply();

        public extern void Play();

        public extern void Stop();

        private extern string status { get; }
    }
}