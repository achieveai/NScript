namespace Sunlight.Framework.UI.Attributes
{
	using System;
	using System.Collections.Generic;

    class SkinFileAttribute : Attribute
    {
        private string resourceName;
        public SkinFileAttribute(string resourceName)
        {
            this.resourceName = resourceName;
        }

        public string Template
        {
            get { return this.resourceName; }
        }
    }
}
