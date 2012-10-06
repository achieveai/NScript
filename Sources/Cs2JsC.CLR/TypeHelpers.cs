//-----------------------------------------------------------------------
// <copyright file="TypeHelpers.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Mono.Cecil;

    /// <summary>
    /// Definition for TypeHelpers
    /// </summary>
    public static class TypeHelpers
    {
        private const string DelegateStr = "System.Delegate";
        private const string MulticastDelegateStr = "System.MulticastDelegate";
        private const string SystemNamespaceStr = "System";

        /// <summary>
        /// Backing store for reusable emty typeReferences.
        /// </summary>
        private static readonly TypeReference[] emtyTypeReferences = new TypeReference[0];

        /// <summary>
        /// Determines whether the specified left is same.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// <c>true</c> if the specified left is same; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSame(this MemberReference left, MemberReference right)
        {
            TypeReference leftType = left as TypeReference;

            if (leftType != null)
            {
                TypeReference rightType = right as TypeReference;

                if (rightType == null)
                { return false; }

                return leftType.IsSame(rightType);
            }

            MethodReference leftMethod = left as MethodReference;
            if (leftMethod != null)
            {
                MethodReference rightMethod = right as MethodReference;

                if (rightMethod == null)
                { return false; }

                return leftMethod.IsSame(rightMethod);
            }

            FieldReference leftField = left as FieldReference;
            if (leftField != null)
            {
                FieldReference rightField = right as FieldReference;

                if (rightField == null)
                { return false; }

                return leftField.IsSame(rightField);
            }

            EventReference leftEvent = left as EventReference;
            if (leftEvent != null)
            {
                EventReference rightEvent = right as EventReference;

                if (rightEvent == null)
                { return false; }

                return leftEvent.IsSame(rightEvent);
            }

            PropertyReference leftProp = left as PropertyReference;
            if (leftProp != null)
            {
                PropertyReference rightProp = right as PropertyReference;

                if (rightProp == null)
                { return false; }

                return leftProp.IsSame(rightProp);
            }

            throw new NotSupportedException();
        }

        /// <summary>
        /// Determines whether the specified left is same.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// <c>true</c> if the specified left is same; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSame(this FieldReference left, FieldReference right)
        {
            return left.FullName == right.FullName
                && left.DeclaringType.IsSame(right.DeclaringType);
        }

        /// <summary>
        /// Determines whether the specified left is same.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// <c>true</c> if the specified left is same; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSame(this PropertyReference left, PropertyReference right)
        {
            if (left.Name != right.Name
                || left.Parameters.Count != right.Parameters.Count
                || !left.DeclaringType.IsSame(right.DeclaringType))
            { return false; }

            for (int iParam = 0; iParam < left.Parameters.Count; iParam++)
            {
                if (!left.Parameters[iParam].IsSame(right.Parameters[iParam]))
                { return false; }
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified left is same.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// <c>true</c> if the specified left is same; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSame(this EventReference left, EventReference right)
        {
            if (left == right)
            { return true; }

            return false;
        }

        /// <summary>
        /// Determines whether the specified left is same.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// <c>true</c> if the specified left is same; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSame(this ParameterDefinition left, ParameterDefinition right)
        {
            return TypeHelpers.IsSame(left, right, new List<KeyValuePair<IGenericParameterProvider, IGenericParameterProvider>>());
        }

        /// <summary>
        /// Determines whether the specified left is same.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// <c>true</c> if the specified left is same; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsSame(
            ParameterDefinition left,
            ParameterDefinition right,
            List<KeyValuePair<IGenericParameterProvider, IGenericParameterProvider>> stack)
        {
            if (left.IsOut != right.IsOut)
            { return false; }

            if (!TypeHelpers.IsSame(left.ParameterType, right.ParameterType, stack))
            { return false; }

            return true;
        }

        /// <summary>
        /// Determines whether the specified left is same.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// <c>true</c> if the specified left is same; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSame(this MethodReference left, MethodReference right)
        {
            return TypeHelpers.IsSame(left, right, new List<KeyValuePair<IGenericParameterProvider, IGenericParameterProvider>>());
        }

        /// <summary>
        /// Determines whether the specified left is same.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// <c>true</c> if the specified left is same; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsSame(
            MethodReference left,
            MethodReference right,
            List<KeyValuePair<IGenericParameterProvider, IGenericParameterProvider>> stack)
        {
            if (left == right)
            { return true; }

            stack.Add(
                new KeyValuePair<IGenericParameterProvider, IGenericParameterProvider>(
                    left,
                    right));

            if (left.Name == right.Name
                && left.CallingConvention == right.CallingConvention
                && left.HasParameters == right.HasParameters
                && left.IsGenericInstance == right.IsGenericInstance
                && (!left.HasParameters || left.Parameters.Count == right.Parameters.Count)
                && TypeHelpers.IsSame(
                    left.DeclaringType,
                    right.DeclaringType,
                    stack))
            {
                if (left.IsGenericInstance)
                {
                    GenericInstanceMethod leftGeneric = left as GenericInstanceMethod;
                    GenericInstanceMethod rightGeneric = right as GenericInstanceMethod;

                    if (leftGeneric.GenericArguments.Count != rightGeneric.GenericArguments.Count)
                    {
                        stack.RemoveAt(stack.Count - 1);
                        return false;
                    }

                    for (int iGeneric = 0; iGeneric < leftGeneric.GenericArguments.Count; iGeneric++)
                    {
                        if (!TypeHelpers.IsSame(
                            leftGeneric.GenericArguments[iGeneric],
                            rightGeneric.GenericArguments[iGeneric],
                            stack))
                        {
                            stack.RemoveAt(stack.Count - 1);
                            return false;
                        }
                    }
                }

                if (left.HasParameters)
                {
                    for (int iParam = 0; iParam < left.Parameters.Count; iParam++)
                    {
                        if (!TypeHelpers.IsSame(
                            left.Parameters[iParam],
                            right.Parameters[iParam],
                            stack))
                        {
                            stack.RemoveAt(stack.Count - 1);
                            return false;
                        }
                    }
                }

                stack.RemoveAt(stack.Count - 1);
                return true;
            }

            stack.RemoveAt(stack.Count - 1);
            return false;
        }

        /// <summary>
        /// Determines whether the specified right Type is same as left Type.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// true if both the typeReferences are same.
        /// </returns>
        public static bool IsSame(this TypeReference left, TypeReference right)
        {
            return TypeHelpers.IsSame(left, right, new List<KeyValuePair<IGenericParameterProvider, IGenericParameterProvider>>());
        }

        /// <summary>
        /// Determines whether the specified right Type is same as left Type.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// true if both the typeReferences are same.
        /// </returns>
        private static bool IsSame(
            TypeReference left,
            TypeReference right,
            List<KeyValuePair<IGenericParameterProvider, IGenericParameterProvider>> stack)
        {
            if (left == right)
            { return true; }

            if (!(left is TypeSpecification)
                && !(left is GenericParameter))
            {
                return TypeHelpers.IsSameDefinition(left, right, stack);
            }

            if (left.GetType() != right.GetType())
            { return false; }

            stack.Add(
                new KeyValuePair<IGenericParameterProvider, IGenericParameterProvider>(
                    left,
                    right));

            bool rv = false;

            if (left.IsArray)
            {
                ArrayType leftAsArray = left as ArrayType;
                ArrayType rightAsArray = right as ArrayType;

                rv = leftAsArray.Dimensions.Count == rightAsArray.Dimensions.Count
                    && TypeHelpers.IsSame(
                        leftAsArray.ElementType,
                        rightAsArray.ElementType,
                        stack);
                stack.RemoveAt(stack.Count - 1);

                return rv;
            }
            else if (left.IsGenericInstance)
            {
                GenericInstanceType leftGeneric = (GenericInstanceType)left;
                GenericInstanceType rightGeneric = (GenericInstanceType)right;

                if (leftGeneric.GenericArguments.Count != rightGeneric.GenericArguments.Count)
                { return false; }

                for (int genericIndex = 0; genericIndex < leftGeneric.GenericArguments.Count; genericIndex++)
                {
                    if (!TypeHelpers.IsSame(
                        leftGeneric.GenericArguments[genericIndex],
                        rightGeneric.GenericArguments[genericIndex],
                        stack))
                    {
                        stack.RemoveAt(stack.Count - 1);
                        return false;
                    }
                }

                stack.RemoveAt(stack.Count - 1);
                return true;
            }
            else if (left.IsGenericParameter)
            {
                GenericParameter leftGenericParameter = (GenericParameter)left;
                GenericParameter rightGenericParameter = (GenericParameter)right;

                if (leftGenericParameter.Position == rightGenericParameter.Position)
                {
                    if (leftGenericParameter.Owner == null || rightGenericParameter.Owner == null)
                    {
                        return true;
                    }
                    else if (leftGenericParameter.Owner.GenericParameterType == rightGenericParameter.Owner.GenericParameterType)
                    {
                        for (int iStack = stack.Count - 1; iStack >= 0; iStack--)
                        {
                            if (leftGenericParameter.Owner == stack[iStack].Key)
                            {
                                stack.RemoveAt(stack.Count - 1);
                                return rightGenericParameter.Owner == stack[iStack].Value;
                            }
                        }

                        if (leftGenericParameter.Owner.GenericParameterType == GenericParameterType.Method)
                        {
                            rv = TypeHelpers.IsSame(
                                (MethodReference)leftGenericParameter.Owner,
                                (MethodReference)rightGenericParameter.Owner,
                                stack);
                        }
                        else
                        {
                            rv = TypeHelpers.IsSame(
                                (TypeReference)leftGenericParameter.Owner,
                                (TypeReference)rightGenericParameter.Owner,
                                stack);
                        }
                    }
                }

                stack.RemoveAt(stack.Count - 1);
                return rv;
            }
            else if (left.IsByReference
                || left.IsOptionalModifier
                || left.IsPointer
                || left.IsRequiredModifier)
            {
                rv = TypeHelpers.IsSame(
                    left.GetElementType(),
                    right.GetElementType(),
                    stack);

                stack.RemoveAt(stack.Count - 1);
                return rv;
            }
            else
            {
                throw new NotSupportedException();
            }

            /*
             * Possibly simpler solution.
            if (left == right)
            { return true; }

            if (left == null || right == null)
            { return false; }

            return left.FullName == right.FullName; // TODO: implement this more efficiently?
             */
        }

        /// <summary>
        /// Determines whether both leftReference and rightReference point to same definition.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// <c>true</c> if both leftReference and rightReference point to same definition; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSameDefinition(
            this MemberReference left,
            MemberReference right)
        {
            if (left == right)
            { return true; }

            TypeReference leftType = left as TypeReference;

            if (leftType != null)
            {
                TypeReference rightType = right as TypeReference;

                if (rightType == null)
                { return false; }

                return leftType.IsSameDefinition(rightType);
            }

            MethodReference leftMethod = left as MethodReference;
            if (leftMethod != null)
            {
                MethodReference rightMethod = right as MethodReference;

                if (rightMethod == null)
                { return false; }

                return leftMethod.IsSameDefinition(rightMethod);
            }

            FieldReference leftField = left as FieldReference;
            if (leftField != null)
            {
                FieldReference rightField = right as FieldReference;

                if (rightField == null)
                { return false; }

                return leftField.IsSameDefinition(rightField);
            }

            EventReference leftEvent = left as EventReference;
            if (leftEvent != null)
            {
                EventReference rightEvent = right as EventReference;

                if (rightEvent == null)
                { return false; }

                return leftEvent.IsSameDefinition(rightEvent);
            }

            PropertyReference leftProp = left as PropertyReference;
            if (leftProp != null)
            {
                PropertyReference rightProp = right as PropertyReference;

                if (rightProp == null)
                { return false; }

                return leftProp.IsSameDefinition(rightProp);
            }

            throw new NotSupportedException();
        }

        /// <summary>
        /// Determines whether left field definition is same as right field definition.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// <c>true</c> if the specified left is same definition; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSameDefinition(
            this FieldReference left,
            FieldReference right)
        {
            if (left == right)
            { return true; }

            if (!left.DeclaringType.IsSameDefinition(right.DeclaringType.DeclaringType))
            { return false; }

            if (left.Name != right.Name)
            { return false; }

            return true;
        }

        /// <summary>
        /// Determines whether left event definition is same as event definition.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// <c>true</c> if the specified left is same definition; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSameDefinition(
            this EventReference left,
            EventReference right)
        {
            if (left == right)
            { return true; }

            if (!left.DeclaringType.IsSameDefinition(right.DeclaringType.DeclaringType))
            { return false; }

            if (left.Name != right.Name)
            { return false; }

            return true;
        }

        /// <summary>
        /// Determines whether left property definition is same as right property definition.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// <c>true</c> if the specified left is same definition; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSameDefinition(
            this PropertyReference left,
            PropertyReference right)
        {
            if (left == right)
            { return true; }

            if (!left.DeclaringType.IsSameDefinition(right.DeclaringType.DeclaringType))
            { return false; }

            if (left.Name != right.Name
                || left.Parameters.Count != right.Parameters.Count)
            { return false; }

            PropertyDefinition leftDef = left.Resolve();
            PropertyDefinition rightDef = right.Resolve();

            for (int iParam = 0; iParam < leftDef.Parameters.Count; iParam++)
            {
                if (!leftDef.Parameters[iParam].IsSame(rightDef.Parameters[iParam]))
                { return false; }
            }

            return true;
        }

        /// <summary>
        /// Determines whether left method definition is same as right method definition.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// <c>true</c> if the specified left is same definition; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSameDefinition(
            this MethodReference left,
            MethodReference right)
        {
            return TypeHelpers.IsSameDefinition(left, right, new List<KeyValuePair<IGenericParameterProvider, IGenericParameterProvider>>());
        }

        /// <summary>
        /// Determines whether left method definition is same as right method definition.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// <c>true</c> if the specified left is same definition; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsSameDefinition(
            MethodReference left,
            MethodReference right,
            List<KeyValuePair<IGenericParameterProvider, IGenericParameterProvider>> stack)
        {
            if (left == right)
            { return true; }

            if (!TypeHelpers.IsSameDefinition(
                    left.DeclaringType,
                    right.DeclaringType,
                    stack))
            { return false; }

            if (left.Name != right.Name
                || left.Parameters.Count != right.Parameters.Count)
            { return false; }

            MethodDefinition leftDef = left.Resolve();
            MethodDefinition rightDef = right.Resolve();

            stack.Add(
                new KeyValuePair<IGenericParameterProvider, IGenericParameterProvider>(
                    leftDef,
                    rightDef));

            for (int iParam = 0; iParam < leftDef.Parameters.Count; iParam++)
            {
                if (!TypeHelpers.IsSame(
                    leftDef.Parameters[iParam],
                    rightDef.Parameters[iParam],
                    stack))
                {
                    stack.RemoveAt(stack.Count - 1);
                    return false;
                }
            }

            if (!TypeHelpers.IsSame(leftDef.ReturnType, rightDef.ReturnType, stack))
            {
                stack.RemoveAt(stack.Count - 1);
                return false;
            }

            stack.RemoveAt(stack.Count - 1);
            return true;
        }

        /// <summary>
        /// Definitions the equals.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>true if both the typeReferences point to same definition.</returns>
        public static bool IsSameDefinition(
            this TypeReference left,
            TypeReference right)
        {
            return TypeHelpers.IsSameDefinition(left, right, new List<KeyValuePair<IGenericParameterProvider, IGenericParameterProvider>>());
        }

        /// <summary>
        /// Definitions the equals.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>true if both the typeReferences point to same definition.</returns>
        private static bool IsSameDefinition(
            TypeReference left,
            TypeReference right,
            List<KeyValuePair<IGenericParameterProvider, IGenericParameterProvider>> stack)
        {
            if (left == right)
            { return true; }

            GenericParameter leftParam = left as GenericParameter;
            bool rv = false;

            if (leftParam != null)
            {
                stack.Add(
                    new KeyValuePair<IGenericParameterProvider, IGenericParameterProvider>(
                        left,
                        right));
                GenericParameter rightParam = right as GenericParameter;

                if (rightParam == null
                    || leftParam.Owner.GenericParameterType != rightParam.Owner.GenericParameterType
                    || leftParam.Position != rightParam.Position)
                {
                    rv = false;
                }
                else if (leftParam.Owner.GenericParameterType == GenericParameterType.Type)
                {
                    rv = TypeHelpers.IsSameDefinition(
                        (TypeReference)leftParam.Owner,
                        (TypeReference)rightParam.Owner,
                        stack);
                }
                else
                {
                    rv = TypeHelpers.IsSameDefinition(
                        (MethodReference)leftParam.Owner,
                        (MethodReference)rightParam.Owner,
                        stack);
                }

                stack.RemoveAt(stack.Count - 1);
                return rv;
            }

            if (left == null
                || right == null
                || left.Namespace != right.Namespace
                || left.Name != right.Name
                || left.Scope.GetName() != right.Scope.GetName())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the definition.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <returns>Reference</returns>
        public static IMemberDefinition GetDefinition(
            this MemberReference reference)
        {
            if (reference.IsDefinition)
            {
                return (IMemberDefinition)reference;
            }

            FieldReference fieldRef = reference as FieldReference;
            if (fieldRef != null)
            { return fieldRef.Resolve(); }

            PropertyReference propRef = reference as PropertyReference;
            if (propRef != null)
            { return propRef.Resolve(); }

            MethodReference methodRef = reference as MethodReference;
            if (methodRef != null)
            { return methodRef.Resolve(); }

            TypeReference typeRef = reference as TypeReference;
            if (typeRef != null)
            { return typeRef.Resolve(); }

            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the generic type scope.
        /// </summary>
        /// <param name="paramDef">The type reference.</param>
        /// <returns>Smallest type scope for this type.</returns>
        public static GenericParameterType? GetGenericTypeScope(this TypeReference typeReference)
        {
            GenericParameterType? rv = null;
            GenericParameter genericParameter = typeReference as GenericParameter;
            if (genericParameter != null)
            {
                return genericParameter.Type;
            }

            GenericInstanceType genericInstance = typeReference as GenericInstanceType;
            if (genericInstance != null)
            {
                foreach (var genericParam in genericInstance.GenericArguments)
                {
                    GenericParameterType? tmpRv = genericParam.GetGenericTypeScope();
                    if (tmpRv.HasValue)
                    {
                        // Method is most restrictive scope, so we return as soon as we find method.
                        if (tmpRv.Value == GenericParameterType.Method)
                        {
                            return tmpRv;
                        }

                        rv = tmpRv;
                    }
                }

                return rv;
            }

            TypeSpecification typeSpec = typeReference as TypeSpecification;
            if (typeSpec != null)
            {
                return typeSpec.ElementType.GetGenericTypeScope();
            }

            return null;
        }

        /// <summary>
        /// Fixes the generic type arguments.
        /// </summary>
        /// <param name="paramDef">The type reference.</param>
        /// <param name="memberReference">The member reference.</param>
        /// <returns>Type reference arguments that now point to new memberReference.</returns>
        public static TypeReference FixGenericTypeArguments(
                this TypeReference typeReference,
                MethodReference memberReference)
        {
            IList<TypeReference> typeArguments;
            IList<TypeReference> methodArguments = null;
            if (memberReference.DeclaringType is ArrayType)
            {
                typeArguments = new List<TypeReference>();
                typeArguments.Add(((ArrayType)memberReference.DeclaringType).ElementType);
            }
            else
            {
                typeArguments = ((GenericInstanceType)memberReference.DeclaringType).GenericArguments;
            }

            if (memberReference is GenericInstanceMethod)
            {
                methodArguments = ((GenericInstanceMethod)memberReference).GenericArguments;
            }

            return typeReference.FixGenericTypeArguments(typeArguments, methodArguments);
        }

        /// <summary>
        /// Fixes the generic type arguments.
        /// </summary>
        /// <param name="paramDef">The type reference.</param>
        /// <param name="genericTypeReferene">The generic type referene.</param>
        /// <returns>Type reference arguments that now point to new paramDef after types being replaced.</returns>
        public static TypeReference FixGenericTypeArguments(
            this TypeReference typeReference,
            TypeReference genericTypeReferene)
        {
            if (typeReference.IsSame(genericTypeReferene))
            { return typeReference; }

            if (genericTypeReferene.IsGenericInstance)
            {
                return typeReference.FixGenericTypeArguments(
                    ((GenericInstanceType)genericTypeReferene).GenericArguments,
                    null);
            }

            if (genericTypeReferene.IsArray)
            {
                return typeReference.FixGenericTypeArguments(
                    new TypeReference[] { ((ArrayType)genericTypeReferene).ElementType },
                    null);
            }

            return typeReference;
        }

        /// <summary>
        /// Fixes the generic type arguments.
        /// </summary>
        /// <param name="paramDef">The type reference.</param>
        /// <param name="memberReference">The member reference.</param>
        /// <returns>Type reference arguments that now point to new memberReference.</returns>
        public static TypeReference FixGenericTypeArguments(
            this TypeReference typeReference,
            IList<TypeReference> typeArguments,
            IList<TypeReference> methodArguments)
        {
            if (typeReference is TypeSpecification)
            {
                ArrayType arrayType = typeReference as ArrayType;
                if (arrayType != null)
                {
                    TypeReference elementType =
                            arrayType.ElementType.FixGenericTypeArguments(
                                typeArguments,
                                methodArguments);

                    if (elementType != arrayType.ElementType)
                    {
                        ArrayType newArrayType = new ArrayType(elementType);
                        newArrayType.Dimensions.Clear(); // remove the single dimension that Cecil adds by default

                        foreach (ArrayDimension d in arrayType.Dimensions)
                        { newArrayType.Dimensions.Add(d); }

                        return newArrayType;
                    }
                    else
                    {
                        return typeReference;
                    }
                }

                ByReferenceType refType = typeReference as ByReferenceType;
                if (refType != null)
                {
                    TypeReference elementType = refType.ElementType.FixGenericTypeArguments(typeArguments, methodArguments);
                    return elementType != refType.ElementType ? new ByReferenceType(elementType) : typeReference;
                }

                GenericInstanceType giType = typeReference as GenericInstanceType;
                if (giType != null)
                {
                    GenericInstanceType newType = new GenericInstanceType(giType.ElementType);
                    bool isChanged = false;
                    for (int i = 0; i < giType.GenericArguments.Count; i++)
                    {
                        newType.GenericArguments.Add(
                            giType.GenericArguments[i].FixGenericTypeArguments(
                                typeArguments,
                                methodArguments));

                        isChanged |= newType.GenericArguments[i] != giType.GenericArguments[i];
                    }

                    return isChanged ? newType : typeReference;
                }

                OptionalModifierType optmodType = typeReference as OptionalModifierType;
                if (optmodType != null)
                {
                    TypeReference elementType = optmodType.ElementType.FixGenericTypeArguments(
                        typeArguments,
                        methodArguments);

                    return elementType != optmodType.ElementType
                        ? new OptionalModifierType(optmodType.ModifierType, elementType)
                        : typeReference;
                }

                RequiredModifierType reqmodType = typeReference as RequiredModifierType;
                if (reqmodType != null)
                {
                    TypeReference elementType = reqmodType.ElementType.FixGenericTypeArguments(
                        typeArguments,
                        methodArguments);

                    return elementType != reqmodType.ElementType
                        ? new RequiredModifierType(reqmodType.ModifierType, elementType)
                        : typeReference;
                }

                PointerType ptrType = typeReference as PointerType;
                if (ptrType != null)
                {
                    TypeReference elementType = ptrType.ElementType.FixGenericTypeArguments(
                        typeArguments,
                        methodArguments);

                    return elementType != ptrType.ElementType
                        ? new PointerType(elementType)
                        : typeReference;
                }
            }

            GenericParameter gp = typeReference as GenericParameter;
            if (gp != null)
            {
                if (gp.Owner.GenericParameterType == GenericParameterType.Method)
                {
                    if (methodArguments != null)
                    {
                        return methodArguments[gp.Position];
                    }
                }
                else
                {
                    if (typeArguments != null)
                    {
                        return typeArguments[gp.Position];
                    }
                }
            }

            return typeReference;
        }

        /// <summary>
        /// Determines whether the specified method is overridable.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="baseMethod">The base method.</param>
        /// <param name="baseType">Type of the base.</param>
        /// <returns>MethodRefernce if method overrides baseMethod.</returns>
        public static MethodReference IsOverridable(
            this MethodDefinition method,
            MethodReference baseMethod,
            TypeReference baseType)
        {
            // Note that we have to do FixGenericTypeArgument twice.
            // 1. To get argument types fixed for DeclaringType
            // 2. To get argument types fixed for baseType.
            if (method.Name == baseMethod.Name
                && method.GenericParameters.Count == baseMethod.GenericParameters.Count
                && method.Parameters.Count == baseMethod.Parameters.Count)
            {
                TypeReference baseReturnType = baseMethod.ReturnType.FixGenericTypeArguments(baseMethod.DeclaringType);
                bool declAndBaseSame = baseMethod.DeclaringType.IsSameDefinition(baseType);
                if (!declAndBaseSame)
                {
                    baseReturnType = baseReturnType.FixGenericTypeArguments(baseType);
                }

                if (!method.ReturnType.IsSame(baseReturnType))
                {
                    return null;
                }

                bool matched = true;
                IList<TypeReference> genericTypeParameters = null;
                IList<TypeReference> genericMethodParameters = null;

                GenericInstanceType genericInstanceType = baseMethod.DeclaringType as GenericInstanceType;
                if (genericInstanceType != null)
                {
                    genericTypeParameters = genericInstanceType.GenericArguments;
                }

                if (method.HasGenericParameters)
                {
                    genericMethodParameters = new List<TypeReference>();
                    foreach (var genericParam in method.GenericParameters)
                    {
                        genericMethodParameters.Add(genericParam);
                    }
                }

                for (int iParam = 0; iParam < baseMethod.Parameters.Count && matched; iParam++)
                {
                    if (baseMethod.Parameters[iParam].Attributes == method.Parameters[iParam].Attributes)
                    {
                        var typeArgument = baseMethod.Parameters[iParam].ParameterType
                            .FixGenericTypeArguments(
                                genericTypeParameters,
                                genericMethodParameters);

                        if (!declAndBaseSame)
                        {
                            typeArgument = typeArgument.FixGenericTypeArguments(baseType);
                        }

                        matched = method.Parameters[iParam].ParameterType.IsSame(typeArgument);
                    }
                    else
                    {
                        matched = false;
                    }
                }

                if (matched)
                {
                    return baseMethod.FixGenericArguments(baseType);
                }
            }

            return null;
        }

        public static MethodReference IsOverridable(
            this MethodReference method,
            MethodReference baseMethod,
            TypeReference baseType)
        {
            // Note that we have to do FixGenericTypeArgument twice.
            // 1. To get argument types fixed for DeclaringType
            // 2. To get argument types fixed for baseType.
            if (method.Name == baseMethod.Name
                && method.GenericParameters.Count == baseMethod.GenericParameters.Count
                && method.Parameters.Count == baseMethod.Parameters.Count)
            {
                TypeReference baseReturnType = baseMethod.ReturnType.FixGenericTypeArguments(baseMethod.DeclaringType);
                bool declAndBaseSame = baseMethod.DeclaringType.IsSameDefinition(baseType);
                if (!declAndBaseSame)
                {
                    baseReturnType = baseReturnType.FixGenericTypeArguments(baseType);
                }

                if (!method.ReturnType.IsSame(baseReturnType))
                {
                    return null;
                }

                bool matched = true;
                IList<TypeReference> genericTypeParameters = null;
                IList<TypeReference> genericMethodParameters = null;

                GenericInstanceType genericInstanceType = baseMethod.DeclaringType as GenericInstanceType;
                if (genericInstanceType != null)
                {
                    genericTypeParameters = genericInstanceType.GenericArguments;
                }

                if (method.HasGenericParameters)
                {
                    genericMethodParameters = new List<TypeReference>();
                    foreach (var genericParam in method.GenericParameters)
                    {
                        genericMethodParameters.Add(genericParam);
                    }
                }

                for (int iParam = 0; iParam < baseMethod.Parameters.Count && matched; iParam++)
                {
                    if (baseMethod.Parameters[iParam].Attributes == method.Parameters[iParam].Attributes)
                    {
                        var typeArgument = baseMethod.Parameters[iParam].ParameterType
                            .FixGenericTypeArguments(
                                genericTypeParameters,
                                genericMethodParameters);

                        if (!declAndBaseSame)
                        {
                            typeArgument = typeArgument.FixGenericTypeArguments(baseType);
                        }

                        matched = method.Parameters[iParam].ParameterType.IsSame(typeArgument);
                    }
                    else
                    {
                        matched = false;
                    }
                }

                if (matched)
                {
                    return baseMethod.FixGenericArguments(baseType);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the override.
        /// </summary>
        /// <param name="typeReference">The type reference.</param>
        /// <param name="methodReference">The method reference.</param>
        /// <returns>MethodReference of the method that overrides given method.</returns>
        public static MethodReference GetOverride(
            this TypeReference typeReference,
            MethodReference methodReference)
        {
            TypeDefinition typeDefinition = typeReference.Resolve();

            if (!methodReference.Resolve().IsVirtual)
            { return null; }

            if (methodReference.DeclaringType.IsSameDefinition(typeReference))
            { return null; }

            MethodReference rv;
            foreach (var method in typeDefinition.Methods)
            {
                if (!method.IsVirtual)
                { continue; }

                rv = method.FixGenericArguments(typeReference);
                if (rv.IsOverridable(methodReference, methodReference.DeclaringType) != null)
                {
                    return rv;
                }
            }

            TypeReference baseType = typeReference.Resolve().BaseType;
            baseType = baseType.FixGenericTypeArguments(typeReference);
            return baseType.GetOverride(methodReference);
        }

        /// <summary>
        /// Gets the interface overrides.
        /// Note: There is a case that we are skipping here.
        ///
        /// If a type implements and interface, but it's base classes has the methods that matches
        /// interface methods, then interface has been implemented. Weird :(, so this case is not
        /// supported for now.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<MethodReference, MethodDefinition> GetInterfaceOverrides(
            this TypeDefinition typeDefinition,
            ClrContext clrContext)
        {
            if (typeDefinition.IsInterface)
            {
                return new Dictionary<MethodReference, MethodDefinition>();
            }

            List<MethodDefinition> virtualMethods = new List<MethodDefinition>();
            Dictionary<MethodReference, MethodDefinition> rv =
                new Dictionary<MethodReference, MethodDefinition>(MemberReferenceComparer.Instance);

            HashSet<TypeReference> baseIfaces = null;
            if (typeDefinition.BaseType != null)
            {
                baseIfaces = new HashSet<TypeReference>(
                    typeDefinition.BaseType.GetAllImplementedInterfaces());
            }

            virtualMethods.AddRange(
                typeDefinition.Methods.Where(method => method.IsVirtual));

            foreach (var vMethod in virtualMethods)
            {
                foreach (var method in vMethod.Overrides)
                {
                    if (!rv.ContainsKey(method))
                    {
                        rv[method] = vMethod;
                    }
                }
            }

            foreach (var baseType in TypeHelpers.GetBaseTypes(typeDefinition))
            {
                if (baseIfaces != null
                    && baseIfaces.Contains(baseType))
                { continue; }

                TypeDefinition baseDefinition = baseType.Resolve();

                foreach (var methodDef in clrContext.GetVirtualOverridables(baseDefinition))
                {
                    MethodReference method = methodDef;
                    bool matched = false;

                    if (method.DeclaringType.HasGenericParameters)
                    {
                        method = methodDef.FixGenericArguments(baseType);
                    }

                    if (rv.ContainsKey(method))
                    { continue; }

                    foreach (var virtualMethod in virtualMethods)
                    {
                        MethodReference methodReference = virtualMethod.IsOverridable(method, baseType);
                        if (methodReference != null)
                        {
                            rv.Add(methodReference, virtualMethod);
                            matched = true;
                            break;
                        }
                    }

                    if (!matched && baseDefinition.IsInterface && !typeDefinition.IsInterface)
                    {
                        throw new InvalidProgramException(
                            string.Format(
                                "Can't map interface method {0} to instance method for type {1}",
                                method,
                                typeDefinition));
                    }
                }
            }

            return rv;
        }

        /// <summary>
        /// Fixes the generic parameters.
        /// </summary>
        /// <param name="typeDefinition">The type definition.</param>
        /// <returns>Return GenericInstanceType if passed in typeDefinition is generic type definition, else typeDefinition itself.</returns>
        public static TypeReference FixGenericParameters(this TypeDefinition typeDefinition)
        {
            if (typeDefinition.HasGenericParameters)
            {
                var genericInstanceType = new GenericInstanceType(
                    typeDefinition);

                foreach (var genericParam in typeDefinition.GenericParameters)
                {
                    genericInstanceType.GenericArguments.Add(genericParam);
                }

                return genericInstanceType;
            }
            else
            {
                return typeDefinition;
            }
        }

        /// <summary>
        /// Fixes the generic arguments.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="paramDef">The type reference.</param>
        /// <returns></returns>
        public static MethodReference FixGenericArguments(
            this MethodReference method,
            TypeReference typeReference)
        {
            TypeReference declaringType = method.DeclaringType;

            if (declaringType.IsDefinition)
            {
                declaringType = ((TypeDefinition)declaringType).FixGenericParameters();
            }

            MethodReference methodReference = new MethodReference(
                method.Name,
                method.ReturnType,
                declaringType.FixGenericTypeArguments(typeReference));

            methodReference.HasThis = method.Resolve().HasThis;

            foreach (var parameter in method.Parameters)
            {
                methodReference.Parameters.Add(parameter);
            }

            foreach (var genericParam in method.GenericParameters)
            {
                methodReference.GenericParameters.Add(genericParam);
            }

            return methodReference;
        }

        /// <summary>
        /// Gets the generic arguments.
        /// </summary>
        /// <param name="paramDef">The type reference.</param>
        /// <returns>Return genericArgument of given types.</returns>
        public static IList<TypeReference> GetGenericArguments(this TypeReference typeReference)
        {
            if (typeReference is TypeSpecification)
            {
                ArrayType arrayType = typeReference as ArrayType;
                if (arrayType != null)
                {
                    return new TypeReference[] { arrayType.ElementType };
                }

                ByReferenceType refType = typeReference as ByReferenceType;
                if (refType != null)
                {
                    return new TypeReference[] { arrayType.ElementType };
                }

                GenericInstanceType giType = typeReference as GenericInstanceType;
                if (giType != null)
                {
                    return giType.GenericArguments;
                }

                OptionalModifierType optmodType = typeReference as OptionalModifierType;
                if (optmodType != null)
                {
                    return new TypeReference[] { arrayType.ElementType };
                }

                RequiredModifierType reqmodType = typeReference as RequiredModifierType;
                if (reqmodType != null)
                {
                    return new TypeReference[] { arrayType.ElementType };
                }

                PointerType ptrType = typeReference as PointerType;
                if (ptrType != null)
                {
                    return new TypeReference[] { arrayType.ElementType };
                }
            }

            return TypeHelpers.emtyTypeReferences;
        }

        /// <summary>
        /// Determines whether the specified type def is static.
        /// </summary>
        /// <param name="typeDef">The type def.</param>
        /// <returns>
        /// <c>true</c> if the specified type def is static; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsStatic(this TypeDefinition typeDef)
        {
            return ((typeDef.Attributes & TypeAttributes.Abstract) == TypeAttributes.Abstract)
                && ((typeDef.Attributes & TypeAttributes.Sealed) == TypeAttributes.Sealed);
        }

        /// <summary>
        /// Determines whether the specified type definition is delegate.
        /// </summary>
        /// <param name="typeDefinition">The type definition.</param>
        /// <returns>
        /// <c>true</c> if the specified type definition is delegate; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsDelegate(this TypeReference typeReference)
        {
            TypeDefinition typeDefinition = typeReference.Resolve();
            if (typeDefinition == null || typeDefinition.BaseType == null)
            { return false; }

            TypeReference baseType = typeDefinition.BaseType;
            return (baseType.FullName == MulticastDelegateStr || baseType.FullName == DelegateStr);
        }

        /// <summary>
        /// Determines whether the specified type is boolean.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// <c>true</c> if the specified type is boolean; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsBoolean(this TypeReference type)
        {
            return type != null && type.MetadataType == MetadataType.Boolean;
        }

        /// <summary>
        /// Determines whether the specified type is EnumOrInteger.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// <c>true</c> if the specified type is integer or enum; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsIntegerOrEnum(this TypeReference type)
        {
            return IsSigned(type) != null;
        }

        /// <summary>
        /// Determines whether the specified type is double.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// <c>true</c> if the specified type is double; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsDouble(this TypeReference type)
        {
            return type != null
                && (type.MetadataType == MetadataType.Double
                    || type.MetadataType == MetadataType.Single);
        }

        /// <summary>
        /// Determines whether the specified type is enum.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// <c>true</c> if the specified type is enum; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEnum(this TypeReference type)
        {
            if (type == null)
                return false;
            // unfortunately we cannot rely on type.IsValueType here - it's not set when the instruction operand is a typeref (as opposed to a typespec)
            TypeDefinition typeDef = type.Resolve() as TypeDefinition;
            return typeDef != null && typeDef.IsEnum;
        }

        /// <summary>
        /// Determines whether the specified type is signed.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>true if paramDef is signed int, false if unsigned int else null.</returns>
        public static bool? IsSigned(this TypeReference type)
        {
            if (type == null)
                return null;
            // unfortunately we cannot rely on type.IsValueType here - it's not set when the instruction operand is a typeref (as opposed to a typespec)
            TypeDefinition typeDef = type.Resolve() as TypeDefinition;
            if (typeDef != null && typeDef.IsEnum)
            {
                TypeReference underlyingType = typeDef.Fields.Single(f => f.IsRuntimeSpecialName && !f.IsStatic).FieldType;
                return IsSigned(underlyingType);
            }

            switch (type.MetadataType)
            {
                case MetadataType.SByte:
                case MetadataType.Int16:
                case MetadataType.Int32:
                case MetadataType.Int64:
                case MetadataType.IntPtr:
                    return true;
                case MetadataType.Byte:
                case MetadataType.Char:
                case MetadataType.UInt16:
                case MetadataType.UInt32:
                case MetadataType.UInt64:
                case MetadataType.UIntPtr:
                    return false;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Determines whether the specified type is generic.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if the specified type is generic; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsGeneric(this TypeReference type)
        {
            TypeDefinition typeDef = type.Resolve();
            return typeDef.IsGenericInstance || typeDef.HasGenericParameters;
        }

        /// <summary>
        /// Gets the type code.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>TypeCode for type</returns>
        public static TypeCode GetTypeCode(this TypeReference type)
        {
            if (type == null)
            { return TypeCode.Empty; }

            switch (type.MetadataType)
            {
                case MetadataType.Boolean:
                    return TypeCode.Boolean;
                case MetadataType.Char:
                    return TypeCode.Char;
                case MetadataType.SByte:
                    return TypeCode.SByte;
                case MetadataType.Byte:
                    return TypeCode.Byte;
                case MetadataType.Int16:
                    return TypeCode.Int16;
                case MetadataType.UInt16:
                    return TypeCode.UInt16;
                case MetadataType.Int32:
                    return TypeCode.Int32;
                case MetadataType.UInt32:
                    return TypeCode.UInt32;
                case MetadataType.Int64:
                    return TypeCode.Int64;
                case MetadataType.UInt64:
                    return TypeCode.UInt64;
                case MetadataType.Single:
                    return TypeCode.Single;
                case MetadataType.Double:
                    return TypeCode.Double;
                case MetadataType.String:
                    return TypeCode.String;
                default:
                    return TypeCode.Object;
            }
        }

        /// <summary>
        /// Determines whether the specified method reference has associated member.
        /// </summary>
        /// <param name="methodReference">The method reference.</param>
        /// <returns>
        /// <c>true</c> if the specified method reference has associated member; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasAssociatedMember(
            this MethodDefinition methodReference)
        {
            return methodReference.IsGetter
                || methodReference.IsSetter
                || methodReference.IsAddOn
                || methodReference.IsRemoveOn;
        }

        /// <summary>
        /// Gets the associated member.
        /// </summary>
        /// <param name="methodDefinition">The method definition.</param>
        /// <returns>PropertyDefinition or EventDefinition</returns>
        public static IMemberDefinition GetAssociatedMember(
            this MethodDefinition methodDefinition)
        {
            if (methodDefinition.IsGetter || methodDefinition.IsSetter)
            {
                return methodDefinition.GetPropertyDefinition();
            }
            else if (methodDefinition.IsAddOn || methodDefinition.IsRemoveOn)
            {
                return methodDefinition.GetEventDefinition();
            }

            return null;
        }

        /// <summary>
        /// Gets the event definition.
        /// </summary>
        /// <param name="methodDefinition">The method definition.</param>
        /// <returns>Event definition if found.</returns>
        public static EventDefinition GetEventDefinition(this MethodDefinition methodDefinition)
        {
            if (!methodDefinition.IsAddOn
                && !methodDefinition.IsRemoveOn)
            { return null; }

            foreach (var evt in methodDefinition.DeclaringType.Resolve().Events)
            {
                if (evt.AddMethod == methodDefinition
                    || evt.RemoveMethod == methodDefinition)
                { return evt; }
            }

            return null;
        }

        /// <summary>
        /// Gets the property definition.
        /// </summary>
        /// <param name="methodDefinition">The method definition.</param>
        /// <returns>Property definition for method, null if no propertyDefinition found.</returns>
        public static PropertyDefinition GetPropertyDefinition(this MethodDefinition methodDefinition)
        {
            if (!methodDefinition.IsGetter
                && !methodDefinition.IsSetter)
            { return null; }

            foreach (var propertyDef in methodDefinition.DeclaringType.Resolve().Properties)
            {
                if (propertyDef.GetMethod == methodDefinition
                    || propertyDef.SetMethod == methodDefinition)
                { return propertyDef; }
            }

            return null;
        }

        /// <summary>
        /// Determines whether the specified prop def is static.
        /// </summary>
        /// <param name="propDef">The prop def.</param>
        /// <returns>
        /// <c>true</c> if the specified prop def is static; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsStatic(this PropertyDefinition propDef)
        {
            return propDef.SetMethod != null
                ? propDef.SetMethod.IsStatic
                : propDef.GetMethod.IsStatic;
        }

        /// <summary>
        /// Gets the parent definition.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>Typedefinition for given method.</returns>
        public static TypeDefinition GetParentDefinition(this MethodDefinition method)
        {
            return (TypeDefinition)method.DeclaringType;
        }

        /// <summary>
        /// Selects the attribute.
        /// </summary>
        /// <param name="attributes">The attributes.</param>
        /// <param name="paramDef">The type reference.</param>
        /// <returns></returns>
        public static CustomAttribute SelectAttribute(this IList<CustomAttribute> attributes, TypeReference typeReference)
        {
            for (int iAttribute = 0; iAttribute < attributes.Count; iAttribute++)
            {
                if (attributes[iAttribute].Constructor.DeclaringType.ExtendsType(typeReference))
                {
                    return attributes[iAttribute];
                }
            }

            return null;
        }

        /// <summary>
        /// A TypeReference extension method that extends type.
        /// </summary>
        /// <param name="type">          The type to act on. </param>
        /// <param name="typeReference"> The type reference. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public static bool ExtendsType(this TypeReference type, TypeReference typeReference)
        {
            do
            {
                if (type.IsSame(typeReference))
                {
                    return true;
                }

                type = type.GetBaseType();
            }
            while (type != null);

            return false;
        }

        /// <summary>
        /// A TypeReference extension method that gets a base type.
        /// </summary>
        /// <param name="type"> The type to act on. </param>
        /// <returns>
        /// The base type.
        /// </returns>
        public static TypeReference GetBaseType(this TypeReference type)
        {
            var baseType = ((TypeDefinition)type.GetDefinition()).BaseType;
            if (baseType != null)
            {
                return baseType.FixGenericTypeArguments(type);
            }

            return null;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>Name without extension.</returns>
        private static string GetName(this IMetadataScope scope)
        {
            ModuleDefinition moduleDefinition = scope as ModuleDefinition;
            if (moduleDefinition != null)
            {
                return moduleDefinition.Assembly.Name.Name;
            }

            AssemblyNameReference assembly = scope as AssemblyNameReference;
            if (assembly != null)
            {
                return assembly.Name;
            }

            return scope.Name;
        }

        /// <summary>
        /// Gets the base types.
        /// </summary>
        /// <returns>Enumerable for enumerating over all the baseTypes incl. interfaces.</returns>
        private static IEnumerable<TypeReference> GetBaseTypes(TypeDefinition typeDefinition)
        {
            if (typeDefinition.BaseType != null)
            {
                yield return typeDefinition.BaseType;
            }

            foreach (var iface in typeDefinition.Interfaces)
            {
                yield return iface;
            }

            yield break;
        }

        /// <summary>
        /// Gets all implemented interfaces.
        /// </summary>
        /// <param name="paramDef">The type reference.</param>
        /// <returns>All the interfaces implemented by this and all it's base types.</returns>
        private static IEnumerable<TypeReference> GetAllImplementedInterfaces(this TypeReference typeReference)
        {
            TypeDefinition typeDefinition = typeReference.Resolve();
            HashSet<TypeReference> returnedInterfaces = new HashSet<TypeReference>();

            if (typeDefinition.BaseType != null)
            {
                TypeReference baseType = typeReference.GetBaseType();
                foreach (var iface in baseType.GetAllImplementedInterfaces())
                {
                    returnedInterfaces.Add(iface);
                    yield return iface;
                }
            }

            foreach (var iface in typeDefinition.Interfaces)
            {
                var fixedIface = iface.FixGenericTypeArguments(typeReference);
                if (!returnedInterfaces.Contains(fixedIface))
                {
                    yield return iface;
                }
            }

            yield break;
        }
    }

    /// <summary>
    /// TypeReference comparer.
    /// </summary>
    public class MemberReferenceComparer
        : IEqualityComparer<TypeReference>,
            IEqualityComparer<TypeDefinition>,
            IEqualityComparer<MethodDefinition>,
            IEqualityComparer<MemberReference>,
            IEqualityComparer<FieldReference>,
            IEqualityComparer<PropertyReference>,
            IEqualityComparer<EventReference>,
            IEqualityComparer<MethodReference>
    {
        Dictionary<object, int> calculatedHashCodes = new Dictionary<object, int>();

        private static MemberReferenceComparer instance;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static MemberReferenceComparer Instance
        {
            get
            {
                if (MemberReferenceComparer.instance == null)
                {
                    MemberReferenceComparer.instance = new MemberReferenceComparer();
                }
                return MemberReferenceComparer.instance;
            }
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <paramref name="T"/> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T"/> to compare.</param>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(EventReference x, EventReference y)
        {
            return x.IsSame(y);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.
        /// </exception>
        public int GetHashCode(EventReference obj)
        {
            int rv;
            if (this.calculatedHashCodes.TryGetValue(obj, out rv))
            {
                return rv;
            }

            if (obj == null)
            { rv = 0; }
            else
            { rv = obj.Name.GetHashCode() ^ this.GetHashCode(obj.DeclaringType); }

            this.calculatedHashCodes[obj] = rv;
            return rv;
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <paramref name="T"/> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T"/> to compare.</param>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(PropertyReference x, PropertyReference y)
        {
            return x.IsSame(y);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.
        /// </exception>
        public int GetHashCode(PropertyReference obj)
        {
            int rv;
            if (this.calculatedHashCodes.TryGetValue(obj, out rv))
            {
                return rv;
            }

            if (obj == null)
            { rv = 0; }
            else
            {
                rv = obj.Name.GetHashCode() ^ obj.Parameters.Count ^ this.GetHashCode(obj.DeclaringType);
            }

            this.calculatedHashCodes[obj] = rv;
            return rv;
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <paramref name="T"/> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T"/> to compare.</param>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(FieldReference x, FieldReference y)
        {
            return x.IsSame(y);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.
        /// </exception>
        public int GetHashCode(FieldReference obj)
        {
            int rv;
            if (this.calculatedHashCodes.TryGetValue(obj, out rv))
            {
                return rv;
            }

            if (obj == null)
            { rv = 0; }
            else
            {
                rv = obj.Name.GetHashCode() ^ this.GetHashCode(obj.DeclaringType);
            }

            this.calculatedHashCodes[obj] = rv;
            return rv;
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <paramref name="T"/> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T"/> to compare.</param>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(MemberReference x, MemberReference y)
        {
            return x.IsSame(y);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.
        /// </exception>
        public int GetHashCode(MemberReference obj)
        {
            if (obj == null)
            { return 0; }

            int rv;
            if (this.calculatedHashCodes.TryGetValue(obj, out rv))
            {
                return rv;
            }

            TypeReference typeRef = obj as TypeReference;
            if (typeRef != null)
            { return this.GetHashCode(typeRef); }

            MethodReference methodRef = obj as MethodReference;
            if (methodRef != null)
            { return this.GetHashCode(methodRef); }

            FieldReference fieldRef = obj as FieldReference;
            if (fieldRef != null)
            { return this.GetHashCode(fieldRef); }

            PropertyReference propRef = obj as PropertyReference;
            if (propRef != null)
            { return this.GetHashCode(propRef); }

            EventReference evtRef = obj as EventReference;
            if (evtRef != null)
            { return this.GetHashCode(evtRef); }

            return 0;
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <paramref name="T"/> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T"/> to compare.</param>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(MethodReference x, MethodReference y)
        {
            return x.IsSame(y);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.
        /// </exception>
        public int GetHashCode(MethodReference obj)
        {
            int rv;
            if (this.calculatedHashCodes.TryGetValue(obj, out rv))
            {
                return rv;
            }

            GenericInstanceMethod genericMethod = obj as GenericInstanceMethod;
            if (genericMethod != null)
            {
                rv = this.GetHashCode(genericMethod.ElementMethod);
                foreach (var arg in genericMethod.GenericArguments)
                {
                    rv += 1;
                    rv ^= this.GetHashCode(arg);
                }
            }
            else
            {
                rv = this.GetHashCode(obj.DeclaringType) + obj.Name.GetHashCode();
                foreach (var argType in obj.Parameters)
                {
                    rv ^= this.GetHashCode(argType.ParameterType);
                    rv ^= argType.Index;
                }
            }

            this.calculatedHashCodes[obj] = rv;
            return rv;
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <paramref name="T"/> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T"/> to compare.</param>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(MethodDefinition x, MethodDefinition y)
        {
            return x.IsSameDefinition(y);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public int GetHashCode(MethodDefinition x)
        {
            /*
            int rv;
            if (this.calculatedHashCodes.TryGetValue(x, out rv))
            {
                return rv;
            }

            rv = this.GetHashCode(x.DeclaringType) + x.Name.GetHashCode();
            foreach (var argType in x.Parameters)
            {
                rv ^= this.GetHashCode(argType.ParameterType);
                rv ^= argType.Index;
            }

            this.calculatedHashCodes[x] = rv;

            return rv;
             */

            return this.GetHashCode((MethodReference)x);
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <paramref name="T"/> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T"/> to compare.</param>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(TypeReference x, TypeReference y)
        {
            return x.IsSame(y);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="typeRef">The type ref.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public int GetHashCode(TypeReference typeRef)
        {
            int rv;
            if (this.calculatedHashCodes.TryGetValue(typeRef, out rv))
            {
                return rv;
            }

            if (typeRef == null)
            { rv = 0; }
            else if (!(typeRef is TypeSpecification))
            {
                GenericParameter genericParameter = typeRef as GenericParameter;
                if (genericParameter != null)
                {
                    rv = genericParameter.Position
                        ^ ((MemberReference)genericParameter.Owner)
                            .Name.ToLowerInvariant()
                            .GetHashCode();
                }
                else
                {
                    rv = typeRef.FullName.GetHashCode()
                        + typeRef.Resolve().Module.Assembly.Name.Name.ToLowerInvariant().GetHashCode();
                }
            }
            else if (typeRef.IsArray)
            {
                ArrayType arrayTypeRef = typeRef as ArrayType;

                rv = this.GetHashCode(arrayTypeRef.ElementType) ^ arrayTypeRef.Dimensions.Count ^ typeof(ArrayType).GetHashCode();
            }
            else if (typeRef.IsGenericInstance)
            {
                GenericInstanceType genericTypeRef = (GenericInstanceType)typeRef;

                rv = genericTypeRef.Name.GetHashCode() ^ genericTypeRef.Namespace.GetHashCode();
                for (int genericIndex = 0; genericIndex < genericTypeRef.GenericArguments.Count; genericIndex++)
                {
                    rv ^= this.GetHashCode(genericTypeRef.GenericArguments[genericIndex]) + genericIndex;
                }
            }
            else if (typeRef.IsGenericParameter)
            {
                GenericParameter paramTypeRef = (GenericParameter)typeRef;

                rv = (paramTypeRef.Position + (int)paramTypeRef.Owner.GenericParameterType * 10);

                if (paramTypeRef.Owner.GenericParameterType == GenericParameterType.Method)
                {
                    MethodReference methodReference = (MethodReference)paramTypeRef.Owner;
                    rv ^= this.GetHashCode(methodReference.Resolve());
                }
                else
                {
                    rv ^= this.GetHashCode(((TypeReference)paramTypeRef.Owner).Resolve());
                }
            }
            else if (typeRef.IsByReference
                || typeRef.IsOptionalModifier
                || typeRef.IsPointer
                || typeRef.IsRequiredModifier)
            {
                rv = typeRef.GetType().GetHashCode() ^ this.GetHashCode(typeRef.GetElementType());
            }
            else
            {
                throw new NotSupportedException();
            }

            this.calculatedHashCodes[typeRef] = rv;
            return rv;

            /*
             * Possibly simpler solution.
            if (left == right)
            { return true; }

            if (left == null || right == null)
            { return false; }

            return left.FullName == right.FullName; // TODO: implement this more efficiently?
             */
        }

        /// <summary>
        /// Equalses the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public bool Equals(TypeDefinition x, TypeDefinition y)
        {
            return x.IsSameDefinition(y);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public int GetHashCode(TypeDefinition obj)
        {
            // return obj.FullName.GetHashCode() + obj.Module.Assembly.Name.Name.ToLowerInvariant().GetHashCode();
            return this.GetHashCode((TypeReference)obj);
        }
    }
}