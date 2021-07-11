//-----------------------------------------------------------------------
// <copyright file="ConverterLocationException.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter
{
    using NScript.Utils;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for ConverterLocationException
    /// </summary>
    public class ConverterLocationException : ApplicationException
    {
        /// <summary>
        /// The location.
        /// </summary>
        private readonly Location location;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="location">       The location. </param>
        /// <param name="message">        The message. </param>
        /// <param name="innerException"> The inner exception. </param>
        public ConverterLocationException(Location location, string message, Exception innerException)
            : base(message, innerException)
        {
            this.location = location;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="location"> The location. </param>
        public ConverterLocationException(Location location, string message)
            : base(message)
        {
            this.location = location;
        }

        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        public Location Location
        { get { return this.location; } }
    }
}
