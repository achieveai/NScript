//-----------------------------------------------------------------------
// <copyright file="TestUIElements.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI.Test
{
    using System;
    using SunlightUnit;
    using Sunlight.Framework.UI.Attributes;
    using Sunlight.Framework.Binders;
    using System.Web.Html;

    public class TestUIElement : UIElement
    {
        private int twoWayLooseBinding;
        public TestUIElement(Element element)
            : base(element)
        { }

        [CssName]
        public string CssString { get; set; }

        [DefaultDataBinding(DefaultValue="test", Mode=DataBindingMode.OneWay, IsStrict=true)]
        public string OneWayStrictBinding { get; set; }

        [DefaultDataBinding(DefaultValue=10, Mode=DataBindingMode.OneWay, IsStrict=false)]
        public int OneWayLooseBinding { get; set; }

        [DefaultDataBinding(DefaultValue=11, Mode=DataBindingMode.TwoWay, IsStrict=false)]
        public int TwoWayLooseBinding
        {
            get
            {
                return this.twoWayLooseBinding;
            }

            set
            {
                if (this.twoWayLooseBinding != value)
                {
                    this.twoWayLooseBinding = value;
                    this.FirePropertyChanged("TwoWayLooseBinding");
                }
            }
        }
    }

    [TagName("abcd")]
    public class TestUIElementWithTag : UIElement
    {
        public TestUIElementWithTag(Element element)
            : base(element)
        { }
    }

    [DomAttribute("test", "value")]
    public class TestUIElementWithAttr : UIElement
    {
        public TestUIElementWithAttr(Element element)
            : base(element)
        { }
    }

    public class TestSkinableWithTestUIElementPart : UISkinableElement
    {
        [SkinPart(typeof(TestUIElement))]
        const string myPart = "Part1";

        public TestSkinableWithTestUIElementPart(Element element)
            : base(element)
        { }

        public TestUIElement Part
        {
            get
            {
                if (base.SkinInstance != null)
                {
                    return (TestUIElement)base.SkinInstance.GetChildById(myPart);
                }

                return null;
            }
        }
    }

    public class TestSkinableWithDomElementPart : TestSkinableWithTestUIElementPart
    {
        [SkinPart(typeof(Element), true)]
        const string myDomPart = "DomPart";

        public TestSkinableWithDomElementPart(Element element)
            : base(element)
        { }

        public Element Part
        {
            get
            {
                if (base.SkinInstance != null)
                {
                    return (Element)base.SkinInstance.GetChildById(myDomPart);
                }

                return null;
            }
        }
    }
}

