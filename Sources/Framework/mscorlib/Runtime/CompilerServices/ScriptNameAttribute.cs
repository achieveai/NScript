namespace System.Runtime.CompilerServices
{
    using System;

    /// <summary>
    /// Attribute for script name. Use this method to name the method/property/type. It won't change the relationship
    /// to other things like namespace, type, etc.
    /// If you want to generate global name, use ScriptAlias.
    /// </summary>
    [NonScriptable, Extended, AttributeUsage(AttributeTargets.Event | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Struct | AttributeTargets.Class, Inherited=false, AllowMultiple=false)]
    public sealed class ScriptNameAttribute : Attribute
    {
        private string _name;

        public ScriptNameAttribute(string name)
        {
            this._name = name;
        }

        public string Name
        {
            get
            {
                return this._name;
            }
        }
    }
}

