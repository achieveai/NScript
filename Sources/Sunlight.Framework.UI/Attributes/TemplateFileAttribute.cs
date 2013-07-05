namespace Sunlight.Framework.UI.Attributes
{
	using System;
	using System.Collections.Generic;
	using System.Text;

    class TemplateFileAttribute : Attribute
    {
        private string resourceName;
        public TemplateFileAttribute(string resourceName)
        {
            this.resourceName = resourceName;
        }

        public string Template
        {
            get { return this.resourceName; }
        }
    }
}
