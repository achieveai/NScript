//-----------------------------------------------------------------------
// <copyright file="CustomJsonSerializer.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Utils
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;

    /// <summary>
    /// Definition for CustomJsonSerializer
    /// </summary>
    public class CustomJsonSerializer : ICustomSerializer
    {
        const string typeProp = "_t";
        bool emitTypeInfo = false;
        JsonTextWriter writer;

        public CustomJsonSerializer(
            TextWriter writer,
            bool emitTypeInfo = false)
        {
            this.writer = new JsonTextWriter(writer);
            this.writer.Indentation = 2;
            this.writer.IndentChar = ' ';
            this.writer.Formatting = Formatting.Indented;
            this.emitTypeInfo = emitTypeInfo;
        }

        public void Serialize(ICustomSerializable serializable)
        {
            this.writer.WriteStartObject();
            serializable.Serialize(this);
            this.writer.WriteEnd();
        }

        public void Close()
        {
            this.writer.Close();
        }

        #region ICustomSerializer Members

        public int Version
        {
            get { return 2; }
        }

        public void AddValue(string name, sbyte value)
        {
            this.writer.WritePropertyName(name);
            this.writer.WriteValue(value);
        }

        public void AddValue(string name, byte value)
        {
            this.writer.WritePropertyName(name);
            this.writer.WriteValue(value);
        }

        public void AddValue(string name, short value)
        {
            this.writer.WritePropertyName(name);
            this.writer.WriteValue(value);
        }

        public void AddValue(string name, ushort value)
        {
            this.writer.WritePropertyName(name);
            this.writer.WriteValue(value);
        }

        public void AddValue(string name, int value)
        {
            this.writer.WritePropertyName(name);
            this.writer.WriteValue(value);
        }

        public void AddValue(string name, uint value)
        {
            this.writer.WritePropertyName(name);
            this.writer.WriteValue(value);
        }

        public void AddValue(string name, long value)
        {
            this.writer.WritePropertyName(name);
            this.writer.WriteValue(value);
        }

        public void AddValue(string name, ulong value)
        {
            this.writer.WritePropertyName(name);
            this.writer.WriteValue(value);
        }

        public void AddValue(string name, IntPtr value)
        {
            this.writer.WritePropertyName(name);
            this.writer.WriteValue(value);
        }

        public void AddValue(string name, UIntPtr value)
        {
            this.writer.WritePropertyName(name);
            this.writer.WriteValue(value);
        }

        public void AddValue(string name, bool value)
        {
            this.writer.WritePropertyName(name);
            this.writer.WriteValue(value);
        }

        public void AddValue(string name, string value)
        {
            this.writer.WritePropertyName(name);
            this.writer.WriteValue(value);
        }

        public void AddValue(string name, char value)
        {
            this.writer.WritePropertyName(name);
            this.writer.WriteValue(value);
        }

        public void AddValue(string name, double value)
        {
            this.writer.WritePropertyName(name);
            this.writer.WriteValue(value);
        }

        public void AddValue(string name, float value)
        {
            this.writer.WritePropertyName(name);
            this.writer.WriteValue(value);
        }

        public void AddValue(string name, decimal value)
        {
            this.writer.WritePropertyName(name);
            this.writer.WriteValue(value);
        }

        public void AddValue(string name, Guid value)
        {
            this.writer.WritePropertyName(name);
            this.writer.WriteValue(value);
        }

        public void AddValue(string name, DateTime value)
        {
            this.writer.WritePropertyName(name);
            this.writer.WriteValue(value);
        }

        public void AddValue(string name, Enum value)
        {
            this.writer.WritePropertyName(name);
            this.writer.WriteValue(value.ToString());
        }

        public void AddValue(string name, ICustomSerializable value)
        {
            this.writer.WritePropertyName(name);
            if (value != null)
            {
                this.writer.WriteStartObject();
                if (this.emitTypeInfo)
                {
                    this.writer.WritePropertyName(CustomJsonSerializer.typeProp);
                    this.writer.WriteValue(value.GetType().Name);
                }

                value.Serialize(this);
                this.writer.WriteEndObject();
            }
            else
            {
                this.writer.WriteNull();
            }
        }

        public void AddValue(string name, IEnumerable<ICustomSerializable> value)
        {
            this.writer.WritePropertyName(name);
            if (value != null)
            {
                this.writer.WriteStartArray();
                foreach (var item in value)
                {
                    if (item != null)
                    {
                        this.writer.WriteStartObject();
                        if (this.emitTypeInfo)
                        {
                            this.writer.WritePropertyName(CustomJsonSerializer.typeProp);
                            this.writer.WriteValue(item.GetType().Name);
                        }

                        item.Serialize(this);
                        this.writer.WriteEndObject();
                    }
                    else
                    {
                        this.writer.WriteNull();
                    }
                }

                this.writer.WriteEndArray();
            }
            else
            {
                this.writer.WriteNull();
            }
        }

        public void AddValue(string name, IEnumerable<string> value)
        {
            this.writer.WritePropertyName(name);
            if (value != null)
            {
                this.writer.WriteStartArray();
                foreach (var item in value)
                {
                    this.writer.WriteStartObject();
                    this.writer.WriteValue(item);
                    this.writer.WriteEndObject();
                }

                this.writer.WriteEndArray();
            }
            else
            {
                this.writer.WriteNull();
            }
        }

        public void AddValue<T>(string name, T value, Action<ICustomSerializer, T> valueSerializer)
        {
            this.writer.WritePropertyName(name);
            if (value != null)
            {
                this.writer.WriteStartObject();
                if (this.emitTypeInfo)
                {
                    this.writer.WritePropertyName(CustomJsonSerializer.typeProp);
                    this.writer.WriteValue(value.GetType().Name);
                }

                valueSerializer(this, value);
                this.writer.WriteEndObject();
            }
            else
            {
                this.writer.WriteNull();
            }
        }

        public void AddValue<T>(string name, IEnumerable<T> collection, Action<ICustomSerializer, T> valueSerializer)
        {
            this.writer.WritePropertyName(name);
            if (collection != null)
            {
                this.writer.WriteStartArray();
                foreach (var item in collection)
                {
                    if (item != null)
                    {
                        this.writer.WriteStartObject();
                        if (this.emitTypeInfo)
                        {
                            this.writer.WritePropertyName(CustomJsonSerializer.typeProp);
                            this.writer.WriteValue(item.GetType().Name);
                        }

                        valueSerializer(this, item);
                        this.writer.WriteEndObject();
                    }
                    else
                    {
                        this.writer.WriteNull();
                    }
                }

                this.writer.WriteEndArray();
            }
            else
            {
                this.writer.WriteNull();
            }
        }

        #endregion ICustomSerializer Members
    }
}