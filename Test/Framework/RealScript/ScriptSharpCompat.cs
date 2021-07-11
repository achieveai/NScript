//-----------------------------------------------------------------------
// <copyright file="ScriptSharpCompat.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System.Collections;

    /// <summary>
    /// Definition for ScriptSharpCompat
    /// </summary>
    public class ScriptSharpCompat
    {
        public int ReturnIntrinsicProperty(string str)
        {
            return str.Length;
        }

        public object ReturnIntrinsicIndexer(Dictionary dict)
        {
            return dict["Foo"];
        }

        public void SetIntrinsicIndexer(Dictionary dict, object value)
        {
            dict["Foo"] = value;
        }

        public Dictionary GetDictionary(string str)
        {
            return Dictionary.GetDictionary(str);
        }

        public System.Delegate DelegateCombine(System.Delegate del1, System.Delegate del2)
        {
            return System.Delegate.Combine(del1, del2);
        }
    }
}
