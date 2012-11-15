//-----------------------------------------------------------------------
// <copyright file="StructTypeConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.TypeSystemConverter
{
    using System;
    using System.Collections.Generic;
    using Cs2JsC.CLR;
    using Cs2JsC.Converter.ExpressionsConverter;
    using Cs2JsC.JST;
    using Mono.Cecil;

    /// <summary>
    /// Definition for StructTypeConverter
    /// </summary>
    public class StructTypeConverter : TypeConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeConverter"/> class.
        /// </summary>
        /// <param name="scopeManager">The scope manager.</param>
        /// <param name="typeDefinition">The type definition.</param>
        /// <param name="isSelectiveInit">if set to <c>true</c> [is selective init].</param>
        public StructTypeConverter(
            RuntimeScopeManager scopeManager,
            TypeDefinition typeDefinition,
            bool isSelectiveInit)
            : base(scopeManager, typeDefinition, isSelectiveInit)
        {
        }

        /// <summary>
        /// Gets a value indicating whether all methods are implemented as static methods.
        /// </summary>
        /// <value>
        /// <c>true</c> if all methods need to be implemented as static methods; otherwise, <c>false</c>.
        /// </value>
        public override bool AllStaticMethods
        { get { return true; } }

        /// <summary>
        /// Creates the constructor function.
        /// </summary>
        /// <returns>Returns constructor function.</returns>
        protected override Expression CreateConstructorFunction(IIdentifier typeName)
        {
            FunctionExpression objExpression;
            IdentifierScope innerScope =
                new IdentifierScope(
                    this.Scope,
                    new string[] { "boxedValue" },
                    false);

            objExpression = new FunctionExpression(
                null,
                this.Scope,
                innerScope,
                innerScope.ParameterIdentifiers,
                typeName);

            objExpression.AddStatement(
                ExpressionStatement.CreateAssignmentExpression(
                    new IndexExpression(
                        null,
                        innerScope,
                        new ThisExpression(null, innerScope),
                        new IdentifierExpression(
                            this.Resolve(this.Context.KnownReferences.BoxedValueField), innerScope)),
                    new IdentifierExpression(
                        innerScope.ParameterIdentifiers[0],
                        innerScope)));

            return objExpression;
        }

        /// <summary>
        /// Gets the type registration method.
        /// </summary>
        protected override MethodReference TypeRegistrationMethod
        {
            get { return this.Context.KnownReferences.RegisterStructTypeMethod; }
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
            return new List<Tuple<Expression, Expression>>();
        }

        /// <summary>
        /// Initializes the static functions.
        /// </summary>
        /// <param name="statements">The statements.</param>
        protected override void InitializeStaticFunctions(List<Statement> statements)
        {
            statements.Add(this.GetDefaultValueFunction());

            base.InitializeStaticFunctions(statements);

            foreach (var function in this.TypeDefinition.Methods)
            {
                if (function.IsStatic)
                {
                    continue;
                }

                MethodConverter methodConverter = this.GetMethodConverter(function);

                if (methodConverter != null
                    && !methodConverter.IsGlobalStaticImplementation)
                {
                    statements.Add(
                        ExpressionStatement.CreateAssignmentExpression(
                            IdentifierExpression.Create(
                                null,
                                this.Scope,
                                this.ResolveStaticMember(function)),
                            methodConverter.MethodFunctionExpression));
                }
            }
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
            var interfaceToOverrideMap = this.TypeDefinition.GetInterfaceOverrides(this.Context.ClrContext);

            foreach (var kvPair in interfaceToOverrideMap)
            {
                if (this.GetMethodConverter(kvPair.Value) == null)
                {
                    continue;
                }

                rv.Add(
                    this.MapToVirtual(
                        prototype,
                        kvPair.Key,
                        kvPair.Value));
            }

            foreach (var method in this.TypeDefinition.Methods)
            {
                if (!method.IsVirtual
                    || this.GetMethodConverter(method) == null)
                {
                    continue;
                }

                if (method.HasOverrides)
                {
                    foreach (var virtualBase in method.Overrides)
                    {
                        if (!interfaceToOverrideMap.ContainsKey(virtualBase))
                        {
                            rv.Add(this.MapToVirtual(prototype, virtualBase, method));
                        }
                    }
                }

                if (method.IsNewSlot
                    && !method.IsFinal)
                {
                    // This is first virtual function and opening new virtual slot. So let's
                    // also fix the virtual method for this function.
                    rv.Add(
                        ExpressionStatement.CreateAssignmentExpression(
                            new IndexExpression(
                                null,
                                this.Scope,
                                prototype,
                                new IdentifierExpression(this.TypeScopeManager.ResolveVirtualMethod(method), this.Scope)),
                            new IndexExpression(
                                null,
                                this.Scope,
                                prototype,
                                new IdentifierExpression(this.TypeScopeManager.ResolveMethod(method), this.Scope))));
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
                new InlineNewArrayInitialization(
                    null,
                    this.Scope,
                    interfaces)
            };
        }

        /// <summary>
        /// Gets the default value function.
        /// </summary>
        /// <returns>Default value function.</returns>
        protected virtual Statement GetDefaultValueFunction()
        {
            Expression thisExpression = IdentifierExpression.Create(
                null,
                this.Scope,
                this.Resolve(this.TypeDefinition));

            IndexExpression defaultValueFunction = new IndexExpression(
                null,
                this.Scope,
                thisExpression,
                new IdentifierExpression(
                    this.Resolve(this.Context.KnownReferences.GetDefaultMethod),
                    this.Scope));

            IdentifierScope funcScope = new IdentifierScope(this.Scope);
            FunctionExpression functionExpression =
                new FunctionExpression(
                    null,
                    this.Scope,
                    funcScope,
                    new IIdentifier[0],
                    null);

            functionExpression.AddStatement(
                new ReturnStatement(
                    null,
                    funcScope,
                    DefaultValueConverter.GetDefaultValue(
                        this,
                        this.RuntimeManager,
                        this.Scope,
                        this.TypeDefinition)));

            return ExpressionStatement.CreateAssignmentExpression(
                    defaultValueFunction,
                    functionExpression);
        }

        /// <summary>
        /// Maps to virtual.
        /// </summary>
        /// <param name="statements">The statements.</param>
        /// <param name="prototype">The prototype.</param>
        /// <param name="virtualBase">The virtual base.</param>
        /// <param name="method">The method.</param>
        private ExpressionStatement MapToVirtual(
            Expression prototype,
            MethodReference virtualBase,
            MethodDefinition method)
        {
            return
                ExpressionStatement.CreateAssignmentExpression(
                    new IndexExpression(
                        null,
                        this.Scope,
                        prototype,
                        this.ResolveVirtualMethod(
                            virtualBase,
                            this.Scope,
                            this.Resolve)),
                    this.MapClassVirtual(prototype, virtualBase, method));
        }
    }
}