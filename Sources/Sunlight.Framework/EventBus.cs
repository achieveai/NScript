//-----------------------------------------------------------------------
// <copyright file="EventBus.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for EventBus
    /// </summary>
    public class EventBus
    {
        StringDictionary<Delegate> eventSubscriptsion = new  StringDictionary<Delegate>();
        Dictionary oneTimeValues = new Dictionary();

        public void Subscribe<T>(Action<T> callback)
        {
            ExceptionHelpers.ThrowOnArgumentNull(callback, "callback");

            Delegate registeredCallback;
            string typeId = typeof(T).TypeId;

            object evt = this.oneTimeValues[typeId];
            if (!object.IsNullOrUndefined(evt))
            {
                callback((T)evt);
            }

            if (!eventSubscriptsion.TryGetValue(typeId, out registeredCallback))
            { eventSubscriptsion[typeId] = callback; }
            else
            { eventSubscriptsion[typeId] = Delegate.Combine(registeredCallback, (Delegate)callback); }
        }

        public void UnSubscribe<T>(Action<T> callback)
        {
            ExceptionHelpers.ThrowOnArgumentNull(callback, "callback");

            Delegate registeredCallback;
            string typeId = typeof(T).TypeId;

            if (eventSubscriptsion.TryGetValue(typeId, out registeredCallback))
            {
                Action<T> act = ((Action<T>)registeredCallback);
                act -= callback;

                if (act == null)
                { eventSubscriptsion.Remove(typeId); }
                else
                { eventSubscriptsion[typeId] = act; }
            }
        }

        public void Raise<T>(T evt)
        {
            ExceptionHelpers.ThrowOnArgumentNull(evt, "evt");

            Delegate registeredCallback;
            Type type = typeof(T);

            do
            {
                string typeId = type.TypeId;
                if (eventSubscriptsion.TryGetValue(typeId, out registeredCallback))
                {
                    ((Action<T>)registeredCallback)(evt);
                }

                type = type.BaseType;
            } while (type != null);
        }

        public void RaiseOneTime<T>(T evt)
        {
            ExceptionHelpers.ThrowOnArgumentNull(evt, "evt");

            string typeId = typeof(T).TypeId;
            var alreadyRegistered = this.oneTimeValues[typeId];

            if (!Object.IsNullOrUndefined(alreadyRegistered))
            {
                throw new Exception("Event " + typeof(T).Name + " already raised");
            }

            this.oneTimeValues[typeId] = evt;
            this.Raise(evt);
            this.eventSubscriptsion.Remove(typeId);
        }
    }
}
