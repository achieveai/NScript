//-----------------------------------------------------------------------
// <copyright file="Activator.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System
{
    using System;

    // Summary:
    //     Contains methods to create types of objects locally or remotely, or obtain
    //     references to existing remote objects. This class cannot be inherited.
    public sealed class Activator
    {
        //
        // Summary:
        //     Creates an instance of the type designated by the specified generic type
        //     parameter, using the parameterless constructor .
        //
        // Type parameters:
        //   T:
        //     The type to create.
        //
        // Returns:
        //     A reference to the newly created object.
        //
        // Exceptions:
        //   System.MissingMethodException:
        //     The type that is specified for T does not have a parameterless constructor.
        public static T CreateInstance<T>()
        {
            return (T)Activator.CreateInstance(typeof(T));
        }

        //
        // Summary:
        //     Creates an instance of the specified type using that type's default constructor.
        //
        // Parameters:
        //   type:
        //     The type of object to create.
        //
        // Returns:
        //     A reference to the newly created object.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     type is null.
        //
        //   System.ArgumentException:
        //     type is not a RuntimeType. -or-type is an open generic type (that is, the
        //     System.Type.ContainsGenericParameters property returns true).
        //
        //   System.NotSupportedException:
        //     type cannot be a System.Reflection.Emit.TypeBuilder.-or- Creation of System.TypedReference,
        //     System.ArgIterator, System.Void, and System.RuntimeArgumentHandle types,
        //     or arrays of those types, is not supported.-or-The assembly that contains
        //     type is a dynamic assembly that was created with System.Reflection.Emit.AssemblyBuilderAccess.Save.
        //
        //   System.Reflection.TargetInvocationException:
        //     The constructor being called throws an exception.
        //
        //   System.MethodAccessException:
        //     The caller does not have permission to call this constructor.
        //
        //   System.MemberAccessException:
        //     Cannot create an instance of an abstract class, or this member was invoked
        //     with a late-binding mechanism.
        //
        //   System.Runtime.InteropServices.InvalidComObjectException:
        //     The COM type was not obtained through Overload:System.Type.GetTypeFromProgID
        //     or Overload:System.Type.GetTypeFromCLSID.
        //
        //   System.MissingMethodException:
        //     No matching public constructor was found.
        //
        //   System.Runtime.InteropServices.COMException:
        //     type is a COM object but the class identifier used to obtain the type is
        //     invalid, or the identified class is not registered.
        //
        //   System.TypeLoadException:
        //     type is not a valid type.
        public extern static object CreateInstance(Type type);

        //
        // Summary:
        //     Creates an instance of the specified type using that type's default constructor.
        //
        // Parameters:
        //   type:
        //     The type of object to create.
        //
        //   nonPublic:
        //     true if a public or nonpublic default constructor can match; false if only
        //     a public default constructor can match.
        //
        // Returns:
        //     A reference to the newly created object.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     type is null.
        //
        //   System.ArgumentException:
        //     type is not a RuntimeType. -or-type is an open generic type (that is, the
        //     System.Type.ContainsGenericParameters property returns true).
        //
        //   System.NotSupportedException:
        //     type cannot be a System.Reflection.Emit.TypeBuilder.-or- Creation of System.TypedReference,
        //     System.ArgIterator, System.Void, and System.RuntimeArgumentHandle types,
        //     or arrays of those types, is not supported. -or-The assembly that contains
        //     type is a dynamic assembly that was created with System.Reflection.Emit.AssemblyBuilderAccess.Save.
        //
        //   System.Reflection.TargetInvocationException:
        //     The constructor being called throws an exception.
        //
        //   System.MethodAccessException:
        //     The caller does not have permission to call this constructor.
        //
        //   System.MemberAccessException:
        //     Cannot create an instance of an abstract class, or this member was invoked
        //     with a late-binding mechanism.
        //
        //   System.Runtime.InteropServices.InvalidComObjectException:
        //     The COM type was not obtained through Overload:System.Type.GetTypeFromProgID
        //     or Overload:System.Type.GetTypeFromCLSID.
        //
        //   System.MissingMethodException:
        //     No matching public constructor was found.
        //
        //   System.Runtime.InteropServices.COMException:
        //     type is a COM object but the class identifier used to obtain the type is
        //     invalid, or the identified class is not registered.
        //
        //   System.TypeLoadException:
        //     type is not a valid type.
        public extern static object CreateInstance(Type type, bool nonPublic);

        //
        // Summary:
        //     Creates an instance of the specified type using the constructor that best
        //     matches the specified parameters.
        //
        // Parameters:
        //   type:
        //     The type of object to create.
        //
        //   args:
        //     An array of arguments that match in number, order, and type the parameters
        //     of the constructor to invoke. If args is an empty array or null, the constructor
        //     that takes no parameters (the default constructor) is invoked.
        //
        //   activationAttributes:
        //     An array of one or more attributes that can participate in activation. This
        //     is typically an array that contains a single System.Runtime.Remoting.Activation.UrlAttribute
        //     object. The System.Runtime.Remoting.Activation.UrlAttribute specifies the
        //     URL that is required to activate a remote object. For a detailed description
        //     of client-activated objects, see Client Activation.
        //
        // Returns:
        //     A reference to the newly created object.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     type is null.
        //
        //   System.ArgumentException:
        //     type is not a RuntimeType. -or-type is an open generic type (that is, the
        //     System.Type.ContainsGenericParameters property returns true).
        //
        //   System.NotSupportedException:
        //     type cannot be a System.Reflection.Emit.TypeBuilder.-or- Creation of System.TypedReference,
        //     System.ArgIterator, System.Void, and System.RuntimeArgumentHandle types,
        //     or arrays of those types, is not supported.-or- activationAttributes is not
        //     an empty array, and the type being created does not derive from System.MarshalByRefObject.
        //     -or-The assembly that contains type is a dynamic assembly that was created
        //     with System.Reflection.Emit.AssemblyBuilderAccess.Save.-or-The constructor
        //     that best matches args has varargs arguments.
        //
        //   System.Reflection.TargetInvocationException:
        //     The constructor being called throws an exception.
        //
        //   System.MethodAccessException:
        //     The caller does not have permission to call this constructor.
        //
        //   System.MemberAccessException:
        //     Cannot create an instance of an abstract class, or this member was invoked
        //     with a late-binding mechanism.
        //
        //   System.Runtime.InteropServices.InvalidComObjectException:
        //     The COM type was not obtained through Overload:System.Type.GetTypeFromProgID
        //     or Overload:System.Type.GetTypeFromCLSID.
        //
        //   System.MissingMethodException:
        //     No matching public constructor was found.
        //
        //   System.Runtime.InteropServices.COMException:
        //     type is a COM object but the class identifier used to obtain the type is
        //     invalid, or the identified class is not registered.
        //
        //   System.TypeLoadException:
        //     type is not a valid type.
        // public static object CreateInstance(Type type, object[] args, object[] activationAttributes);

        //
        // Summary:
        //     Creates an instance of the specified type using the constructor that best
        //     matches the specified parameters.
        //
        // Parameters:
        //   type:
        //     The type of object to create.
        //
        //   bindingAttr:
        //     A combination of zero or more bit flags that affect the search for the type
        //     constructor. If bindingAttr is zero, a case-sensitive search for public constructors
        //     is conducted.
        //
        //   binder:
        //     An object that uses bindingAttr and args to seek and identify the type constructor.
        //     If binder is null, the default binder is used.
        //
        //   args:
        //     An array of arguments that match in number, order, and type the parameters
        //     of the constructor to invoke. If args is an empty array or null, the constructor
        //     that takes no parameters (the default constructor) is invoked.
        //
        //   culture:
        //     Culture-specific information that governs the coercion of args to the formal
        //     types declared for the type constructor. If culture is null, the System.Globalization.CultureInfo
        //     for the current thread is used.
        //
        // Returns:
        //     A reference to the newly created object.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     type is null.
        //
        //   System.ArgumentException:
        //     type is not a RuntimeType. -or-type is an open generic type (that is, the
        //     System.Type.ContainsGenericParameters property returns true).
        //
        //   System.NotSupportedException:
        //     type cannot be a System.Reflection.Emit.TypeBuilder.-or- Creation of System.TypedReference,
        //     System.ArgIterator, System.Void, and System.RuntimeArgumentHandle types,
        //     or arrays of those types, is not supported. -or-The assembly that contains
        //     type is a dynamic assembly that was created with System.Reflection.Emit.AssemblyBuilderAccess.Save.-or-The
        //     constructor that best matches args has varargs arguments.
        //
        //   System.Reflection.TargetInvocationException:
        //     The constructor being called throws an exception.
        //
        //   System.MethodAccessException:
        //     The caller does not have permission to call this constructor.
        //
        //   System.MemberAccessException:
        //     Cannot create an instance of an abstract class, or this member was invoked
        //     with a late-binding mechanism.
        //
        //   System.Runtime.InteropServices.InvalidComObjectException:
        //     The COM type was not obtained through Overload:System.Type.GetTypeFromProgID
        //     or Overload:System.Type.GetTypeFromCLSID.
        //
        //   System.MissingMethodException:
        //     No matching constructor was found.
        //
        //   System.Runtime.InteropServices.COMException:
        //     type is a COM object but the class identifier used to obtain the type is
        //     invalid, or the identified class is not registered.
        //
        //   System.TypeLoadException:
        //     type is not a valid type.
        // public static object CreateInstance(Type type, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture);
        //
        // Summary:
        //     Creates an instance of the specified type using the constructor that best
        //     matches the specified parameters.
        //
        // Parameters:
        //   type:
        //     The type of object to create.
        //
        //   bindingAttr:
        //     A combination of zero or more bit flags that affect the search for the type
        //     constructor. If bindingAttr is zero, a case-sensitive search for public constructors
        //     is conducted.
        //
        //   binder:
        //     An object that uses bindingAttr and args to seek and identify the type constructor.
        //     If binder is null, the default binder is used.
        //
        //   args:
        //     An array of arguments that match in number, order, and type the parameters
        //     of the constructor to invoke. If args is an empty array or null, the constructor
        //     that takes no parameters (the default constructor) is invoked.
        //
        //   culture:
        //     Culture-specific information that governs the coercion of args to the formal
        //     types declared for the type constructor. If culture is null, the System.Globalization.CultureInfo
        //     for the current thread is used.
        //
        //   activationAttributes:
        //     An array of one or more attributes that can participate in activation. This
        //     is typically an array that contains a single System.Runtime.Remoting.Activation.UrlAttribute
        //     object. The System.Runtime.Remoting.Activation.UrlAttribute specifies the
        //     URL that is required to activate a remote object. For a detailed description
        //     of client-activated objects, see Client Activation.
        //
        // Returns:
        //     A reference to the newly created object.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     type is null.
        //
        //   System.ArgumentException:
        //     type is not a RuntimeType. -or-type is an open generic type (that is, the
        //     System.Type.ContainsGenericParameters property returns true).
        //
        //   System.NotSupportedException:
        //     type cannot be a System.Reflection.Emit.TypeBuilder.-or- Creation of System.TypedReference,
        //     System.ArgIterator, System.Void, and System.RuntimeArgumentHandle types,
        //     or arrays of those types, is not supported.-or- activationAttributes is not
        //     an empty array, and the type being created does not derive from System.MarshalByRefObject.
        //     -or-The assembly that contains type is a dynamic assembly that was created
        //     with System.Reflection.Emit.AssemblyBuilderAccess.Save.-or-The constructor
        //     that best matches args has varargs arguments.
        //
        //   System.Reflection.TargetInvocationException:
        //     The constructor being called throws an exception.
        //
        //   System.MethodAccessException:
        //     The caller does not have permission to call this constructor.
        //
        //   System.MemberAccessException:
        //     Cannot create an instance of an abstract class, or this member was invoked
        //     with a late-binding mechanism.
        //
        //   System.Runtime.InteropServices.InvalidComObjectException:
        //     The COM type was not obtained through Overload:System.Type.GetTypeFromProgID
        //     or Overload:System.Type.GetTypeFromCLSID.
        //
        //   System.MissingMethodException:
        //     No matching constructor was found.
        //
        //   System.Runtime.InteropServices.COMException:
        //     type is a COM object but the class identifier used to obtain the type is
        //     invalid, or the identified class is not registered.
        //
        //   System.TypeLoadException:
        //     type is not a valid type.
        // public static object CreateInstance(Type type, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes);
    }
}
