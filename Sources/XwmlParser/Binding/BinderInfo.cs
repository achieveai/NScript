//-----------------------------------------------------------------------
// <copyright file="BinderInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser
{
    using System;
    using System.Collections.Generic;
    using XwmlParser.Binding;

    public enum SourceType
    {
        Static,
        Parent,
        DataContext,
        NamedPart
    }

    /// <summary>
    /// Definition for BinderInfo
    /// </summary>
    public class BinderInfo
    {
        public BinderInfo(
            TargetBindingInfo targetBindingInfo,
            SourceBindingInfo sourceBindingInfo,
            ConverterInfo converterInfo,
            BindingMode mode,
            SourceType sourceType,
            string namedPartName)
        {
            this.TargetBindingInfo = targetBindingInfo;
            this.SourceBindingInfo = sourceBindingInfo;
            this.ConverterInfo = converterInfo;
            this.SourceType = sourceType;
            this.NamedPartName = namedPartName;
            this.Mode = mode;
        }

        public BindingMode Mode { get; private set; }

        public TargetBindingInfo TargetBindingInfo { get; private set; }

        public SourceBindingInfo SourceBindingInfo { get; private set; }

        public ConverterInfo ConverterInfo { get; private set; }

        public SourceType SourceType { get; private set; }

        public string NamedPartName { get; private set; }
    }
}
