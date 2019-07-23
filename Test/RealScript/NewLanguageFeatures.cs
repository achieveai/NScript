//-----------------------------------------------------------------------
// <copyright file="NewLanguageFeatures.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;
    using System.Collections.Generic;

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

        public int InitProp { get; } = int.Parse("21");

        public int? OutVarParam(IDictionary<string, int> dict, string str)
        {
            if (dict.TryGetValue(str, out var value))
            {
                return value;
            }

            return null;
        }
    }
}
