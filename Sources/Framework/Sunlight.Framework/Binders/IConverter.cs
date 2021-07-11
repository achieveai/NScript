//-----------------------------------------------------------------------
// <copyright file="IConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Binders
{
    using System;

    /// <summary>
    /// Definition for IConverter.
    /// </summary>
    /// <typeparam name="To">   Type of to. </typeparam>
    /// <typeparam name="From"> Type of from. </typeparam>
    /// <typeparam name="Arg">  Type of the argument. </typeparam>
    public interface IToConverter<To, From, Arg>
    {
        /// <summary>
        /// Converts to To Type.
        /// </summary>
        /// <param name="obj"> The object. </param>
        /// <param name="arg"> The argument. </param>
        /// <returns>
        /// Converted value.
        /// </returns>
        To Convert(From obj, Arg arg);
    }

    /// <summary>
    /// Interface for from converter.
    /// </summary>
    /// <typeparam name="To">   Type of to. </typeparam>
    /// <typeparam name="From"> Type of from. </typeparam>
    /// <typeparam name="Arg">  Type of the argument. </typeparam>
    public interface IFromConverter<To, From, Arg> : IToConverter<To, From, Arg>
    {
        /// <summary>
        /// Convert back.
        /// </summary>
        /// <param name="obj"> The object. </param>
        /// <param name="arg"> The argument. </param>
        /// <returns>
        /// Converted value back to ToType.
        /// </returns>
        From ConvertBack(To obj, Arg arg);
    }
}