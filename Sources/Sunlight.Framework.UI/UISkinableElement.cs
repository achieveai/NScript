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
        public const string SkinPropertyName = "Skin";
        public const string SkinInstancePropertyName = "SkinInstance";

        private Skin skin;

        private SkinInstance skinInstance;

        public UISkinableElement(Element element)
            : base(element)
        {
        }

        public Skin Skin
        {
            get
            {
                return this.skin;
            }

            set
            {
                if (this.skin != value)
                {
                    this.skin = value;
                    if (this.skin != null
                        && this.IsActive)
                    {
                        this.SkinInstance = this.skin.CreateInstance();
                    }

                    this.FirePropertyChanged(SkinPropertyName);
                }
            }
        }

        protected SkinInstance SkinInstance
        {
            get
            {
                return this.skinInstance;
            }

            private set
            {
                if (this.skinInstance != value)
                {
                    if (this.skinInstance != null)
                    {
                        this.skinInstance.Dispose();
                    }

                    this.skinInstance = value;

                    if (this.skinInstance != null)
                    {
                        this.skinInstance.Bind(this);

                        if (this.IsActive)
                        {
                            this.skinInstance.Activate();
                        }

                        this.ApplySkinInternal(this.skinInstance);
                    }

                    this.FirePropertyChanged(SkinInstancePropertyName);
                }
            }
        }

        protected virtual void ApplySkinInternal(SkinInstance skin)
        {
        }

        protected override void OnBeforeFirstActivate()
        {
            base.OnBeforeFirstActivate();

            if (this.skin != null
                && this.skinInstance == null)
            {
                this.SkinInstance = this.skin.CreateInstance();
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            if (this.skin != null
                && this.skinInstance == null)
            {
                this.SkinInstance = this.skin.CreateInstance();
            }
            else if (this.skinInstance != null)
            {
                this.skinInstance.Activate();
            }
        }

        protected override void OnDeactivate()
        {
            if (this.skinInstance != null)
            {
                this.skinInstance.Deactivate();
            }

            base.OnDeactivate();
        }

        protected override void InternalDispose()
        {
            if (this.SkinInstance != null)
            {
                this.SkinInstance = null;
            }

            this.Skin = null;

            base.InternalDispose();
        }

        protected override void OnDataContextUpdated(object oldValue)
        {
            base.OnDataContextUpdated(oldValue);

            if (this.skinInstance != null)
            {
                this.skinInstance.UpdateDataContext();
            }
        }
    }
}
