namespace System.Runtime.CompilerServices
{
    using System;
    using System.ComponentModel;

    [Extended, EditorBrowsable(EditorBrowsableState.Never), NonScriptable, AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public sealed class CompilerFeatureRequiredAttribute : Attribute
    {
        public const string RefStructs = "RefStructs";
        public const string RequiredMembers = "RequiredMembers";

        public CompilerFeatureRequiredAttribute(string featureName)
        {
            this.FeatureName = featureName;
        }

        public string FeatureName { get; }

        public bool IsOptional { get; set; }
    }
}
