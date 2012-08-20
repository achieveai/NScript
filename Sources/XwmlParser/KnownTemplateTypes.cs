//-----------------------------------------------------------------------
// <copyright file="KnownTemplateTypes.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for KnownTemplateTypes
    /// </summary>
    public class KnownTemplateTypes
    {
        public TypeReference ContextBindableObject
        {
            get;
            set;
        }

        public TypeReference UIElement
        {
            get;
            set;
        }

        public TypeReference Control
        {
            get;
            set;
        }

        public TypeReference Panel
        {
            get;
            set;
        }

        public TypeReference ObservableObject
        {
            get;
            set;
        }

        public TypeReference ArrayType
        {
            get;
            set;
        }

        public TypeReference IListType
        {
            get;
            set;
        }
    }
}
