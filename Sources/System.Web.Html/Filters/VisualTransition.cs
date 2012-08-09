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
    [Imported]
    [IgnoreNamespace]
    public sealed class VisualTransition : VisualFilter
    {
        private VisualTransition() { }

        [IntrinsicField]
        public double Duration;

        [IntrinsicField]
        public readonly int Percent;

        [IntrinsicField]
        private readonly string status;

        [MakeStaticUsage]
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
    }
}