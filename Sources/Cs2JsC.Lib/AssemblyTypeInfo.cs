using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Cs2JsC.Lib
{
    class AssemblyTypeInfo
    {
        #region member variables
        Type type;
        MemberInfo[] memberInfos; 
        #endregion

        #region constructors/Factories
        public AssemblyTypeInfo(Type type)
        {
            this.type = type;

            this.memberInfos = this.type.GetMembers(
                BindingFlags.CreateInstance |
                BindingFlags.DeclaredOnly |
                BindingFlags.ExactBinding |
                BindingFlags.FlattenHierarchy |
                BindingFlags.GetField |
                BindingFlags.GetProperty |
                BindingFlags.IgnoreCase |
                BindingFlags.IgnoreReturn |
                BindingFlags.Instance |
                BindingFlags.InvokeMethod |
                BindingFlags.NonPublic |
                BindingFlags.OptionalParamBinding |
                BindingFlags.Public |
                BindingFlags.PutDispProperty |
                BindingFlags.PutRefDispProperty |
                BindingFlags.SetField |
                BindingFlags.SetProperty |
                BindingFlags.Static |
                BindingFlags.SuppressChangeType);
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        #endregion

        #region public functions
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        #endregion
    }
}
