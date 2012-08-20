//-----------------------------------------------------------------------
// <copyright file="TemplateManager.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Html;

    /// <summary>
    /// Definition for TemplateManager
    /// </summary>
    public class TemplateManager
    {
        StringDictionary<Func<Element, SkinInstance>> skinFactoryDelegates = new StringDictionary<Func<Element, SkinInstance>>();
        StringDictionary<List<KeyValuePair<string, string>>> primarySkins = new StringDictionary<List<KeyValuePair<string, string>>>();
        StringDictionary<Func<Element, UIElement>> elementFactories = new StringDictionary<Func<Element, UIElement>>();
        StringDictionary<List<KeyValuePair<string, string>>> primaryUIElements = new StringDictionary<List<KeyValuePair<string, string>>>();
    }
}
