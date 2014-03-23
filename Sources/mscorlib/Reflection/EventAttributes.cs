//-----------------------------------------------------------------------
// <copyright file="EventAttributes.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Reflection
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

	/// <summary>Specifies the attributes of an event.</summary>
	[Serializable]
	public enum EventAttributes
	{
		/// <summary>Specifies that the event has no attributes.</summary>
		None = 0,
		/// <summary>Specifies that the event is special in a way described by the name.</summary>
		SpecialName = 512,
		/// <summary>Specifies a reserved flag for common language runtime use only.</summary>
		ReservedMask = 1024,
		/// <summary>Specifies that the common language runtime should check name encoding.</summary>
		RTSpecialName = 1024
	}
}
