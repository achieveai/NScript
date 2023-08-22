namespace Sunlight.Framework.UI.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AutoFireAttribute : Attribute
    {
        public AutoFireAttribute(params string[] alsoFire) { }
    }
}
