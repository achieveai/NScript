//-----------------------------------------------------------------------
// <copyright file="TemplateInstance.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI
{
    using System;
    using System.Collections.Generic;
    using System.Web.Html;

    /// <summary>
    /// Definition for TemplateInstance
    /// </summary>
    public class TemplateInstance
    {
        private Node[] rootElements;
        private Action<object> elementDataBinders;
        private Action<UISkinableElement> elementTemplateParentBinders;

        public TemplateInstance(
            Element rootElement,
            Action<object> dataContextBinders,
            Action<UISkinableElement> templateParentBinders)
        {
            var childNodes = rootElement.ChildNodes;
            this.rootElements = new Node[childNodes.Length];
            this.elementDataBinders = dataContextBinders;
            this.elementTemplateParentBinders = templateParentBinders;

            for (int i = rootElements.Length - 1; i >= 0; i--)
            {
                this.rootElements[i] = childNodes[i];
            }
        }
    }
}
