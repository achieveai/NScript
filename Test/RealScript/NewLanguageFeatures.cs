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

        public static int? TestConditionalAccess(NewLanguageFeatures obj)
        {
            return obj?.AddNum(10);
        }

        public static int? TestConditionalAccess2(NewLanguageFeatures obj)
        {
            return obj?.NameofField()?.Length;
        }

        public static int? TestConditionalInvoke(Func<int> func)
            => func?.Invoke();

        public static Func<int, long, int> TestNestedFunction(NewLanguageFeatures obj)
        {
            int Compute(long l)
            {
                return obj.AddNum((int)l);
            }

            return (x, y) =>
            {
                return x + Compute(y);
            };
        }

        public static int TestNestedFunctionScoped(NewLanguageFeatures obj)
        {
            int Compute(long l)
            {
                int Compute2(int l2)
                {
                    return l2 + 10;
                }

                return obj.AddNum(Compute2((int)l));
            }

            if (obj == null)
            {
                int Compute2(long l)
                {
                    return (int)l;
                }

                return Compute2(obj.AddNum(10));
            }

            return Compute(obj.AddNum(11));
        }

//         public static List<NewLanguageFeatures> TestNestedFunctionGeneric(NewLanguageFeatures obj)
//         {
//             List<U> Compute<U>(U item, int count)
//             {
//                 var rv = new List<U>();
//                 while(count-- < 0)
//                 { rv.Add(item); }
// 
//                 return rv;
//             }
// 
//             return Compute(obj, 10);
//         }
    }
}
