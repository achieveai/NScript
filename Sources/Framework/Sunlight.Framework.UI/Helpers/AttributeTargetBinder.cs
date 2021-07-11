//-----------------------------------------------------------------------
// <copyright file="AttributeBinder.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI.Helpers
{
    using Sunlight.Framework.Binders;
    using Sunlight.Framework.Observables;

    /// <summary>
    /// Definition for AttributeBinder
    /// </summary>
    public class AttributeTargetBinder : TargetBinder
    {
        /// <summary>
        /// Name of the attribute.
        /// </summary>
        private readonly string attributeName;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="attributeName"> Name of the attribute. </param>
        public AttributeTargetBinder(
            string attributeName)
            : base("AttributeBinder:" + attributeName, null, null, false)
        {
            this.attributeName = attributeName;
            this.SetPropertySetter(this.Setter);
        }

        /// <summary>
        /// Setters.
        /// </summary>
        /// <param name="target"> Target for the. </param>
        /// <param name="value">  The value. </param>
        private void Setter(INotifyPropertyChanged target, object value)
        {
            this.RealSetter((UIElement)target, (string)value);
        }

        /// <summary>
        /// Real setter.
        /// </summary>
        /// <param name="uiElement"> The element. </param>
        /// <param name="value">     The value. </param>
        private void RealSetter(UIElement uiElement, string value)
        {
            if (value != null)
            {
                uiElement.Element.SetAttribute(this.attributeName, value);
            }
            else
            {
                uiElement.Element.RemoveAttribute(this.attributeName);
            }
        }
    }
}
