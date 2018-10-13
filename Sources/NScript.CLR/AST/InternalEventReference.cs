//-----------------------------------------------------------------------
// <copyright file="InternalPropertyRefernce.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST
{
    using System;
    using Mono.Cecil;

    public class InternalEventReference : EventReference
    {
        private readonly MethodReference relatedMethod;
        private EventDefinition eventDefinition;

        public InternalEventReference(
            MethodReference addOn,
            MethodReference removeOn)
            : base(InternalEventReference.GetEventName(addOn, removeOn),
                InternalEventReference.GetEventType(addOn, removeOn))
        {
            base.DeclaringType = addOn != null
                ? addOn.DeclaringType
                : removeOn.DeclaringType;

            this.relatedMethod = addOn ?? removeOn;
        }

        /// <summary>
        /// Resolves this instance.
        /// </summary>
        /// <returns>PropertyDefinition</returns>
        public override EventDefinition Resolve()
        {
            if (this.eventDefinition == null)
            { this.eventDefinition = this.relatedMethod.Resolve().GetEventDefinition(); }

            return this.eventDefinition;
        }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        public override string FullName
        {
            get
            {
                return this.DeclaringType.FullName + "." + this.Name;
            }
        }

        private static TypeReference GetEventType(
            MethodReference add,
            MethodReference remove)
        {
            if (add != null)
            {
                return add.Parameters[0].ParameterType;
            }
            else
            {
                return remove.Parameters[0].ParameterType;
            }
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <param name="add">The getter.</param>
        /// <param name="remove">The setter.</param>
        /// <returns>Name of the property</returns>
        private static string GetEventName(
            MethodReference add,
            MethodReference remove)
        {
            if (add != null)
            {
                if (!add.Resolve().IsAddOn)
                {
                    throw new InvalidProgramException("getter is not a getter");
                }

                return add.Resolve().GetEventDefinition().Name;
            }

            if (!remove.Resolve().IsRemoveOn)
            {
                throw new InvalidProgramException("Setter is not a setter");
            }

            return remove.Resolve().GetEventDefinition().Name;
        }
    }
}
