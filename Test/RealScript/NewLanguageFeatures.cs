//-----------------------------------------------------------------------
// <copyright file="NewLanguageFeatures.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;

    /// <summary>
    /// Definition for NewLanguageFeatures
    /// </summary>
    public class NewLanguageFeatures
    {
        private int field;

        public NewLanguageFeatures(int tmp)
        { field = tmp; }

        public int InlineProp => field;

        public int AddNum(int x)
            => field + x;

        public string NameofField()
            => nameof(field);
    }
}
