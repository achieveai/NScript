namespace System.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

	public interface IReflectableType
	{
		TypeInfo GetTypeInfo();
	}
}
