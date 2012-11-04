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
        public StringDictionary<Func<Element, SkinInstance>> skinFactoryDelegates = new StringDictionary<Func<Element, SkinInstance>>();
        public StringDictionary<List<KeyValuePair<string, string>>> primarySkins = new StringDictionary<List<KeyValuePair<string, string>>>();
        public StringDictionary<Func<Element, UIElement>> elementFactories = new StringDictionary<Func<Element, UIElement>>();
        public StringDictionary<List<KeyValuePair<string, string>>> primaryUIElements = new StringDictionary<List<KeyValuePair<string, string>>>();
    }
}
