//-----------------------------------------------------------------------
// <copyright file="PropertyValue.cs" company="Microsoft Corp.">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Template.Compiler.PropertyBuilders
{
    using System.IO;

    /// <summary>
    /// Base class for all the property values.
    /// </summary>
    public abstract class PropertyValue
    {
        /// <summary>
        /// Writes the value to js.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public abstract void WriteValueToJs(TextWriter writer);

        /// <summary>
        /// Adds the type references.
        /// </summary>
        public virtual void AddTypeReferences()
        {
        }
    }
}
