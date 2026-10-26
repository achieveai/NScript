//-----------------------------------------------------------------------
// <copyright file="PropertySourceBindingInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.Binding
{
    using Mono.Cecil;
    using NScript.JST;
    using NScript.CLR;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for PropertySourceBindingInfo
    /// </summary>
    public class PropertySourceBindingInfo : SourceBindingInfo
    {
        private readonly TypeReference castValueType;

        public PropertySourceBindingInfo(
            TypeReference sourceType,
            List<MemberReference> propertyReferences,
            TypeReference valueType = null)
        {
            this.SourceType = sourceType;
            this.castValueType = valueType;
            this.PropertyReferencePath = new List<MemberReference>(propertyReferences);
            if (propertyReferences[0].IsStatic())
            {
                this.SourceType = null;
            }
        }

        internal override TypeReference ValueType
        {
            get
            {
                if (this.castValueType != null)
                { return this.castValueType; }

                var lastPart = this.PropertyReferencePath[this.PropertyReferencePath.Count - 1];
                if (lastPart is PropertyReference)
                {
                    return ((PropertyReference)lastPart).PropertyType;
                }
                else
                {
                    return ((FieldReference)lastPart).FieldType;
                }
            }
        }

        internal override bool IsStatic
        { get { return this.SourceType == null; } }

        public TypeReference SourceType { get; private set; }

        public List<MemberReference> PropertyReferencePath { get; private set; }

        internal override Tuple<IList<string>, IList<IIdentifier>, IIdentifier>
            GenerateGetterSetterInfo(
                SkinCodeGenerator codeGenerator,
                BindingMode mode)
        {
            if (mode == BindingMode.OneTime)
            {
                return Tuple.Create(
                    (IList<string>)null,
                    (IList<IIdentifier>)new IIdentifier[]{
                        codeGenerator.CodeGenerator.GetGetterForPropertyPath(
                            this.PropertyReferencePath)
                    },
                    (IIdentifier)null);
            }

            List<IIdentifier> rvGetter = new List<IIdentifier>();

            if (mode == BindingMode.OneTime)
            {
                rvGetter.Add(codeGenerator.CodeGenerator.GetGetterForPropertyPath(
                    this.PropertyReferencePath));

                return Tuple.Create(
                    (IList<string>)null,
                    (IList<IIdentifier>)rvGetter,
                    (IIdentifier)null);
            }

            List<List<MemberReference>> observablePaths = new List<List<MemberReference>>();
            List<string> rvNames = new List<string>();
            IIdentifier rvSetter = null;

            var resolver = codeGenerator.CodeGenerator.ParserContext.ClrResolver;
            var observableInterface = codeGenerator.CodeGenerator.KnownTypes.ObservableInterface;

            observablePaths.Add(new List<MemberReference>());
            for (int iProp = 0; iProp < this.PropertyReferencePath.Count - 1; iProp++)
            {
                observablePaths[observablePaths.Count-1].Add(this.PropertyReferencePath[iProp]);
                if (PropertySourceBindingInfo.IsObservable(
                    PropertySourceBindingInfo.GetMemberType(this.PropertyReferencePath[iProp]),
                    resolver,
                    observableInterface))
                {
                    observablePaths.Add(new List<MemberReference>());
                }
            }

            observablePaths[observablePaths.Count-1].Add(
                this.PropertyReferencePath[this.PropertyReferencePath.Count-1]);

            for (int iPathPart = 0; iPathPart < observablePaths.Count; iPathPart++)
            {
                rvGetter.Add(codeGenerator.CodeGenerator.GetGetterForPropertyPath(
                    observablePaths[iPathPart]));

                rvNames.Add(observablePaths[iPathPart][0].Name);
            }

            // A plain (or static) root can't notify; null tells LiveBinder to read it without listening.
            if (!this.IsRootObservable(resolver, observableInterface))
            {
                rvNames[0] = null;
            }

            if (mode == BindingMode.TwoWay)
            {
                rvSetter = codeGenerator.CodeGenerator.GetSetterForProperty(
                    this.PropertyReferencePath[this.PropertyReferencePath.Count - 1]);
            }

            return Tuple.Create(
                (IList<string>)rvNames,
                (IList<IIdentifier>)rvGetter,
                rvSetter);
        }

        /// <summary>
        /// True when a live binding can listen somewhere: the root or any object it walks through.
        /// The last member's value is never listened to, so its type does not count.
        /// </summary>
        internal bool HasObservablePart(IClrResolver resolver, TypeReference observableInterface)
        {
            if (this.IsRootObservable(resolver, observableInterface))
            {
                return true;
            }

            for (int iProp = 0; iProp < this.PropertyReferencePath.Count - 1; iProp++)
            {
                if (PropertySourceBindingInfo.IsObservable(
                    PropertySourceBindingInfo.GetMemberType(this.PropertyReferencePath[iProp]),
                    resolver,
                    observableInterface))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsRootObservable(IClrResolver resolver, TypeReference observableInterface)
        {
            return this.SourceType != null
                && PropertySourceBindingInfo.IsObservable(this.SourceType, resolver, observableInterface);
        }

        private static bool IsObservable(
            TypeReference type,
            IClrResolver resolver,
            TypeReference observableInterface)
        {
            return type.IsSame(observableInterface)
                || resolver.TypeImplements(type, observableInterface);
        }

        private static TypeReference GetMemberType(MemberReference member)
        {
            return member is PropertyReference
                ? ((PropertyReference)member).PropertyType
                : ((FieldReference)member).FieldType;
        }
    }
}
