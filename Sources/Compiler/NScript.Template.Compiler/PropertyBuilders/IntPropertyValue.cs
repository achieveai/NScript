//-----------------------------------------------------------------------
// <copyright file="IntPropertyValue.cs" company="Microsoft Corp.">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace NScript.Template.Compiler.PropertyBuilders
{
    /// <summary>
    /// Int property value.
    /// </summary>
    public class IntPropertyValue : PropertyValue
    {
        /// <summary>
        /// value field.
        /// </summary>
        private int value;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntPropertyValue"/> class.
        /// </summary>
        /// <param name="value">The  value.</param>
        public IntPropertyValue(int value)
        {
            this.value = value;
        }

        /// <summary>
        /// Writes the value to js.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void WriteValueToJs(System.IO.TextWriter writer)
        {
            writer.Write(this.value);
        }
    }
}
