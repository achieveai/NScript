//-----------------------------------------------------------------------
// <copyright file="DataTransfer.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition of DataTransfer
    /// </summary>
    [IgnoreNamespace]
    public sealed class DataTransfer
    {
        private extern DataTransfer();

        public extern DropEffect DropEffect
        { get; set; }

        public extern DropEffects EffectAllowed
        { get; set; }

        public extern void ClearData();

        public void ClearData(DataFormat format)
        {
            this.ClearData(
                format == DataFormat.Text
                    ? DataFormatStrings.Text
                    : DataFormatStrings.URL);
        }

        public string GetData(DataFormat format)
        {
            return this.GetData(
                format == DataFormat.Text
                    ? DataFormatStrings.Text
                    : DataFormatStrings.URL);
        }

        public bool SetData(DataFormat format, string data)
        {
            return this.SetData(
                format == DataFormat.Text
                    ? DataFormatStrings.Text
                    : DataFormatStrings.URL,
                data);
        }

        internal extern void ClearData(string format);

        internal extern string GetData(string format);

        internal extern bool SetData(string format, string data);
    }
}