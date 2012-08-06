//-----------------------------------------------------------------------
// <copyright file="Enums.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    /// <summary>
    /// Definition for Enums
    /// </summary>
    public enum DropEffect
    {
        Copy,
        Link,
        Move,
        None
    }

    public enum DropEffects
    {
        Copy,
        Link,
        Move,
        CopyLink,
        CopyMove,
        LinkMove,
        All,
        None
    }

    public enum DataFormat
    {
        Text,
        URL
    }

    public enum ElementType
    {
        Element = 1,
        Attribute,
        Text,
        CharacterData,
        EntityReference,
        Entity,
        ProcessingInstruction,
        Comment,
        Document,
        DocumentType,
        DocumentFragment,
        Notation
    }
}