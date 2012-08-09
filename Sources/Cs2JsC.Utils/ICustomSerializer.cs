//-----------------------------------------------------------------------
// <copyright file="ICustomSerializer.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Utils
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for ICustomSerializer
    /// </summary>
    public interface ICustomSerializer
    {
        /// <summary>
        /// Gets the version.
        /// </summary>
        int Version
        { get; }

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void AddValue(string name, sbyte value);

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void AddValue(string name, byte value);

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void AddValue(string name, short value);

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void AddValue(string name, ushort value);

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void AddValue(string name, int value);

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void AddValue(string name, uint value);

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void AddValue(string name, long value);

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void AddValue(string name, ulong value);

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void AddValue(string name, IntPtr value);

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void AddValue(string name, UIntPtr value);

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        void AddValue(string name, bool value);

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void AddValue(string name, string value);

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void AddValue(string name, char value);

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void AddValue(string name, double value);

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void AddValue(string name, float value);

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void AddValue(string name, decimal value);

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void AddValue(string name, Guid value);

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void AddValue(string name, DateTime value);

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void AddValue(string name, Enum value);

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="obj">The obj.</param>
        void AddValue(string name, ICustomSerializable value);

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void AddValue(string name, IEnumerable<ICustomSerializable> value);

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void AddValue(string name, IEnumerable<string> value);

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="valueSerializer">The value serializer.</param>
        void AddValue<T>(string name, T value, Action<ICustomSerializer, T> valueSerializer);

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="collection">The collection.</param>
        /// <param name="valueSerializer">The value serializer.</param>
        void AddValue<T>(string name, IEnumerable<T> collection, Action<ICustomSerializer, T> valueSerializer);
    }
}
