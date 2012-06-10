//-----------------------------------------------------------------------
// <copyright file="AudioElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for AudioElement
    /// </summary>
    [Imported]
    [IgnoreNamespace]
    public sealed class AudioElement
    {
        private AudioElement() { }

        [IntrinsicField]
        public double CurrentTime;

        [IntrinsicField]
        public readonly double Duration;

        [IntrinsicField]
        public readonly bool Ended;

        [IntrinsicField]
        public readonly bool Paused;

        [IntrinsicField]
        public string Src;

        [IntrinsicField]
        public float Volume;

        public extern void Load();

        public extern void Pause();

        public extern void Play();
    }
}
