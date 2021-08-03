//-----------------------------------------------------------------------
// <copyright file="DefaultValueConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.ExpressionsConverter
{
    using Mono.Cecil;
    using NScript.CLR;
    using NScript.CLR.AST;
    using NScript.Converter.TypeSystemConverter;
    using NScript.Utils;

    /// <summary>
    /// Definition for DefaultValueConverter
    /// </summary>
    public static class DefaultValueConverter
    {
        /// <summary>
        /// Converts the specified converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="defaultValueExpression">The default value expression.</param>
        /// <returns>returns expression for default value.</returns>
        public static JST.Expression Convert(
            IMethodScopeConverter converter,
            DefaultValueExpression defaultValueExpression)
        {
            return DefaultValueConverter.GetDefaultValue(
                converter,
                converter.RuntimeManager,
                converter.Scope,
                defaultValueExpression.ResultType,
                defaultValueExpression.Location);
        }

        /// <summary>
        /// Gets the default value.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="paramDef">The type reference.</param>
        /// <returns>returns expression for default value.</returns>
        public static JST.Expression GetDefaultValue(
            IResolver resolver,
            RuntimeScopeManager runtimeScopeManager,
            JST.IdentifierScope scope,
            TypeReference typeReference,
            Location location = null)
        {
            if (typeReference.IsBoolean())
            {
                return new JST.BooleanLiteralExpression(
                    scope,
                    false);
            }
            else if (typeReference.IsIntegerOrEnum()
                || typeReference.IsDouble())
            {
                return new JST.NumberLiteralExpression(scope, 0);
            }

            if (typeReference.IsArray)
            { return new JST.NullLiteralExpression(scope); }

            TypeDefinition typeDefinition = typeReference.Resolve();

            if (typeDefinition != null)
            {
                if (!typeDefinition.IsValueOrEnum()
                    || typeDefinition.IsSameDefinition(
                        runtimeScopeManager.Context.ClrKnownReferences.NullableType))
                {
                    // Everything else than struct are nulls.
                    return new JST.NullLiteralExpression(scope);
                }

                JST.InlineObjectInitializer initializer = new JST.InlineObjectInitializer(
                    location,
                    scope);

                foreach (var field in typeDefinition.Fields)
                {
                    if (field.IsStatic || field.HasConstant)
                    {
                        continue;
                    }

                    initializer.AddInitializer(
                        runtimeScopeManager.Resolve(field),
                        DefaultValueConverter.GetDefaultValue(
                            resolver,
                            runtimeScopeManager,
                            scope,
                            field.FieldType));
                }

                return initializer;
            }

            TypeDefinition genericTypeDefinition = typeReference.Resolve();
            if (typeReference.IsArray
                || (genericTypeDefinition != null
                    && genericTypeDefinition.HasGenericParameters
                    && genericTypeDefinition.BaseType != null
                    && !runtimeScopeManager.Context.ClrKnownReferences.ValueType.IsSameDefinition(genericTypeDefinition.BaseType)))
            {
                // Again in this case we have generic type but it is of class type.
                return new JST.NullLiteralExpression(scope);
            }

            GenericParameter genericParameter = typeReference as GenericParameter;
            if (genericParameter != null)
            {
                bool? isValueType = null;
                foreach (var constraint in genericParameter.Constraints)
                {
                    if (constraint.IsValueOrEnum())
                    {
                        isValueType = true;
                        break;
                    }
                }

                if (isValueType.HasValue && isValueType.Value == false)
                {
                    return new JST.NullLiteralExpression(scope);
                }
            }

            return MethodCallExpressionConverter.CreateMethodCallExpression(
                new MethodCallContext(
                    runtimeScopeManager.Context.KnownReferences.GetDefaultMethodStatic,
                    location,
                    scope),
                new JST.Expression[]
                {
                     JST.IdentifierExpression.Create(
                        location,
                        scope,
                        resolver.Resolve(typeReference)),
                },
                resolver,
                runtimeScopeManager);
        }

        /// <summary>
        /// Determines whether the specified type reference is struct.
        /// </summary>
        /// <param name="paramDef">The type reference.</param>
        /// <returns>
        /// true if the specified type reference is struct; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsStruct(TypeReference typeReference)
        {
            TypeDefinition typeDefinition = typeReference.Resolve();

            if (typeDefinition != null
                && typeDefinition.BaseType != null
                && typeDefinition.BaseType.IsValueOrEnum()
                    || typeDefinition.BaseType.IsEnum())
            {
                return true;
            }

            return false;
        }
    }
}