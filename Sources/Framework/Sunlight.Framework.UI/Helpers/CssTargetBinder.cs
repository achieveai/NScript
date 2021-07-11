//-----------------------------------------------------------------------
// <copyright file="CssTargetBinder.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI.Helpers
{
    using System;
    using Sunlight.Framework.Binders;
    using Sunlight.Framework.Observables;
    using System.Web.Html;

    /// <summary>
    /// Definition for CssTargetBinder
    /// </summary>
    public class CssTargetBinder : TargetBinder
    {
        private string className;

        public CssTargetBinder(
            string className)
            : base("CssBinder:" + className, null, null, false)
        {
            this.className = className;
            this.SetPropertySetter(this.Setter);
        }

		public static void Bind(Element element, string className, bool value)
        {
            if (value)
            {
                element.ClassList.Add(className);
            }
			else
            {
				element.ClassList.Remove(className);
            }
        }

        private void Setter(INotifyPropertyChanged target, object isTrue)
        {
            this.RealSetter((UIElement)target, (bool)isTrue);
        }

        private void RealSetter(UIElement uiElement, bool isTrue)
        {
            if (isTrue)
            {
                uiElement.AddClass(this.className);
            }
            else
            {
                uiElement.RemoveClass(this.className);
            }
        }
    }
}
