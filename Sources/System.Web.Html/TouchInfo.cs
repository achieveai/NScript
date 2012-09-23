//-----------------------------------------------------------------------
// <copyright file="TouchInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for TouchInfo
    /// </summary>
    [PsudoType]
    [IgnoreNamespace]
    public sealed class TouchInfo
    {
        private TouchInfo() { }

        [IntrinsicField]
        public readonly int ClientX;

        [IntrinsicField]
        public readonly int ClientY;

        [IntrinsicField]
        public readonly int Identifier;

        [IntrinsicField]
        public readonly int PageX;

        [IntrinsicField]
        public readonly int PageY;

        [IntrinsicField]
        public readonly int ScreenX;

        [IntrinsicField]
        public readonly long ScreenY;

        [IntrinsicField]
        public readonly Element Target;
    }
}