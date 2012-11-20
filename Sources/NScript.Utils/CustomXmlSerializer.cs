//-----------------------------------------------------------------------
// <copyright file="CustomXmlSerializer.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Xml;

    /// <summary>
    /// Definition for CustomXmlSerializer
    /// </summary>
    public class CustomXmlSerializer : ICustomSerializer
    {
        private readonly XmlWriter writer;

        public CustomXmlSerializer(XmlWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        public int Version
        {
            get;
            set;
        }

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddValue(string name, sbyte value)
        {
            this.writer.WriteStartElement(name);
            this.writer.WriteValue(value);
            this.writer.WriteEndElement();
        }

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddValue(string name, byte value)
        {
            this.writer.WriteStartElement(name);
            this.writer.WriteValue(value);
            this.writer.WriteEndElement();
        }

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddValue(string name, short value)
        {
            this.writer.WriteStartElement(name);
            this.writer.WriteValue(value);
            this.writer.WriteEndElement();
        }

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddValue(string name, ushort value)
        {
            this.writer.WriteStartElement(name);
            this.writer.WriteValue(value);
            this.writer.WriteEndElement();
        }

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddValue(string name, int value)
        {
            this.writer.WriteStartElement(name);
            this.writer.WriteValue(value);
            this.writer.WriteEndElement();
        }

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddValue(string name, uint value)
        {
            this.writer.WriteStartElement(name);
            this.writer.WriteValue(value);
            this.writer.WriteEndElement();
        }

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddValue(string name, IntPtr value)
        {
            this.writer.WriteStartElement(name);
            this.writer.WriteValue(value);
            this.writer.WriteEndElement();
        }

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddValue(string name, UIntPtr value)
        {
            this.writer.WriteStartElement(name);
            this.writer.WriteValue(value);
            this.writer.WriteEndElement();
        }

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddValue(string name, bool value)
        {
            this.writer.WriteStartElement(name);
            this.writer.WriteValue(value);
            this.writer.WriteEndElement();
        }

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddValue(string name, string value)
        {
            this.writer.WriteStartElement(name);
            this.writer.WriteValue(value);
            this.writer.WriteEndElement();
        }

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddValue(string name, char value)
        {
            this.writer.WriteStartElement(name);
            this.writer.WriteValue(value);
            this.writer.WriteEndElement();
        }

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddValue(string name, double value)
        {
            this.writer.WriteStartElement(name);
            this.writer.WriteValue(value);
            this.writer.WriteEndElement();
        }

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddValue(string name, float value)
        {
            this.writer.WriteStartElement(name);
            this.writer.WriteValue(value);
            this.writer.WriteEndElement();
        }

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddValue(string name, decimal value)
        {
            this.writer.WriteStartElement(name);
            this.writer.WriteValue(value.ToString());
            this.writer.WriteEndElement();
        }

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddValue(string name, Guid value)
        {
            this.writer.WriteStartElement(name);
            this.writer.WriteValue(value.ToString());
            this.writer.WriteEndElement();
        }

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddValue(string name, DateTime value)
        {
            this.writer.WriteStartElement(name);
            this.writer.WriteValue(value);
            this.writer.WriteEndElement();
        }

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddValue(string name, ICustomSerializable value)
        {
            if (value != null)
            {
                this.writer.WriteStartElement(name);
                this.writer.WriteAttributeString("type", value.GetType().Name);
                value.Serialize(this);
                this.writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddValue(string name, Enum value)
        {
            this.writer.WriteStartElement(name);
            this.writer.WriteAttributeString("type", value.GetType().Name);
            this.writer.WriteValue(value.ToString());
            this.writer.WriteEndElement();
        }

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddValue(string name, IEnumerable<ICustomSerializable> value)
        {
            if (value != null)
            {
                this.writer.WriteStartElement(name);
                int i = 0;
                foreach (var serializable in value)
                {
                    this.writer.WriteStartElement(string.Format("Elem{0}", i++));
                    if (serializable != null)
                    {
                        this.writer.WriteAttributeString("type", serializable.GetType().Name);
                        serializable.Serialize(this);
                    }
                    this.writer.WriteEndElement();
                }
                this.writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddValue(string name, IEnumerable<string> value)
        {
            if (value != null)
            {
                this.writer.WriteStartElement(name);
                int i = 0;
                foreach (var str in value)
                {
                    this.AddValue(
                        string.Format("Elem{0}", i++),
                        str);
                }
                this.writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddValue(string name, long value)
        {
            this.writer.WriteStartElement(name);
            this.writer.WriteValue(value);
            this.writer.WriteEndElement();
        }

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddValue(string name, ulong value)
        {
            this.writer.WriteStartElement(name);
            this.writer.WriteValue((decimal)value);
            this.writer.WriteEndElement();
        }

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="valueSerializer">The value serializer.</param>
        public void AddValue<T>(string name, T value, Action<ICustomSerializer, T> valueSerializer)
        {
            if (value != null)
            {
                this.writer.WriteStartElement(name);
                valueSerializer(this, value);
                this.writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="collection">The collection.</param>
        /// <param name="valueSerializer">The value serializer.</param>
        public void AddValue<T>(string name, IEnumerable<T> collection, Action<ICustomSerializer, T> valueSerializer)
        {
            if (collection != null)
            {
                this.writer.WriteStartElement(name);
                int i = 0;
                foreach (var serializable in collection)
                {
                    this.writer.WriteStartElement(string.Format("Elem{0}", i++));
                    if (serializable != null)
                    {
                        valueSerializer(this, serializable);
                    }
                    this.writer.WriteEndElement();
                }
                this.writer.WriteEndElement();
            }
        }
    }
}
