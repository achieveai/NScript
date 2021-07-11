//-----------------------------------------------------------------------
// <copyright file="DynamicAttribute.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Runtime.CompilerServices
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for DynamicAttribute
    /// </summary>
    public class DynamicAttribute : Attribute
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public DynamicAttribute()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="transformFlags"> The transform flags. </param>
        public DynamicAttribute(bool[] transformFlags)
        {
            this.TransformFlags = transformFlags;
        }

        /// <summary>
        /// Gets or sets the transform flags.
        /// </summary>
        /// <value>
        /// The transform flags.
        /// </value>
        public IList<bool> TransformFlags
        {
            get;
            private set;
        }
    }

    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    internal sealed class __DynamicallyInvokableAttribute : Attribute
    {
    }

	/// <summary>Indicates that the .NET Framework class library method to which this attribute is applied is unlikely to be affected by servicing releases, and therefore is eligible to be inlined across Native Image Generator (NGen) images.</summary>
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class TargetedPatchingOptOutAttribute : Attribute
	{
		private string m_reason;
		/// <summary>Gets the reason why the method to which this attribute is applied is considered to be eligible for inlining across Native Image Generator (NGen) images.</summary>
		/// <returns>The reason why the method is considered to be eligible for inlining across NGen images.</returns>
		public string Reason
		{
			get
			{
				return this.m_reason;
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.TargetedPatchingOptOutAttribute" /> class.</summary>
		/// <param name="reason">The reason why the method to which the <see cref="T:System.Runtime.TargetedPatchingOptOutAttribute" /> attribute is applied is considered to be eligible for inlining across Native Image Generator (NGen) images.</param>
		public TargetedPatchingOptOutAttribute(string reason)
		{
			this.m_reason = reason;
		}
		private TargetedPatchingOptOutAttribute()
		{
		}
	}}
