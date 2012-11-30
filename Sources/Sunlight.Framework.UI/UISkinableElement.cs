//-----------------------------------------------------------------------
// <copyright file="UISkinableElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI
{
    using System;
    using System.Web.Html;
using Sunlight.Framework.UI.Helpers;

    /// <summary>
    /// Definition for UISkinableElement
    /// </summary>
    public class UISkinableElement : UIElement
    {
        public const string SkinFactoryPropertyName = "SkinFactory";
        public const string SkinPropertyName = "Skin";

        private SkinFactory skinFactory;
        private SkinInstance skin;

        public UISkinableElement(Element element)
            : base(element)
        {
        }

        public SkinFactory SkinFactory
        {
            get
            {
                return this.skinFactory;
            }

            set
            {
                if (this.skinFactory != value)
                {
                    this.skinFactory = value;
                    if (this.skinFactory != null)
                    {
                        this.Skin = this.skinFactory.CreateInstance();
                    }

                    this.FirePropertyChanged(SkinFactoryPropertyName);
                }
            }
        }

        public SkinInstance Skin
        {
            get
            {
                return this.skin;
            }

            private set
            {
                if (this.skin != value)
                {
                    if (this.skin != null)
                    {
                        this.skin.Dispose();
                    }

                    this.skin = value;

                    if (this.skin != null)
                    {
                        this.skin.Bind(this, this.Parent);

                        if (this.IsActive)
                        {
                            this.skin.Activate();
                        }
                    }

                    this.FirePropertyChanged(SkinPropertyName);
                }
            }
        }

        protected void ApplySkinInternal(SkinInstance skin)
        { }
    }
}
