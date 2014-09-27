//-----------------------------------------------------------------------
// <copyright file="EventInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Reflection
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>Discovers the attributes of an event and provides access to event metadata.</summary>
    [Serializable]
    public abstract class EventInfo : MemberInfo
    {
        /// <summary>Gets a <see cref="T:System.Reflection.MemberTypes" /> value indicating that this member is an event.</summary>
        /// <returns>A <see cref="T:System.Reflection.MemberTypes" /> value indicating that this member is an event.</returns>
        public override MemberTypes MemberType
        {
            get
            {
                return MemberTypes.Event;
            }
        }
        /// <summary>Gets the attributes for this event.</summary>
        /// <returns>The read-only attributes for this event.</returns>
        public abstract EventAttributes Attributes
        {
            get;
        }

        public extern virtual MethodInfo AddMethod
        {
            get;
        }


        public extern virtual MethodInfo RemoveMethod
        {
            get;
        }

        public extern virtual MethodInfo RaiseMethod
        {
            get;
        }

        /// <summary>Gets the Type object of the underlying event-handler delegate associated with this event.</summary>
        /// <returns>A read-only Type object representing the delegate event handler.</returns>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public extern virtual Type EventHandlerType
        {
            get;
        }

        /// <summary>Gets a value indicating whether the EventInfo has a name with a special meaning.</summary>
        /// <returns>true if this event has a special name; otherwise, false.</returns>
        public extern bool IsSpecialName
        {
            get;
        }

        /// <summary>Gets a value indicating whether the event is multicast.</summary>
        /// <returns>true if the delegate is an instance of a multicast delegate; otherwise, false.</returns>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public extern virtual bool IsMulticast
        {
            get;
        }

        /// <summary>Initializes a new instance of the EventInfo class.</summary>
        protected EventInfo()
        {
        }

        /// <summary>Indicates whether two <see cref="T:System.Reflection.EventInfo" /> objects are equal.</summary>
        /// <returns>true if <paramref name="left" /> is equal to <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        public extern static bool operator ==(EventInfo left, EventInfo right);

        /// <summary>Indicates whether two <see cref="T:System.Reflection.EventInfo" /> objects are not equal.</summary>
        /// <returns>true if <paramref name="left" /> is not equal to <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        public extern static bool operator !=(EventInfo left, EventInfo right);

        /// <summary>Returns the methods that have been associated with the event in metadata using the .other directive, specifying whether to include non-public methods.</summary>
        /// <returns>An array of <see cref="T:System.Reflection.EventInfo" /> objects representing methods that have been associated with an event in metadata by using the .other directive. If there are no methods matching the specification, an empty array is returned.</returns>
        /// <param name="nonPublic">true to include non-public methods; otherwise, false.</param>
        /// <exception cref="T:System.NotImplementedException">This method is not implemented.</exception>
        public extern virtual MethodInfo[] GetOtherMethods(bool nonPublic);

        /// <summary>When overridden in a derived class, retrieves the MethodInfo object for the <see cref="M:System.Reflection.EventInfo.AddEventHandler(System.Object,System.Delegate)" /> method of the event, specifying whether to return non-public methods.</summary>
        /// <returns>A <see cref="T:System.Reflection.MethodInfo" /> object representing the method used to add an event handler delegate to the event source.</returns>
        /// <param name="nonPublic">true if non-public methods can be returned; otherwise, false. </param>
        /// <exception cref="T:System.MethodAccessException">
        ///   <paramref name="nonPublic" /> is true, the method used to add an event handler delegate is non-public, and the caller does not have permission to reflect on non-public methods. </exception>
        public abstract MethodInfo GetAddMethod(bool nonPublic);

        /// <summary>When overridden in a derived class, retrieves the MethodInfo object for removing a method of the event, specifying whether to return non-public methods.</summary>
        /// <returns>A <see cref="T:System.Reflection.MethodInfo" /> object representing the method used to remove an event handler delegate from the event source.</returns>
        /// <param name="nonPublic">true if non-public methods can be returned; otherwise, false. </param>
        /// <exception cref="T:System.MethodAccessException">
        ///   <paramref name="nonPublic" /> is true, the method used to add an event handler delegate is non-public, and the caller does not have permission to reflect on non-public methods. </exception>
        public abstract MethodInfo GetRemoveMethod(bool nonPublic);

        /// <summary>When overridden in a derived class, returns the method that is called when the event is raised, specifying whether to return non-public methods.</summary>
        /// <returns>A MethodInfo object that was called when the event was raised.</returns>
        /// <param name="nonPublic">true if non-public methods can be returned; otherwise, false. </param>
        /// <exception cref="T:System.MethodAccessException">
        ///   <paramref name="nonPublic" /> is true, the method used to add an event handler delegate is non-public, and the caller does not have permission to reflect on non-public methods. </exception>
        public abstract MethodInfo GetRaiseMethod(bool nonPublic);

        /// <summary>Returns the public methods that have been associated with an event in metadata using the .other directive.</summary>
        /// <returns>An array of <see cref="T:System.Reflection.EventInfo" /> objects representing the public methods that have been associated with the event in metadata by using the .other directive. If there are no such public methods, an empty array is returned.</returns>
        public extern MethodInfo[] GetOtherMethods();

        /// <summary>Returns the method used to add an event handler delegate to the event source.</summary>
        /// <returns>A <see cref="T:System.Reflection.MethodInfo" /> object representing the method used to add an event handler delegate to the event source.</returns>
        public extern MethodInfo GetAddMethod();

        /// <summary>Returns the method used to remove an event handler delegate from the event source.</summary>
        /// <returns>A <see cref="T:System.Reflection.MethodInfo" /> object representing the method used to remove an event handler delegate from the event source.</returns>
        public extern MethodInfo GetRemoveMethod();

        /// <summary>Returns the method that is called when the event is raised.</summary>
        /// <returns>The method that is called when the event is raised.</returns>
        public extern MethodInfo GetRaiseMethod();

        /// <summary>Adds an event handler to an event source.</summary>
        /// <param name="target">The event source. </param>
        /// <param name="handler">Encapsulates a method or methods to be invoked when the event is raised by the target. </param>
        /// <exception cref="T:System.InvalidOperationException">The event does not have a public add accessor.</exception>
        /// <exception cref="T:System.ArgumentException">The handler that was passed in cannot be used. </exception>
        /// <exception cref="T:System.MethodAccessException">The caller does not have access permission to the member. </exception>
        /// <exception cref="T:System.Reflection.TargetException">The <paramref name="target" /> parameter is null and the event is not static.-or- The <see cref="T:System.Reflection.EventInfo" /> is not declared on the target. </exception>
        public extern virtual void AddEventHandler(object target, Delegate handler);

        /// <summary>Removes an event handler from an event source.</summary>
        /// <param name="target">The event source. </param>
        /// <param name="handler">The delegate to be disassociated from the events raised by target. </param>
        /// <exception cref="T:System.InvalidOperationException">The event does not have a public remove accessor. </exception>
        /// <exception cref="T:System.ArgumentException">The handler that was passed in cannot be used. </exception>
        /// <exception cref="T:System.Reflection.TargetException">The <paramref name="target" /> parameter is null and the event is not static.-or- The <see cref="T:System.Reflection.EventInfo" /> is not declared on the target. </exception>
        /// <exception cref="T:System.MethodAccessException">The caller does not have access permission to the member. </exception>
        public extern virtual void RemoveEventHandler(object target, Delegate handler);
    }
}
