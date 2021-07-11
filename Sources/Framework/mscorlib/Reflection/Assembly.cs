//-----------------------------------------------------------------------
// <copyright file="Assembly.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>Represents an assembly, which is a reusable, versionable, and self-describing building block of a common language runtime application.</summary>
    [Serializable]
    public abstract class Assembly : ICustomAttributeProvider
    {
        /// <summary>Gets the location of the assembly as specified originally, for example, in an <see cref="T:System.Reflection.AssemblyName" /> object.</summary>
        /// <returns>The location of the assembly as specified originally.</returns>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public extern virtual string CodeBase
        {
            get;
        }

        /// <summary>Gets the URI, including escape characters, that represents the codebase.</summary>
        /// <returns>A URI with escape characters.</returns>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public extern virtual string EscapedCodeBase
        {
            get;
        }

        /// <summary>Gets the display name of the assembly.</summary>
        /// <returns>The display name of the assembly.</returns>
        public extern virtual string FullName
        {
            get;
        }

        /// <summary>Gets the entry point of this assembly.</summary>
        /// <returns>An object that represents the entry point of this assembly. If no entry point is found (for example, the assembly is a DLL), null is returned.</returns>
        public extern virtual MethodInfo EntryPoint
        {
            get;
        }

        public extern virtual IEnumerable<Type> ExportedTypes
        {
            get;
        }

        public extern virtual IEnumerable<TypeInfo> DefinedTypes
        {
            get;
        }

        /// <summary>Gets a value that indicates whether the current assembly is loaded with full trust.</summary>
        /// <returns>true if the current assembly is loaded with full trust; otherwise, false.</returns>
        public extern bool IsFullyTrusted
        {
            get;
        }

        /// <summary>Gets the module that contains the manifest for the current assembly. </summary>
        /// <returns>The module that contains the manifest for the assembly. </returns>
        public extern virtual Module ManifestModule
        {
            get;
        }

        public extern virtual IEnumerable<CustomAttributeData> CustomAttributes
        {
            get;
        }

        /// <summary>Gets a <see cref="T:System.Boolean" /> value indicating whether this assembly was loaded into the reflection-only context.</summary>
        /// <returns>true if the assembly was loaded into the reflection-only context, rather than the execution context; otherwise, false.</returns>
        [ComVisible(false)]
        public extern virtual bool ReflectionOnly
        {
            get;
        }

        public extern virtual IEnumerable<Module> Modules
        {
            get;
        }

        /// <summary>Gets the path or UNC location of the loaded file that contains the manifest.</summary>
        /// <returns>The location of the loaded file that contains the manifest. If the loaded file was shadow-copied, the location is that of the file after being shadow-copied. If the assembly is loaded from a byte array, such as when using the <see cref="M:System.Reflection.Assembly.Load(System.Byte[])" /> method overload, the value returned is an empty string ("").</returns>
        /// <exception cref="T:System.NotSupportedException">The current assembly is a dynamic assembly, represented by an <see cref="T:System.Reflection.Emit.AssemblyBuilder" /> object. </exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public extern virtual string Location
        {
            get;
        }

        /// <summary>Gets a string representing the version of the common language runtime (CLR) saved in the file containing the manifest.</summary>
        /// <returns>The CLR version folder name. This is not a full path.</returns>
        [ComVisible(false)]
        public extern virtual string ImageRuntimeVersion
        {
            get;
        }

        /// <summary>Gets a value indicating whether the assembly was loaded from the global assembly cache.</summary>
        /// <returns>true if the assembly was loaded from the global assembly cache; otherwise, false.</returns>
        public extern virtual bool GlobalAssemblyCache
        {
            get;
        }

        /// <summary>Gets the host context with which the assembly was loaded.</summary>
        /// <returns>An <see cref="T:System.Int64" /> value that indicates the host context with which the assembly was loaded, if any.</returns>
        [ComVisible(false)]
        public extern virtual long HostContext
        {
            get;
        }

        /// <summary>Gets a value that indicates whether the current assembly was generated dynamically in the current process by using reflection emit.</summary>
        /// <returns>true if the current assembly was generated dynamically in the current process; otherwise, false.</returns>
        public extern virtual bool IsDynamic
        {
            get;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Reflection.Assembly" /> class.</summary>
        protected Assembly()
        {
        }

        /// <summary>Creates the name of a type qualified by the display name of its assembly.</summary>
        /// <returns>The full name of the type qualified by the display name of the assembly.</returns>
        /// <param name="assemblyName">The display name of an assembly. </param>
        /// <param name="typeName">The full name of a type. </param>
        public extern static string CreateQualifiedName(string assemblyName, string typeName);

        /// <summary>Gets the currently loaded assembly in which the specified class is defined.</summary>
        /// <returns>The assembly in which the specified class is defined.</returns>
        /// <param name="type">An object representing a class in the assembly that will be returned. </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="type" /> is null. </exception>
        public extern static Assembly GetAssembly(Type type);

        /// <summary>Indicates whether two <see cref="T:System.Reflection.Assembly" /> objects are equal.</summary>
        /// <returns>true if <paramref name="left" /> is equal to <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The assembly to compare to <paramref name="right" />. </param>
        /// <param name="right">The assembly to compare to <paramref name="left" />.</param>
        public extern static bool operator ==(Assembly left, Assembly right);

        /// <summary>Indicates whether two <see cref="T:System.Reflection.Assembly" /> objects are not equal.</summary>
        /// <returns>true if <paramref name="left" /> is not equal to <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The assembly to compare to <paramref name="right" />.</param>
        /// <param name="right">The assembly to compare to <paramref name="left" />.</param>
        public extern static bool operator !=(Assembly left, Assembly right);

        /// <summary>Gets the assembly that contains the code that is currently executing.</summary>
        /// <returns>The assembly that contains the code that is currently executing. </returns>
        public extern static Assembly GetExecutingAssembly();

        /// <summary>Returns the <see cref="T:System.Reflection.Assembly" /> of the method that invoked the currently executing method.</summary>
        /// <returns>The Assembly object of the method that invoked the currently executing method.</returns>
        public extern static Assembly GetCallingAssembly();

        /// <summary>Gets the process executable in the default application domain. In other application domains, this is the first executable that was executed by <see cref="M:System.AppDomain.ExecuteAssembly(System.String)" />.</summary>
        /// <returns>The assembly that is the process executable in the default application domain, or the first executable that was executed by <see cref="M:System.AppDomain.ExecuteAssembly(System.String)" />. Can return null when called from unmanaged code.</returns>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="ControlEvidence" />
        /// </PermissionSet>
        public extern static Assembly GetEntryAssembly();

        /// <summary>Gets an <see cref="T:System.Reflection.AssemblyName" /> for this assembly.</summary>
        /// <returns>An object that contains the fully parsed display name for this assembly.</returns>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public extern virtual AssemblyName GetName();


        /// <summary>Gets an <see cref="T:System.Reflection.AssemblyName" /> for this assembly, setting the codebase as specified by <paramref name="copiedName" />.</summary>
        /// <returns>An object that contains the fully parsed display name for this assembly.</returns>
        /// <param name="copiedName">true to set the <see cref="P:System.Reflection.Assembly.CodeBase" /> to the location of the assembly after it was shadow copied; false to set <see cref="P:System.Reflection.Assembly.CodeBase" /> to the original location. </param>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public extern virtual AssemblyName GetName(bool copiedName);

        internal extern virtual string GetFullNameForEtw();

        /// <summary>Gets the <see cref="T:System.Type" /> object with the specified name in the assembly instance.</summary>
        /// <returns>An object that represents the specified class, or null if the class is not found.</returns>
        /// <param name="name">The full name of the type. </param>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="name" /> is invalid. </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="name" /> is null. </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">
        ///   <paramref name="name" /> requires a dependent assembly that could not be found. </exception>
        /// <exception cref="T:System.IO.FileLoadException">
        ///   <paramref name="name" /> requires a dependent assembly that was found but could not be loaded.-or-The current assembly was loaded into the reflection-only context, and <paramref name="name" /> requires a dependent assembly that was not preloaded. </exception>
        /// <exception cref="T:System.BadImageFormatException">
        ///   <paramref name="name" /> requires a dependent assembly, but the file is not a valid assembly. -or-<paramref name="name" /> requires a dependent assembly which was compiled for a version of the runtime later than the currently loaded version.</exception>
        public extern virtual Type GetType(string name);

        /// <summary>Gets the <see cref="T:System.Type" /> object with the specified name in the assembly instance and optionally throws an exception if the type is not found.</summary>
        /// <returns>An object that represents the specified class.</returns>
        /// <param name="name">The full name of the type. </param>
        /// <param name="throwOnError">true to throw an exception if the type is not found; false to return null. </param>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="name" /> is invalid.-or- The length of <paramref name="name" /> exceeds 1024 characters. </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="name" /> is null. </exception>
        /// <exception cref="T:System.TypeLoadException">
        ///   <paramref name="throwOnError" /> is true, and the type cannot be found.</exception>
        /// <exception cref="T:System.IO.FileNotFoundException">
        ///   <paramref name="name" /> requires a dependent assembly that could not be found. </exception>
        /// <exception cref="T:System.IO.FileLoadException">
        ///   <paramref name="name" /> requires a dependent assembly that was found but could not be loaded.-or-The current assembly was loaded into the reflection-only context, and <paramref name="name" /> requires a dependent assembly that was not preloaded. </exception>
        /// <exception cref="T:System.BadImageFormatException">
        ///   <paramref name="name" /> requires a dependent assembly, but the file is not a valid assembly. -or-<paramref name="name" /> requires a dependent assembly which was compiled for a version of the runtime later than the currently loaded version.</exception>
        public extern virtual Type GetType(string name, bool throwOnError);

        /// <summary>Gets the <see cref="T:System.Type" /> object with the specified name in the assembly instance, with the options of ignoring the case, and of throwing an exception if the type is not found.</summary>
        /// <returns>An object that represents the specified class.</returns>
        /// <param name="name">The full name of the type. </param>
        /// <param name="throwOnError">true to throw an exception if the type is not found; false to return null. </param>
        /// <param name="ignoreCase">true to ignore the case of the type name; otherwise, false. </param>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="name" /> is invalid.-or- The length of <paramref name="name" /> exceeds 1024 characters. </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="name" /> is null. </exception>
        /// <exception cref="T:System.TypeLoadException">
        ///   <paramref name="throwOnError" /> is true, and the type cannot be found.</exception>
        /// <exception cref="T:System.IO.FileNotFoundException">
        ///   <paramref name="name" /> requires a dependent assembly that could not be found. </exception>
        /// <exception cref="T:System.IO.FileLoadException">
        ///   <paramref name="name" /> requires a dependent assembly that was found but could not be loaded.-or-The current assembly was loaded into the reflection-only context, and <paramref name="name" /> requires a dependent assembly that was not preloaded. </exception>
        /// <exception cref="T:System.BadImageFormatException">
        ///   <paramref name="name" /> requires a dependent assembly, but the file is not a valid assembly. -or-<paramref name="name" /> requires a dependent assembly which was compiled for a version of the runtime later than the currently loaded version.</exception>
        public extern virtual Type GetType(string name, bool throwOnError, bool ignoreCase);

        /// <summary>Gets the public types defined in this assembly that are visible outside the assembly.</summary>
        /// <returns>An array that represents the types defined in this assembly that are visible outside the assembly.</returns>
        /// <exception cref="T:System.NotSupportedException">The assembly is a dynamic assembly.</exception>
        public extern virtual Type[] GetExportedTypes();

        /// <summary>Gets the types defined in this assembly.</summary>
        /// <returns>An array that contains all the types that are defined in this assembly.</returns>
        /// <exception cref="T:System.Reflection.ReflectionTypeLoadException">The assembly contains one or more types that cannot be loaded. The array returned by the <see cref="P:System.Reflection.ReflectionTypeLoadException.Types" /> property of this exception contains a <see cref="T:System.Type" /> object for each type that was loaded and null for each type that could not be loaded, while the <see cref="P:System.Reflection.ReflectionTypeLoadException.LoaderExceptions" /> property contains an exception for each type that could not be loaded.</exception>
        public extern virtual Type[] GetTypes();

        /// <summary>Gets the satellite assembly for the specified culture.</summary>
        /// <returns>The specified satellite assembly.</returns>
        /// <param name="culture">The specified culture. </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="culture" /> is null. </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">The assembly cannot be found. </exception>
        /// <exception cref="T:System.IO.FileLoadException">The satellite assembly with a matching file name was found, but the CultureInfo did not match the one specified. </exception>
        /// <exception cref="T:System.BadImageFormatException">The satellite assembly is not a valid assembly. </exception>
        public extern virtual Assembly GetSatelliteAssembly(CultureInfo culture);

        /// <summary>Gets all the custom attributes for this assembly.</summary>
        /// <returns>An array that contains the custom attributes for this assembly.</returns>
        /// <param name="inherit">This argument is ignored for objects of type <see cref="T:System.Reflection.Assembly" />. </param>
        public extern virtual object[] GetCustomAttributes(bool inherit);

        /// <summary>Gets the custom attributes for this assembly as specified by type.</summary>
        /// <returns>An array that contains the custom attributes for this assembly as specified by <paramref name="attributeType" />.</returns>
        /// <param name="attributeType">The type for which the custom attributes are to be returned. </param>
        /// <param name="inherit">This argument is ignored for objects of type <see cref="T:System.Reflection.Assembly" />. </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="attributeType" /> is null. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="attributeType" /> is not a runtime type. </exception>
        public extern virtual object[] GetCustomAttributes(Type attributeType, bool inherit);

        /// <summary>Indicates whether or not a specified attribute has been applied to the assembly.</summary>
        /// <returns>true if the attribute has been applied to the assembly; otherwise, false.</returns>
        /// <param name="attributeType">The type of the attribute to be checked for this assembly. </param>
        /// <param name="inherit">This argument is ignored for objects of this type. </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="attributeType" /> is null. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="attributeType" /> uses an invalid type.</exception>
        public extern virtual bool IsDefined(Type attributeType, bool inherit);

        /// <summary>Returns information about the attributes that have been applied to the current <see cref="T:System.Reflection.Assembly" />, expressed as <see cref="T:System.Reflection.CustomAttributeData" /> objects.</summary>
        /// <returns>A generic list of <see cref="T:System.Reflection.CustomAttributeData" /> objects representing data about the attributes that have been applied to the current assembly.</returns>
        public extern virtual IList<CustomAttributeData> GetCustomAttributesData();

        /// <summary>Loads the module, internal to this assembly, with a common object file format (COFF)-based image containing an emitted module, or a resource file.</summary>
        /// <returns>The loaded module.</returns>
        /// <param name="moduleName">The name of the module. This string must correspond to a file name in this assembly's manifest. </param>
        /// <param name="rawModule">A byte array that is a COFF-based image containing an emitted module, or a resource. </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="moduleName" /> or <paramref name="rawModule" /> is null. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="moduleName" /> does not match a file entry in this assembly's manifest. </exception>
        /// <exception cref="T:System.BadImageFormatException">
        ///   <paramref name="rawModule" /> is not a valid module. </exception>
        /// <exception cref="T:System.IO.FileLoadException">A file that was found could not be loaded. </exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="ControlEvidence" />
        /// </PermissionSet>
        public extern Module LoadModule(string moduleName, byte[] rawModule);

        /// <summary>Loads the module, internal to this assembly, with a common object file format (COFF)-based image containing an emitted module, or a resource file. The raw bytes representing the symbols for the module are also loaded.</summary>
        /// <returns>The loaded module.</returns>
        /// <param name="moduleName">The name of the module. This string must correspond to a file name in this assembly's manifest. </param>
        /// <param name="rawModule">A byte array that is a COFF-based image containing an emitted module, or a resource. </param>
        /// <param name="rawSymbolStore">A byte array containing the raw bytes representing the symbols for the module. Must be null if this is a resource file. </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="moduleName" /> or <paramref name="rawModule" /> is null. </exception>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="moduleName" /> does not match a file entry in this assembly's manifest. </exception>
        /// <exception cref="T:System.BadImageFormatException">
        ///   <paramref name="rawModule" /> is not a valid module. </exception>
        /// <exception cref="T:System.IO.FileLoadException">A file that was found could not be loaded. </exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*" />
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="ControlEvidence" />
        /// </PermissionSet>
        public extern virtual Module LoadModule(string moduleName, byte[] rawModule, byte[] rawSymbolStore);

        /// <summary>Locates the specified type from this assembly and creates an instance of it using the system activator, using case-sensitive search.</summary>
        /// <returns>An instance of the specified type created with the default constructor; or null if <paramref name="typeName" /> is not found. The type is resolved using the default binder, without specifying culture or activation attributes, and with <see cref="T:System.Reflection.BindingFlags" /> set to Public or Instance. </returns>
        /// <param name="typeName">The <see cref="P:System.Type.FullName" /> of the type to locate. </param>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="typeName" /> is an empty string ("") or a string beginning with a null character.-or-The current assembly was loaded into the reflection-only context.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="typeName" /> is null. </exception>
        /// <exception cref="T:System.MissingMethodException">No matching constructor was found. </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">
        ///   <paramref name="typeName" /> requires a dependent assembly that could not be found. </exception>
        /// <exception cref="T:System.IO.FileLoadException">
        ///   <paramref name="typeName" /> requires a dependent assembly that was found but could not be loaded.-or-The current assembly was loaded into the reflection-only context, and <paramref name="typeName" /> requires a dependent assembly that was not preloaded. </exception>
        /// <exception cref="T:System.BadImageFormatException">
        ///   <paramref name="typeName" /> requires a dependent assembly, but the file is not a valid assembly. -or-<paramref name="typeName" /> requires a dependent assembly which was compiled for a version of the runtime later than the currently loaded version.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        /// </PermissionSet>
        public extern object CreateInstance(string typeName);

        /// <summary>Locates the specified type from this assembly and creates an instance of it using the system activator, with optional case-sensitive search.</summary>
        /// <returns>An instance of the specified type created with the default constructor; or null if <paramref name="typeName" /> is not found. The type is resolved using the default binder, without specifying culture or activation attributes, and with <see cref="T:System.Reflection.BindingFlags" /> set to Public or Instance.</returns>
        /// <param name="typeName">The <see cref="P:System.Type.FullName" /> of the type to locate. </param>
        /// <param name="ignoreCase">true to ignore the case of the type name; otherwise, false. </param>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="typeName" /> is an empty string ("") or a string beginning with a null character. -or-The current assembly was loaded into the reflection-only context.</exception>
        /// <exception cref="T:System.MissingMethodException">No matching constructor was found. </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="typeName" /> is null. </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">
        ///   <paramref name="typeName" /> requires a dependent assembly that could not be found. </exception>
        /// <exception cref="T:System.IO.FileLoadException">
        ///   <paramref name="typeName" /> requires a dependent assembly that was found but could not be loaded.-or-The current assembly was loaded into the reflection-only context, and <paramref name="typeName" /> requires a dependent assembly that was not preloaded. </exception>
        /// <exception cref="T:System.BadImageFormatException">
        ///   <paramref name="typeName" /> requires a dependent assembly, but the file is not a valid assembly. -or-<paramref name="typeName" /> requires a dependent assembly which was compiled for a version of the runtime later than the currently loaded version.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        /// </PermissionSet>
        public extern object CreateInstance(string typeName, bool ignoreCase);

        /// <summary>Locates the specified type from this assembly and creates an instance of it using the system activator, with optional case-sensitive search and having the specified culture, arguments, and binding and activation attributes.</summary>
        /// <returns>An instance of the specified type, or null if <paramref name="typeName" /> is not found. The supplied arguments are used to resolve the type, and to bind the constructor that is used to create the instance.</returns>
        /// <param name="typeName">The <see cref="P:System.Type.FullName" /> of the type to locate. </param>
        /// <param name="ignoreCase">true to ignore the case of the type name; otherwise, false. </param>
        /// <param name="bindingAttr">A bitmask that affects the way in which the search is conducted. The value is a combination of bit flags from <see cref="T:System.Reflection.BindingFlags" />. </param>
        /// <param name="binder">An object that enables the binding, coercion of argument types, invocation of members, and retrieval of MemberInfo objects via reflection. If <paramref name="binder" /> is null, the default binder is used. </param>
        /// <param name="args">An array that contains the arguments to be passed to the constructor. This array of arguments must match in number, order, and type the parameters of the constructor to be invoked. If the default constructor is desired, <paramref name="args" /> must be an empty array or null. </param>
        /// <param name="culture">An instance of CultureInfo used to govern the coercion of types. If this is null, the CultureInfo for the current thread is used. (This is necessary to convert a String that represents 1000 to a Double value, for example, since 1000 is represented differently by different cultures.) </param>
        /// <param name="activationAttributes">An array of one or more attributes that can participate in activation. Typically, an array that contains a single <see cref="T:System.Runtime.Remoting.Activation.UrlAttribute" /> object. The <see cref="T:System.Runtime.Remoting.Activation.UrlAttribute" /> specifies the URL that is required to activate a remote object.  </param>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="typeName" /> is an empty string ("") or a string beginning with a null character. -or-The current assembly was loaded into the reflection-only context.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="typeName" /> is null. </exception>
        /// <exception cref="T:System.MissingMethodException">No matching constructor was found. </exception>
        /// <exception cref="T:System.NotSupportedException">A non-empty activation attributes array is passed to a type that does not inherit from <see cref="T:System.MarshalByRefObject" />. </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">
        ///   <paramref name="typeName" /> requires a dependent assembly that could not be found. </exception>
        /// <exception cref="T:System.IO.FileLoadException">
        ///   <paramref name="typeName" /> requires a dependent assembly that was found but could not be loaded.-or-The current assembly was loaded into the reflection-only context, and <paramref name="typeName" /> requires a dependent assembly that was not preloaded. </exception>
        /// <exception cref="T:System.BadImageFormatException">
        ///   <paramref name="typeName" /> requires a dependent assembly, but the file is not a valid assembly. -or-<paramref name="typeName" /> requires a dependent assembly which was compiled for a version of the runtime later than the currently loaded version.</exception>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        /// </PermissionSet>
        public extern virtual object CreateInstance(string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes);

        /// <summary>Gets all the loaded modules that are part of this assembly.</summary>
        /// <returns>An array of modules.</returns>
        public extern Module[] GetLoadedModules();

        /// <summary>Gets all the loaded modules that are part of this assembly, specifying whether to include resource modules.</summary>
        /// <returns>An array of modules.</returns>
        /// <param name="getResourceModules">true to include resource modules; otherwise, false. </param>
        public extern virtual Module[] GetLoadedModules(bool getResourceModules);

        /// <summary>Gets all the modules that are part of this assembly.</summary>
        /// <returns>An array of modules.</returns>
        /// <exception cref="T:System.IO.FileNotFoundException">The module to be loaded does not specify a file name extension. </exception>
        public extern Module[] GetModules();

        /// <summary>Gets all the modules that are part of this assembly, specifying whether to include resource modules.</summary>
        /// <returns>An array of modules.</returns>
        /// <param name="getResourceModules">true to include resource modules; otherwise, false. </param>
        public extern virtual Module[] GetModules(bool getResourceModules);

        /// <summary>Gets the specified module in this assembly.</summary>
        /// <returns>The module being requested, or null if the module is not found.</returns>
        /// <param name="name">The name of the module being requested. </param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="name" /> parameter is null. </exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="name" /> parameter is an empty string (""). </exception>
        /// <exception cref="T:System.IO.FileLoadException">A file that was found could not be loaded. </exception>
        /// <exception cref="T:System.IO.FileNotFoundException">
        ///   <paramref name="name" /> was not found. </exception>
        /// <exception cref="T:System.BadImageFormatException">
        ///   <paramref name="name" /> is not a valid assembly. </exception>
        public extern virtual Module GetModule(string name);

        /// <summary>Returns the names of all the resources in this assembly.</summary>
        /// <returns>An array that contains the names of all the resources.</returns>
        public extern virtual string[] GetManifestResourceNames();

        /// <summary>Gets the <see cref="T:System.Reflection.AssemblyName" /> objects for all the assemblies referenced by this assembly.</summary>
        /// <returns>An array that contains the fully parsed display names of all the assemblies referenced by this assembly.</returns>
        public extern virtual AssemblyName[] GetReferencedAssemblies();
    }
}
