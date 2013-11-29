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
        public PropertySourceBindingInfo(
            TypeReference sourceType,
            List<MemberReference> propertyReferences)
        {
            this.SourceType = sourceType;
            this.PropertyReferencePath = new List<MemberReference>(propertyReferences);
            if (propertyReferences[0].IsStatic())
            {
                this.SourceType = null;
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

            observablePaths.Add(new List<MemberReference>());
            for (int iProp = 0; iProp < this.PropertyReferencePath.Count - 1; iProp++)
            {
                observablePaths[observablePaths.Count-1].Add(this.PropertyReferencePath[iProp]);
                TypeReference returnType = 
                    this.PropertyReferencePath[iProp] is PropertyReference
                        ? ((PropertyReference)this.PropertyReferencePath[iProp]).PropertyType
                        : ((FieldReference)this.PropertyReferencePath[iProp]).FieldType;
                if (codeGenerator.CodeGenerator.ParserContext.ClrResolver.TypeImplements(
                    returnType,
                    codeGenerator.CodeGenerator.KnownTypes.ObservableInterface))
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
    }
}
