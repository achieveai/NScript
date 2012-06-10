//-----------------------------------------------------------------------
// <copyright file="IntPropertyValue.cs" company="Microsoft Corp.">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Cs2JsC.Template.Compiler.PropertyBuilders
{
    using System;
    using Cs2JsC.PELoader.Reflection;

    /// <summary>
    /// Int property value.
    /// </summary>
    public class EnumPropertyValue : PropertyValue
    {
        /// <summary>
        /// value field.
        /// </summary>
        private long value;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntPropertyValue"/> class.
        /// </summary>
        /// <param name="value">The  value.</param>
        public EnumPropertyValue(TypeReference typeInfo, string enumValue)
        {
            this.value = this.GetEnumValue(typeInfo, enumValue);
        }

        /// <summary>
        /// Writes the value to js.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void WriteValueToJs(System.IO.TextWriter writer)
        {
            writer.Write(this.value);
        }

        /// <summary>
        /// Gets the enum value.
        /// </summary>
        /// <param name="typeInfo">The type info.</param>
        /// <param name="enumValue">The enum value.</param>
        /// <returns>Enum value.</returns>
        private long GetEnumValue(TypeReference typeInfo, string enumValue)
        {
            TypeDefinition type = typeInfo.Type as TypeDefinition;

            for (int fieldIndex = 1; fieldIndex < type.Fields.Count; fieldIndex++)
            {
                FieldDefinition fieldInfo = type.Fields[fieldIndex];

                if (fieldInfo.Name == enumValue &&
                    fieldInfo.IsConst)
                {
                    object rv = fieldInfo.ConstValue;

                    if (rv is int)
                    {
                        return (int)rv;
                    }

                    if (rv is short)
                    {
                        return (short)rv;
                    }

                    if (rv is long)
                    {
                        return (long)rv;
                    }

                    if (rv is sbyte)
                    {
                        return (sbyte)rv;
                    }

                    if (rv is byte)
                    {
                        return (byte)rv;
                    }

                    if (rv is ushort)
                    {
                        return (ushort)rv;
                    }

                    if (rv is uint)
                    {
                        return (uint)rv;
                    }

                    if (rv is ulong)
                    {
                        return (long)(ulong)rv;
                    }
                }
            }

            throw new ApplicationException(
                string.Format(
                    "Invalid enum value: {0}",
                    enumValue));
        }
    }
}
