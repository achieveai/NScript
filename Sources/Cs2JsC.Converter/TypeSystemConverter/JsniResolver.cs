//-----------------------------------------------------------------------
// <copyright file="JsniResolver.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.TypeSystemConverter
{
    using System;
    using System.Linq;
    using Cs2JsC.CLR;
    using Cs2JsC.JST;
    using Mono.Cecil;

    /// <summary>
    /// Definition for JsniResolver
    /// </summary>
    public class JsniResolver : JSParser.IResolver
    {
        /// <summary>
        /// Method converter to be used.
        /// </summary>
        private readonly MethodConverter converter;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsniResolver"/> class.
        /// </summary>
        /// <param name="methodConverter">The method converter.</param>
        public JsniResolver(
            MethodConverter methodConverter)
        {
            this.converter = methodConverter;
        }

        /// <summary>
        /// Resolves the identifier.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="identifier">The identifier.</param>
        /// <returns>
        /// Resolved identifier.
        /// </returns>
        public JST.Expression ResolveIdentifier(
            JST.IdentifierScope scope,
            string identifierName)
        {
            Identifier identifier;
            if (!this.converter.TryResolveArgument(
                identifierName,
                out identifier))
            {
                if (identifierName == MethodConverter.ThisArgument)
                {
                    return this.converter.ResolveThis(scope);
                }

                identifier = this.converter.RuntimeManager.GetKnownIdentifier(
                    identifierName);

                if (identifier == null)
                {
                    throw new ApplicationException(
                        string.Format("Can't resolve local variable {0} in script for {1}",
                            identifierName,
                            this.converter.MethodDefinition));
                }
            }

            return new IdentifierExpression(
                identifier,
                scope);
        }

        /// <summary>
        /// Resolves the type.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>
        /// Expression for resolved type.
        /// </returns>
        public JST.Expression ResolveType(
            JST.IdentifierScope scope,
            Tuple<string, string> typeName)
        {
            if (typeName.Item1 == null)
            {
                // this means that we are trying to resolve GenericTypeParameter.
                MethodDefinition methodDef = converter.MethodDefinition;
                foreach (var genericTypeArg in methodDef.GenericParameters)
                {
                    if (genericTypeArg.Name == typeName.Item2)
                    {
                        return converter.ResolveTypeToExpression(
                            genericTypeArg,
                            scope);
                    }
                }

                TypeDefinition typeDef = methodDef.DeclaringType.Resolve();

                do
                {
                    foreach (var genericTypeArg in typeDef.GenericParameters)
                    {
                        if (genericTypeArg.Name == typeName.Item2)
                        {
                            return converter.ResolveTypeToExpression(
                                genericTypeArg,
                                scope);
                        }
                    }

                    typeDef = typeDef.DeclaringType;
                }
                while (typeDef != null);
            }

            Cs2JsC.CLR.ClrContext clrContext = this.converter.RuntimeManager.Context.ClrContext;
            TypeDefinition typeDefinition = clrContext.GetTypeDefinition(typeName);
            if (typeDefinition == null)
            {
                throw new ApplicationException(
                    string.Format("Can't resolve type: [{0}]{1}.",
                        typeName.Item1,
                        typeName.Item2));
            }

            return JST.IdentifierExpression.Create(
                null,
                scope,
                this.converter.Resolve(typeDefinition));
        }

        /// <summary>
        /// Resolves the field.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>
        /// Expression for resolved field.
        /// </returns>
        public JST.Expression ResolveField(
            JST.IdentifierScope scope,
            Tuple<string, string> typeName,
            bool isInstance,
            string fieldName)
        {
            Cs2JsC.CLR.ClrContext clrContext = this.converter.RuntimeManager.Context.ClrContext;
            TypeDefinition typeDefinition = clrContext.GetTypeDefinition(typeName);
            if (typeDefinition == null)
            {
                throw new ApplicationException(
                    string.Format("Can't resolve type: [{0}]{1}.",
                        typeName.Item1,
                        typeName.Item2));
            }

            FieldDefinition fieldDefinition = typeDefinition.Fields.FirstOrDefault(f => f.Name == fieldName);

            if (fieldDefinition == null)
            {
                throw new ApplicationException(
                    string.Format("Can't resolve [{0}]{1}::{2}",
                        typeName.Item1,
                        typeName.Item2,
                        fieldName));
            }

            if ((fieldDefinition.HasConstant || fieldDefinition.IsStatic) == isInstance)
            {
                throw new ApplicationException(
                    string.Format("Expecting instance member but got static one, [{0}]{1}::{2}",
                        typeName.Item1,
                        typeName.Item2,
                        fieldName));
            }

            if (fieldDefinition.IsStatic)
            {
                return IdentifierExpression.Create(
                        null,
                        scope,
                        this.converter.ResolveStaticMember(fieldDefinition));
            }
            else if (fieldDefinition.HasConstant)
            {
                return TypeConverter.ConvertConstValue(fieldDefinition.Constant);
            }
            else
            {
                return new IdentifierExpression(
                    this.converter.Resolve(fieldDefinition),
                    scope);
            }
        }

        /// <summary>
        /// Resolves the method.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="methodArguments">The method arguments.</param>
        /// <returns>
        /// Expression for referncing given method.
        /// </returns>
        public JST.Expression ResolveMethod(
            JST.IdentifierScope scope,
            Tuple<string, string> typeName,
            bool isInstance,
            string methodName,
            params Tuple<string, string>[] methodArguments)
        {
            Cs2JsC.CLR.ClrContext clrContext = this.converter.RuntimeManager.Context.ClrContext;
            TypeDefinition typeDefinition = clrContext.GetTypeDefinition(typeName);
            if (typeDefinition == null)
            {
                throw new ApplicationException(
                    string.Format("Can't resolve type: [{0}]{1}.",
                        typeName.Item1,
                        typeName.Item2));
            }

            TypeReference[] argumentTypes = new TypeReference[methodArguments.Length];
            for (int iMethodArgument = 0;
                iMethodArgument < methodArguments.Length;
                iMethodArgument++)
            {
                if (string.IsNullOrEmpty(methodArguments[iMethodArgument].Item1)
                    && methodArguments[iMethodArgument].Item2.StartsWith("!"))
                {
                    var gtp = clrContext.GetTypeParameter(this.converter.MethodDefinition.Module, methodArguments[iMethodArgument]);
                    argumentTypes[iMethodArgument] = gtp;
                }
                else
                {
                    TypeDefinition argumentDefinition = clrContext.GetTypeDefinition(methodArguments[iMethodArgument]);
                    if (argumentDefinition == null)
                    {
                        throw new ApplicationException(
                            string.Format("Can't resolve type: [{0}]{1}.",
                                methodArguments[iMethodArgument].Item1,
                                methodArguments[iMethodArgument].Item2));
                    }

                    argumentTypes[iMethodArgument] = argumentDefinition;
                }
            }

            foreach (var method in typeDefinition.Methods)
            {
                if (method.Name == methodName &&
                    argumentTypes.Length == method.Parameters.Count &&
                    !method.HasGenericParameters)
                {
                    bool argsMatched = true;
                    for (int iArgument = 0; iArgument < argumentTypes.Length; iArgument++)
                    {
                        if (!argumentTypes[iArgument].IsSame(method.Parameters[iArgument].ParameterType))
                        {
                            argsMatched = false;
                            break;
                        }
                    }

                    if (argsMatched)
                    {
                        if (method.IsStatic == isInstance)
                        {
                            throw new ApplicationException(
                                string.Format(
                                    "Expecting instance member but got static one, [{0}]{1}::{2}(...)",
                                    typeName.Item1,
                                    typeName.Item2,
                                    methodName));
                        }
                        if (method.IsStatic)
                        {
                            return IdentifierExpression.Create(
                                null,
                                scope,
                                this.converter.ResolveStaticMember(method));
                        }
                        else if (method.IsVirtual &&
                            !typeDefinition.IsValueType)
                        {
                            // If the type is struct type, then there is no point
                            // in calling virtual method, since there are no more
                            // derived types.
                            return this.converter.ResolveVirtualMethod(
                                method,
                                scope);
                        }
                        else
                        {
                            return new IdentifierExpression(
                                this.converter.Resolve(method),
                                scope);
                        }
                    }
                }
            }

            throw new ApplicationException(
                string.Format("Method [{0}]{1}::{2}(...) not found",
                    typeName.Item1,
                    typeName.Item2,
                    methodName));
        }
    }
}