//-----------------------------------------------------------------------
// <copyright file="ReferenceTypeConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.TypeSystemConverter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cs2JsC.CLR;
    using Cs2JsC.Converter.ExpressionsConverter;
    using Cs2JsC.JST;
    using Mono.Cecil;

    /// <summary>
    /// Definition for ReferenceTypeConverter
    /// </summary>
    public class ReferenceTypeConverter : TypeConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceTypeConverter"/> class.
        /// </summary>
        /// <param name="runtimeManager">The runtime manager.</param>
        /// <param name="typeDefinition">The type definition.</param>
        /// <param name="isSelectiveInit">if set to <c>true</c> [is selective init].</param>
        public ReferenceTypeConverter(
            RuntimeScopeManager runtimeManager,
            TypeDefinition typeDefinition,
            bool isSelectiveInit)
            : base(runtimeManager, typeDefinition, isSelectiveInit)
        {
        }

        /// <summary>
        /// Gets the type registration method.
        /// </summary>
        protected override MethodReference TypeRegistrationMethod
        {
            get { return this.Context.KnownReferences.RegisterReferenceTypeMethod; }
        }

        /// <summary>
        /// Initializes the static functions.
        /// </summary>
        /// <param name="statements">The statements.</param>
        protected override void InitializeStaticFunctions(List<Statement> statements)
        {
            base.InitializeStaticFunctions(statements);

            if (!this.TypeDefinition.IsAbstract)
            {
                this.CreateFactories(statements);
            }

            // Here we spit out all the functions that are marked as MakeStaticUsage.
            foreach (var function in this.TypeDefinition.Methods)
            {
                if (function.CustomAttributes.SelectAttribute(
                    this.Context.KnownReferences.MakeStaticUsageAttribute) == null)
                {
                    continue;
                }

                // Anything that's not marked with MakeStaticUsage Attribute should be processed elsewhere.
                MethodConverter methodConverter = this.GetMethodConverter(function);

                if (methodConverter != null)
                {
                    statements.Add(
                        new ExpressionStatement(
                            null,
                            this.Scope,
                            new BinaryExpression(
                                null,
                                this.Scope,
                                BinaryOperator.Assignment,
                                IdentifierExpression.Create(
                                    null,
                                    this.Scope,
                                    this.ResolveStaticMember(function)),
                                methodConverter.MethodFunctionExpression)));
                }
            }
        }

        /// <summary>
        /// Gets the prototype initializers.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        /// <returns>
        /// List of prototype index and initializer expression pair.
        /// </returns>
        protected override List<Tuple<Expression, Expression>> GetPrototypeInitializers(Expression prototype)
        {
            List<Tuple<Expression, Expression>> rv = new List<Tuple<Expression, Expression>>();

            foreach (var field in this.TypeDefinition.Fields)
            {
                if (field.IsStatic
                    || field.HasConstant
                    || !this.IsFieldImplemented(field))
                {
                    continue;
                }

                rv.Add(
                    Tuple.Create<Expression, Expression>(
                        new IdentifierExpression(
                            this.Resolve(field),
                            this.Scope),
                        DefaultValueConverter.GetDefaultValue(
                            this,
                            this.RuntimeManager,
                            this.Scope,
                            field.FieldType)));
            }

            foreach (var function in this.TypeDefinition.Methods)
            {
                if (function.IsStatic
                    || function.CustomAttributes.SelectAttribute(
                        this.Context.KnownReferences.MakeStaticUsageAttribute) != null)
                {
                    continue;
                }

                if (function.IsConstructor
                    && this.Context.IsExtended(this.TypeDefinition))
                {
                    continue;
                }

                MethodConverter methodConverter = this.GetMethodConverter(function);

                if (methodConverter != null
                    && !methodConverter.IsGlobalStaticImplementation)
                {
                    rv.Add(
                        Tuple.Create<Expression, Expression>(
                            new IdentifierExpression(
                                this.Resolve(function),
                                this.Scope),
                            methodConverter.MethodFunctionExpression));
                }
            }

            return rv;
        }

        /// <summary>
        /// Initializes the virtuals.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        /// <returns>List of expressions each initializing one field in prototype.</returns>
        protected override List<ExpressionStatement> InitializeVirtuals(
            Expression prototype)
        {
            List<ExpressionStatement> rv = new List<ExpressionStatement>();

            foreach (var kvPair in this.TypeDefinition.GetInterfaceOverrides(this.Context.ClrContext))
            {
                MethodReference virtualBase = kvPair.Key;
                MethodDefinition method = kvPair.Value;
                Expression value = null;
                TypeDefinition virtualBaseTypeDef = virtualBase.DeclaringType.Resolve();

                // If virtualBase is not being tracked, continue.
                if (this.IsSelectiveInit
                    && !this.RuntimeManager.DependencyAnalyzer.TypeToTypeReferences.ContainsKey(virtualBaseTypeDef))
                { continue; }

                if (virtualBaseTypeDef.IsInterface
                    && !(method.IsFinal && method.IsNewSlot))
                {
                    // Since all the overrides to this method won't be redirecting this pointer,
                    // we need to create method that will redirect to the the virtual method
                    // on current type.
                    IdentifierScope tempScope =
                        new IdentifierScope(
                            this.Scope,
                            method.Parameters.Count + method.GenericParameters.Count);

                    FunctionExpression redirectorFunc =
                        new FunctionExpression(
                            null,
                            this.Scope,
                            tempScope,
                            tempScope.ParameterIdentifiers,
                            null);

                    Expression redirectMethodCall =
                        new MethodCallExpression(
                            null,
                            tempScope,
                            new IndexExpression(
                                null,
                                tempScope,
                                new ThisExpression(null, tempScope),
                                new IdentifierExpression(
                                    this.TypeScopeManager.ResolveVirtualMethod(method), this.Scope)),
                            tempScope.ParameterIdentifiers.Select(
                                (p) => new IdentifierExpression(p, tempScope)).ToArray());

                    redirectorFunc.AddStatement(
                        new ReturnStatement(
                            null,
                            tempScope,
                            redirectMethodCall));

                    value = redirectorFunc;
                }
                else
                {
                    MethodConverter methodConverter = this.GetMethodConverter(method);

                    if (methodConverter != null)
                    {
                        if (methodConverter.IsGlobalStaticImplementation)
                        {
                            value = this.MapClassVirtual(
                                prototype,
                                virtualBase,
                                method);
                        }
                        else
                        {
                            // Since we are either overriding a virtual method or we are implementing non virtual interface
                            // method, we don't really need to implement redirect function.
                            value =
                                new IndexExpression(
                                    null,
                                    this.Scope,
                                    prototype,
                                    new IdentifierExpression(
                                        this.TypeScopeManager.ResolveMethod(method), this.Scope));
                        }
                    }
                }

                if (value != null)
                {
                    rv.Add(
                        ExpressionStatement.CreateAssignmentExpression(
                            new IndexExpression(
                                null,
                                this.Scope,
                                prototype,
                                this.ResolveVirtualMethod(
                                    virtualBase,
                                    this.Scope,
                                    this.Resolve)),
                            value));
                }
            }

            foreach (var method in this.TypeDefinition.Methods)
            {
                if (!method.IsVirtual)
                {
                    continue;
                }

                if (method.IsNewSlot
                    && !method.IsFinal
                    && this.GetMethodConverter(method) != null)
                {
                    // This is first virtual function and opening new virtual slot. So let's
                    // also fix the virtual method for this function.
                    bool isFixedName, isScriptAlias;
                    this.Context.GetMemberName(
                        method,
                        false,
                        out isFixedName,
                        out isScriptAlias);

                    Expression virtualFunctionExpression =
                        new IndexExpression(
                            null,
                            this.Scope,
                            prototype,
                            new IdentifierExpression(this.TypeScopeManager.ResolveVirtualMethod(method), this.Scope));

                    Expression implementedFunctionExpress =
                        new IndexExpression(
                            null,
                            this.Scope,
                            prototype,
                            new IdentifierExpression(this.TypeScopeManager.ResolveMethod(method), this.Scope));

                    if (isFixedName)
                    {
                        // If we have fixed name for the virtual function slot, then we have to backup
                        // our function in implemented function slot.
                        // So let's swap these 2 expressions.
                        var tmp = virtualFunctionExpression;
                        virtualFunctionExpression = implementedFunctionExpress;
                        implementedFunctionExpress = tmp;
                    }

                    rv.Add(
                        ExpressionStatement.CreateAssignmentExpression(
                            virtualFunctionExpression,
                            implementedFunctionExpress));
                }
            }

            return rv;
        }

        /// <summary>
        /// Registers the type internal.
        /// </summary>
        /// <param name="typeNameExpression">The type name expression.</param>
        /// <param name="parentTypeExpression">The parent type expression.</param>
        /// <param name="interfaces">The interfaces.</param>
        /// <returns>
        /// Type registration expression.
        /// </returns>
        protected override IList<Expression> RegisterTypeMethodArguments(
            Expression typeNameExpression,
            Expression parentTypeExpression,
            List<Expression> interfaces)
        {
            return new Expression[]
            {
                typeNameExpression,
                parentTypeExpression,
                new InlineNewArrayInitialization(
                    null,
                    this.Scope,
                    interfaces)
            };
        }

        /// <summary>
        /// Creates the factories.
        /// </summary>
        /// <param name="statements">The statements.</param>
        private void CreateFactories(List<Statement> statements)
        {
            if (!this.Context.IsJsonType(this.TypeDefinition)
                && !this.TypeDefinition.IsAbstract)
            {
                foreach (var function in this.TypeDefinition.Methods)
                {
                    MethodConverter methodConverter = this.GetMethodConverter(function);
                    if (!function.HasThis 
                        || function.Name != ".ctor"
                        || methodConverter == null)
                    {
                        if (!function.HasParameters
                            && function.IsConstructor
                            && this.MethodsAdded.Contains(function)
                            && function.IsPublic
                            && methodConverter == null)
                        {
                            // This is a default constructor for an imported or extended class.
                        }
                        else
                        {
                            continue;
                        }
                    }

                    List<string> argumentNames = new List<string>();
                    foreach (var parameter in function.Parameters)
                    {
                        argumentNames.Add(parameter.Name);
                    }

                    IdentifierScope functionScope = new IdentifierScope(
                        this.Scope,
                        argumentNames,
                        false);

                    IList<IIdentifier> functionNameExpr = this.ResolveFactory(function, this.Resolve);
                    IIdentifier functionName;

                    if (functionNameExpr.Count > 1)
                    {
                        functionName = SimpleIdentifier.CreateScopeIdentifier(
                            this.Scope,
                            string.Format("{0}_factory", this.TypeDefinition.FullName).Replace('.', '_'),
                            false);
                    }
                    else
                    {
                        functionName = functionNameExpr[0];
                    }

                    FunctionExpression factoryFunction = new FunctionExpression(
                        null,
                        this.Scope,
                        functionScope,
                        functionScope.ParameterIdentifiers,
                        functionName);

                    // If we extend object, and instruction count is 3, this means that
                    // this is empty constructor.
                    if (methodConverter == null
                        || (function.Body.Instructions.Count == 3
                            && this.Context.ClrKnownReferences.Object.IsSameDefinition(this.TypeDefinition.BaseType)))
                    {
                        factoryFunction.AddStatement(
                            new ReturnStatement(
                                null,
                                functionScope,
                                new NewObjectExpression(
                                    null,
                                    functionScope,
                                    IdentifierExpression.Create(
                                        null,
                                        functionScope,
                                        this.Resolve(this.TypeDefinition)))));
                    }
                    else
                    {
                        IdentifierExpression thisObjectExpression = new IdentifierExpression(
                            SimpleIdentifier.CreateScopeIdentifier(functionScope, "this_", false),
                            functionScope);

                        BinaryExpression thisAssignmentExpression = new BinaryExpression(
                            null,
                            functionScope,
                            BinaryOperator.Assignment,
                            thisObjectExpression,
                            new NewObjectExpression(
                                null,
                                functionScope,
                                IdentifierExpression.Create(
                                    null,
                                    functionScope,
                                    this.Resolve(this.TypeDefinition))));

                        factoryFunction.AddStatement(
                            new ExpressionStatement(
                                null,
                                functionScope,
                                thisAssignmentExpression));

                        MethodCallExpression constructorCallExpression = new MethodCallExpression(
                            null,
                            functionScope,
                            new IndexExpression(
                                null,
                                functionScope,
                                thisObjectExpression,
                                new IdentifierExpression(
                                    this.Resolve(function),
                                    functionScope)),
                                functionScope.ParameterIdentifiers.Select(arg => new IdentifierExpression(arg, functionScope)).ToArray());

                        factoryFunction.AddStatement(
                            new ExpressionStatement(
                                null,
                                functionScope,
                                constructorCallExpression));

                        factoryFunction.AddStatement(
                            new ReturnStatement(
                                null,
                                functionScope,
                                thisObjectExpression));
                    }

                    if (functionNameExpr.Count > 1)
                    {
                        statements.Add(
                            new ExpressionStatement(
                                null,
                                this.Scope,
                                new BinaryExpression(
                                    null,
                                    functionScope,
                                    BinaryOperator.Assignment,
                                    IdentifierExpression.Create(
                                        null,
                                        functionScope,
                                        this.ResolveStaticMember(function)),
                                    factoryFunction)));
                    }
                    else
                    {
                        statements.Add(
                            new ExpressionStatement(
                            null,
                            this.Scope,
                            factoryFunction));

                        if (function.Parameters.Count == 0)
                        {
                            statements.Add(
                                ExpressionStatement.CreateAssignmentExpression(
                                    new IndexExpression(
                                        null,
                                        this.Scope,
                                        IdentifierExpression.Create(
                                            null,
                                            this.Scope,
                                            this.Resolve(this.TypeDefinition)),
                                        new IdentifierExpression(this.Resolve(this.Context.KnownReferences.GetDefaultConstructorMethod), this.Scope)),
                                    new IdentifierExpression(functionName, this.Scope)));
                        }
                    }
                }
            }
        }
    }
}