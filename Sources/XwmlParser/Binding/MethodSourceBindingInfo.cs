//-----------------------------------------------------------------------
// <copyright file="MethodSourceBindingInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.Binding
{
    using Mono.Cecil;
    using NScript.JST;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for MethodSourceBindingInfo
    /// </summary>
    public class MethodSourceBindingInfo : SourceBindingInfo
    {
        public MethodSourceBindingInfo(
            PropertySourceBindingInfo propertySourceBinding,
            MethodReference methodReference)
        {
            this.MethodReference = methodReference;
            this.PropertySourceBinding = propertySourceBinding;
        }

        public PropertySourceBindingInfo PropertySourceBinding
        { get; private set; }

        public MethodReference MethodReference
        { get; private set; }

        internal override Tuple<IList<string>, IList<IIdentifier>, IIdentifier>
            GenerateGetterSetterInfo(
                SkinCodeGenerator codeGenerator,
                BindingMode mode)
        {
            Tuple<IList<string>, IList<IIdentifier>, IIdentifier> rv = null;

            if (this.PropertySourceBinding != null)
            {
                rv = this.PropertySourceBinding.GenerateGetterSetterInfo(
                    codeGenerator,
                    mode);
            }

            // Final getter should look something like this
            // function(s) {
            //  return type.CreateDelegate(getter(s), "methodName", getter(s).methodName);
            // }
            // OR
            // function(s) {
            //  return type.CreateDelegate(s, "methodName", s.methodName);
            // }
            IIdentifier delegateGetter = codeGenerator.CodeGenerator.GetGetterForDelegate(
                rv != null && rv.Item2 != null && rv.Item2.Count > 0
                    ? rv.Item2[rv.Item2.Count-1]
                    : null,
                this.MethodReference);

            if (rv != null)
            {
                rv.Item2[rv.Item2.Count - 1] = delegateGetter;

                rv = Tuple.Create(rv.Item1, rv.Item2, (IIdentifier)null);
            }
            else
            {
                rv = Tuple.Create(
                    (IList<string>)null,
                    (IList<IIdentifier>)new List<IIdentifier>() { delegateGetter },
                    (IIdentifier)null);
            }

            return rv;
        }
    }
}
