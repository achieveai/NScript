//-----------------------------------------------------------------------
// <copyright file="$fileinputname$.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace $rootnamespace$
{
    using Sunlight.Framework.Observables;
    using Sunlight.Framework.UI;
    using Sunlight.Framework.UI.Attributes;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for $safeitemrootname$
    /// </summary>
    public class $safeitemrootname$ : ObservableObject
    {

        /// <summary>
        /// Gets the default skin.
        /// </summary>
        /// <value>
        /// The default skin.
        /// </value>
        [Skin("$rootnamespace$.$safeitemrootname$:DefaultSkin")]
        public static Skin DefaultSkin
        { get { return null; } }
    }
}
