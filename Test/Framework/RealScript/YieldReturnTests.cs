//-----------------------------------------------------------------------
// <copyright file="YieldReturnTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System.Collections.Generic;

    /// <summary>
    /// Definition for YieldReturnTests
    /// </summary>
    public class YieldReturnTests
    {
        public static IEnumerator<string> GetEnumeratorNestedForeach(System.Collections.Generic.IList<string> strList)
        {
            foreach (var kv in strList)
                yield return kv;
        }

        public static IEnumerator<string> GetEnumeratorNestedFor(System.Collections.Generic.IList<string> strList)
        {
            for (int i = 0; i < strList.Count; i++)
            {
                if (i % 2 == 0)
                {
                    yield return strList[i];
                }
            }
        }
    }
}