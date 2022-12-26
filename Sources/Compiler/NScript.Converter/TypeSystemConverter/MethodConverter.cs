//-----------------------------------------------------------------------
// <copyright file="MethodConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Mono.Cecil;
using NScript.CLR;
using NScript.CLR.AST;
using NScript.Converter.StatementsConverter;
using NScript.JSParser;
using NScript.JST;
using NScript.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryExpression = NScript.JST.BinaryExpression;
using BinaryOperator = NScript.JST.BinaryOperator;
using Expression = NScript.CLR.AST.Expression;
using ExpressionStatement = NScript.JST.ExpressionStatement;
using MethodCallExpression = NScript.JST.MethodCallExpression;
using ReturnStatement = NScript.JST.ReturnStatement;
using ScopeBlock = NScript.CLR.AST.ScopeBlock;
using Statement = NScript.JST.Statement;

namespace NScript.Converter.TypeSystemConverter
{
    /// <summary>
    /// Definition for MethodConverter
    /// </summary>
    public class MethodConverter : IResolver, IMethodScopeConverter
    {
        /// <summary>
        /// This argument.
        /// </summary>
        public const string ThisArgument = "this_";

        /// <summary>
        /// Stack of all the argumentVariablesIdentifierMap tracker when converting CSAst to JSAst.
        /// </summary>
        private readonly LinkedList<Dictionary<string, IIdentifier>> argumentVariableToIdentifierMapStack =
            new LinkedList<Dictionary<string, IIdentifier>>();

        /// <summary>
        /// Backing store for Clr Known references.
        /// </summary>
        private readonly ClrKnownReferences clrKnownReferences;

        /// <summary>
        /// Backing store for ConverterKnownReferences.
        /// </summary>
        private readonly ConverterKnownReferences cnvtKnownReferences;

        /// <summary>
        /// Backing store for ConverterContext.
        /// </summary>
        private readonly ConverterContext context;

        /// <summary>
        /// Queue for free temp variables.
        /// </summary>
        private readonly Queue<IIdentifier> freeTempVariables = new Queue<IIdentifier>();

        /// <summary>
        /// Tracker for if this method has Generic Arguments.
        /// </summary>
        private readonly bool hasGenericArguments;

        /// <summary>
        /// Dictionary for keeping track of local TypeReferences (typeReferences with scope
        /// of MethodReference).
        /// </summary>
        private readonly Dictionary<TypeReference, IIdentifier> localTypeReferences =
            new Dictionary<TypeReference, IIdentifier>(MemberReferenceComparer.Instance);

        /// <summary>
        /// Stack of all the localVariablesIdentifierMap tracker when converting CSAst to JSAst.
        /// </summary>
        private readonly LinkedList<Dictionary<string, IIdentifier>> localVariableToIdentifierMapStack =
            new LinkedList<Dictionary<string, IIdentifier>>();

        /// <summary>
        /// Backing field for MethodDefinition.
        /// </summary>
        private readonly MethodDefinition methodDefinition;

        /// <summary>
        /// Backing field for MethodFunctionExpression.
        /// </summary>
        private readonly FunctionExpression methodFunctionExpression;

        /// <summary>
        /// Backing field for MethodScope.
        /// </summary>
        private readonly IdentifierScope methodScope;

        /// <summary>
        /// Stack of all the alive parameterBlocks when converting ASTs.
        /// </summary>
        private readonly LinkedList<ParameterBlock> parameterBlocksStack =
            new LinkedList<ParameterBlock>();

        /// <summary>
        /// Replacers for replacing given expression with other expression.
        /// </summary>
        private readonly List<Tuple<Expression, Func<IdentifierScope, JST.Expression>>[]> replacers =
            new List<Tuple<Expression, Func<IdentifierScope, JST.Expression>>[]>();

        /// <summary>
        /// Stack of all the alive ScopeBlocks when converting.
        /// </summary>
        private readonly LinkedList<ScopeBlock> scopeBlocksStack =
            new LinkedList<ScopeBlock>();

        /// <summary>
        /// Stack of all the alive identifierScopes when converting CSAst to JSAst.
        /// </summary>
        private readonly LinkedList<IdentifierScope> scopeStack =
            new LinkedList<IdentifierScope>();

        /// <summary>
        /// Tempvariables used for instatement operations. These are mostly
        /// used around expansion of OpAssignments or Property post/pre fix operations.
        /// </summary>
        private readonly List<IIdentifier> statementTemporaryVariables =
            new List<IIdentifier>();

        /// <summary>
        /// Backing field for TypeConverter.
        /// </summary>
        private readonly TypeConverter typeConverter;

        /// <summary>
        /// Stack of all the variableNameGiven (for temp) tracker when converting CSAst to JSAst.
        /// </summary>
        private readonly LinkedList<Dictionary<string, int>> variableNamesGivenStack =
            new LinkedList<Dictionary<string, int>>();

        /// <summary>
        /// Variable tracking number delegates declared in this function.
        /// </summary>
        private int delegateCount;

        /// <summary>
        /// Backing store for thisIdentifier.
        /// </summary>
        private IIdentifier thisIdentifier;

        /// <summary>
        /// Set if we are using AST out of Mono compiler.
        /// </summary>
        private bool usingMcs;

        /// <summary>
        /// true to initialize wrapper done.
        /// </summary>
        private bool initializeWrapperDone;

        private IIdentifier conditionalAccessTempVariable = null;

        private BlockKind _kind;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodConverter"/> class.
        /// </summary>
        /// <param name="typeConverter">The type converter.</param>
        /// <param name="methodDefinition">The method definition.</param>
        public MethodConverter(
            TypeConverter typeConverter,
            MethodDefinition methodDefinition)
        {
            this.typeConverter = typeConverter;
            this.methodDefinition = methodDefinition;
            context = typeConverter.RuntimeManager.Context;
            clrKnownReferences = context.ClrKnownReferences;
            cnvtKnownReferences = context.KnownReferences;
            hasGenericArguments = context.HasGenericArguments(methodDefinition);

            if (methodDefinition.HasGenericParameters
                && !hasGenericArguments
                && methodDefinition.IsVirtual)
            {
                throw new ApplicationException(
                    string.Format(
                        "Virtual method ({0}) can't have IgnoreGenericArguments.",
                        methodDefinition));
            }

            var argumentNames = new List<string>();

            if (hasGenericArguments)
            {
                argumentNames.AddRange(methodDefinition.GenericParameters.Select(g => g.Name));
            }

            if (HasStaticImplementation
                && this.methodDefinition.HasThis
                && !IsConstructor)
            {
                argumentNames.Add(ThisArgument);
            }

            var argumentsStartIndex = argumentNames.Count;
            argumentNames.AddRange(methodDefinition.Parameters.Select(a => a.Name));

            methodScope = new IdentifierScope(
                typeConverter.Scope,
                argumentNames,
                false);

            scopeStack.AddFirst(methodScope);

            if (hasGenericArguments)
            {
                for (var genericArgumentIndex = 0;
                     genericArgumentIndex < this.methodDefinition.GenericParameters.Count;
                     genericArgumentIndex++)
                {
                    localTypeReferences.Add(
                        this.methodDefinition.GenericParameters[genericArgumentIndex],
                        methodScope.ParameterIdentifiers[genericArgumentIndex]);
                }
            }

            argumentVariableToIdentifierMapStack.AddFirst(new Dictionary<string, IIdentifier>());
            for (var argumentIndex = 0;
                 argumentIndex < methodDefinition.Parameters.Count;
                 argumentIndex++)
            {
                argumentVariableToIdentifierMapStack.First.Value.Add(
                    methodDefinition.Parameters[argumentIndex].Name,
                    methodScope.ParameterIdentifiers[argumentsStartIndex + argumentIndex]);
            }

            if (HasStaticImplementation
                && this.methodDefinition.HasThis)
            {
                if (this.methodDefinition.GenericParameters.Count != argumentsStartIndex)
                {
                    thisIdentifier = methodScope.ParameterIdentifiers[
                        this.methodDefinition.GenericParameters.Count];
                }
                else if (this.methodDefinition.CustomAttributes.SelectAttribute(
                        KnownReferences.IgnoreGenericArgumentsAttribute) != null)
                {
                    thisIdentifier = methodScope.ParameterIdentifiers[0];
                }
                else
                {
                    thisIdentifier = SimpleIdentifier.CreateScopeIdentifier(
                        methodScope,
                        ThisArgument,
                        false);
                }
            }

            try
            {
                methodFunctionExpression = Convert();
            }
            catch (Exception ex)
            {
                if (!(ex is ApplicationException))
                {
                    throw new ApplicationException(
                        string.Format(
                            "Error hit when converting {0} method",
                            this.methodDefinition),
                        ex);
                }

                throw;
            }
        }

        public bool IsIterator => (_kind & BlockKind.Iterator) == BlockKind.Iterator;

        public bool IsAsync => (_kind & BlockKind.Async) == BlockKind.Async;

        /// <summary>
        /// Gets the scope.
        /// </summary>
        /// <value>The scope.</value>
        public IdentifierScope Scope => scopeStack.First.Value;

        /// <summary>
        /// Gets the runtime manager.
        /// </summary>
        /// <value>The runtime manager.</value>
        public RuntimeScopeManager RuntimeManager => typeConverter.RuntimeManager;

        /// <summary>
        /// Gets a value indicating whether this instance is constructor.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is constructor; otherwise, <c>false</c>.
        /// </value>
        public bool IsConstructor => methodDefinition.Name == ".ctor";

        /// <summary>
        /// Gets the CLR known references.
        /// </summary>
        public ClrKnownReferences ClrKnownReferences => clrKnownReferences;

        /// <summary>
        /// Gets the known references.
        /// </summary>
        public ConverterKnownReferences KnownReferences => cnvtKnownReferences;

        /// <summary>
        /// Gets the method definition.
        /// </summary>
        public MethodDefinition MethodDefinition => methodDefinition;

        /// <summary>
        /// Gets the method function expression.
        /// </summary>
        public FunctionExpression MethodFunctionExpression => methodFunctionExpression;

        /// <summary>
        /// Gets a value indicating whether this instance has static implementation.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has static implementation; otherwise, <c>false</c>.
        /// </value>
        public bool HasStaticImplementation => (methodDefinition.HasThis
                        && (context.IsExtended(typeConverter.TypeDefinition)
                            || context.IsPsudoType(typeConverter.TypeDefinition)
                            || typeConverter.AllStaticMethods
                            || (RuntimeManager.ImplementInstanceAsStatic
                                && !methodDefinition.DeclaringType.HasGenericParameters
                                && !methodDefinition.DeclaringType.IsInterface))
                        && methodDefinition.CustomAttributes.SelectAttribute(
                                KnownReferences.KeepInstanceUsageAttribute) == null)
                    || !methodDefinition.HasThis;

        /// <summary>
        /// Gets a value indicating whether this instance is global static implementation.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is global static implementation; otherwise, <c>false</c>.
        /// </value>
        public bool IsGlobalStaticImplementation => HasStaticImplementation
                    && !typeConverter.IsGenericLike;

        /// <summary>
        /// Generates a wrapper expression.
        /// </summary>
        /// <exception cref="InvalidProgramException"> Thrown when an invalid program error condition
        ///     occurs. </exception>
        /// <param name="expressionType"> Type of the expression. </param>
        /// <param name="expression">     The expression. </param>
        /// <param name="converter">      The converter. </param>
        /// <param name="scope">          The scope. </param>
        /// <returns>
        /// The wrapper expression.
        /// </returns>
        public static JST.MethodCallExpression GenerateWrapperExpression(
            TypeReference expressionType,
            JST.Expression expression,
            IMethodScopeConverter converter,
            IdentifierScope scope)
        {
            MethodReference constructorMethod = null;
            var knownReferences = converter.RuntimeManager.Context.KnownReferences;

            if (expressionType is ArrayType)
            {
                constructorMethod = knownReferences.GetArrayNativeArrayArgCtor(((ArrayType)expressionType).ElementType);
            }
            else
            {
                var genericInstanceType = expressionType as GenericInstanceType;
                if (genericInstanceType == null
                    || !genericInstanceType.ElementType.Resolve().IsSameDefinition(knownReferences.ListGeneric))
                {
                    throw new InvalidProgramException("Can't generate ImportedWrapper for properties of type other than Array and List");
                }

                constructorMethod = knownReferences.GetListNativeArrayArgCtor(genericInstanceType.GenericArguments[0]);
            }

            return new JST.MethodCallExpression(
                        null,
                        scope,
                        JST.IdentifierExpression.Create(null, scope, converter.ResolveFactory(constructorMethod)),
                        expression);
        }

        /// <summary>
        /// Generates an extration expression.
        /// </summary>
        /// <exception cref="InvalidProgramException"> Thrown when an invalid program error condition
        ///     occurs. </exception>
        /// <param name="expressionType"> Type of the expression. </param>
        /// <param name="expression">     The expression. </param>
        /// <param name="converter">      The converter. </param>
        /// <param name="scope">          The scope. </param>
        /// <returns>
        /// The extration expression.
        /// </returns>
        public static JST.MethodCallExpression GenerateExtrationExpression(
            TypeReference expressionType,
            JST.Expression expression,
            IMethodScopeConverter converter,
            IdentifierScope scope)
        {
            MethodReference converterMethod = null;
            var knownReferences = converter.RuntimeManager.Context.KnownReferences;

            if (expressionType is ArrayType)
            {
                converterMethod = knownReferences.GetNativeArrayFromArrayMethod(
                    ((ArrayType)expressionType).ElementType);
            }
            else
            {
                var genericInstanceType = expressionType as GenericInstanceType;
                if (genericInstanceType == null
                    || !genericInstanceType.ElementType.IsSameDefinition(knownReferences.ListGeneric))
                {
                    throw new InvalidProgramException("Can't generate ImportedWrapper for properties of type other than Array and List");
                }

                converterMethod = knownReferences.GetNativeArrayFromListMethod(genericInstanceType.GenericArguments[0]);
            }

            return new JST.MethodCallExpression(
                            null,
                            scope,
                            JST.IdentifierExpression.Create(
                                null,
                                scope,
                                converter.ResolveStaticMember(converterMethod)),
                            expression);
        }

        /// <summary>
        /// Determines whether this instance can implement the specified method definition.
        /// </summary>
        /// <param name="methodDefinition">The method definition.</param>
        /// <returns>
        /// <c>true</c> if this instance can implement the specified method definition; otherwise, <c>false</c>.
        /// </returns>
        public bool CanImplement(MethodDefinition methodDefinition)
        {
            if (methodDefinition.HasAssociatedMember())
            {
                var propertyDefinition = methodDefinition.GetPropertyDefinition();
                if (propertyDefinition != null
                    && context.IsIntrinsicProperty(propertyDefinition))
                {
                    return false;
                }
            }

            // Abstract classes may have Un implemented functions.
            if (!(methodDefinition.HasBody && methodDefinition.Body.Instructions.Count > 0)
                && methodDefinition.CustomAttributes.SelectAttribute(cnvtKnownReferences.ScriptAttribute) == null)
            {
                return false;
            }

            if (context.IsExtended(methodDefinition.DeclaringType)
                && methodDefinition.HasThis
                && methodDefinition.IsConstructor)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Resolves the local.
        /// </summary>
        /// <param name="localIndex">Index of the local.</param>
        /// <returns>Resolved identifier.</returns>
        public IIdentifier ResolveLocal(string localVariable) => ResolveLocalInternal(localVariable, false);

        public IIdentifier ResolveLocalFunction(string localVariable) => ResolveLocalInternal(localVariable, true);

        /// <summary>
        /// Resolves the argument.
        /// </summary>
        /// <param name="argumentIndex">Index of the argument.</param>
        /// <returns></returns>
        public IIdentifier ResolveArgument(string argumentName)
        {
            if (!TryResolveArgument(argumentName, out var rv))
            {
                throw new ArgumentException(argumentName + " not found");
            }

            return rv;
        }

        /// <summary>
        /// Resolves the this.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns></returns>
        public JST.Expression ResolveThis(IdentifierScope scope, Location loc)
        {
            if (thisIdentifier != null)
            {
                return new IdentifierExpression(
                    thisIdentifier,
                    scope);
            }

            return new ThisExpression(loc, scope);
        }

        /// <summary>
        /// Tries the resolve argument.
        /// </summary>
        /// <param name="argumentName">Name of the argument.</param>
        /// <param name="identifier">The identifier.</param>
        /// <returns>true if argument name exists, else false.</returns>
        public bool TryResolveArgument(
            string argumentName,
            out IIdentifier identifier)
        {
            foreach (var dict in argumentVariableToIdentifierMapStack)
            {
                if (dict.TryGetValue(argumentName, out identifier))
                {
                    return true;
                }
            }

            identifier = null;
            return false;
        }

        /// <summary>
        /// Resolves the specified type reference base.
        /// </summary>
        /// <param name="paramDef">The type reference base.</param>
        /// <returns>Identifier for givenType.</returns>
        public IList<IIdentifier> Resolve(
            TypeReference typeReference)
        {
            var typeScope = typeReference.GetGenericTypeScope();

            // Convert ByRef type to normal type. There is not need to keep it
            // as ByRef type, we do not ever care about byRef part of the type.
            // at least not while referencing the type.
            var byRefType = typeReference as ByReferenceType;
            if (byRefType != null)
            { typeReference = byRefType.ElementType; }

            if (typeScope.HasValue
                && typeScope.Value == GenericParameterType.Method)
            {
                // TODO: We currently do not treat implemented classes with IgnoreGenericArgument like
                // normal type.
                // var genericInstanceType = typeReference as GenericInstanceType;
                // bool requiresGenericParameter = genericInstanceType != null
                //     ? this.context.HasGenericArguments(genericInstanceType.ElementType.Resolve())
                //     : true;

                // // if the type being resolve does not require any generic parameters, there is no point in going forward.
                // if (!requiresGenericParameter)
                // { return typeConverter.Resolve(typeReference); }
                if (!hasGenericArguments)
                {
                    throw new ApplicationException(
                        string.Format(
                            "Can't access generic type ({0}) if they are ignored",
                            typeReference));
                }

                if (!localTypeReferences.TryGetValue(typeReference, out var rv))
                {
                    var strBuilder = new StringBuilder();

                    RuntimeScopeManager.CalculateFriendlyTypeReferenceName(
                        typeReference,
                        strBuilder,
                        (typeDefinitionBase, typeNameBuilder) =>
                        {
                            var typeName = RuntimeScopeManager.GetSplitName(
                                typeDefinitionBase.Name).Item2;

                            var apposIndex = typeName.LastIndexOf('`');
                            if (apposIndex > 0)
                            {
                                typeName = typeName.Substring(0, apposIndex);
                            }

                            typeNameBuilder.Append(typeName);
                        });

                    rv = SimpleIdentifier.CreateScopeIdentifier(
                        methodScope,
                        strBuilder.ToString(),
                        false);

                    localTypeReferences.Add(typeReference, rv);
                }

                return new[] { rv };
            }

            return typeConverter.Resolve(typeReference);
        }

        /// <summary>
        /// Resolves the type to expression.
        /// </summary>
        /// <param name="typeReference">The type reference.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="initializeRefsAndStaticCtor">The initialize refs and static ctor.</param>
        /// <returns>
        /// Expression for the type.
        /// </returns>
        public JST.Expression ResolveTypeToExpression(
            TypeReference typeReference,
            IdentifierScope scope,
            JST.Expression initializeRefsAndStaticCtor = null)
        {
            var genericArgument = typeReference as GenericParameter;

            if (genericArgument != null)
            {
                return IdentifierExpression.Create(
                    null,
                    scope,
                    Resolve(typeReference));
            }

            return RuntimeManager.ResolveTypeToExpression(
                typeReference,
                scope,
                ResolveTypeToExpression,
                initializeRefsAndStaticCtor);
        }

        /// <summary>
        /// Resolves the static member.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>Resolve static member</returns>
        public IList<IIdentifier> ResolveStaticMember(FieldReference member) => typeConverter.ResolveStaticMember(
                member,
                Resolve);

        /// <summary>
        /// Resolves the static member.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>Resolve static member</returns>
        public IList<IIdentifier> ResolveStaticMember(MethodReference member) => typeConverter.ResolveStaticMember(
                member,
                Resolve);

        public IList<IIdentifier> ResolveFactory(MethodReference constructor) => typeConverter.ResolveFactory(
                constructor,
                Resolve);

        /// <summary>
        /// Resolves the specified member reference.
        /// </summary>
        /// <param name="fieldReference">The member reference.</param>
        /// <returns>Identifier identifying the member.</returns>
        public IIdentifier Resolve(FieldReference fieldReference) => typeConverter.Resolve(fieldReference);

        /// <summary>
        /// Resolves the specified member reference.
        /// </summary>
        /// <param name="propertyReference">The member reference.</param>
        /// <returns>Identifier identifying the member.</returns>
        public IIdentifier Resolve(PropertyReference propertyReference) => typeConverter.Resolve(propertyReference);

        /// <summary>
        /// Resolves the specified member reference.
        /// </summary>
        /// <param name="memberReference">The member reference.</param>
        /// <returns>Identifier identifying the member.</returns>
        public IIdentifier Resolve(MethodReference memberReference) => typeConverter.Resolve(memberReference);

        /// <summary>
        /// Resolves the virtual.
        /// </summary>
        /// <param name="methodReference">The method reference.</param>
        /// <param name="scope">The scope.</param>
        /// <returns>Expression that will resolve in slot for the method.</returns>
        public JST.Expression ResolveVirtualMethod(
            MethodReference methodReference,
            IdentifierScope scope) => typeConverter.ResolveVirtualMethod(
                methodReference,
                scope,
                Resolve);

        /// <summary>
        /// Resolves the name of the method slot.
        /// </summary>
        /// <param name="methodReference">The method reference.</param>
        /// <param name="isVirtual">if set to <c>true</c> [is virtual].</param>
        /// <param name="scope">The scope.</param>
        /// <returns>Resolve methods slot name to string expression.</returns>
        public JST.Expression ResolveMethodSlotName(
            MethodReference methodReference,
            bool isVirtual,
            IdentifierScope scope)
        {
            if (isVirtual)
            {
                var rv = ResolveVirtualMethod(
                    methodReference,
                    scope);

                var idExperssion = rv as IdentifierExpression;
                if (idExperssion != null)
                {
                    rv = new IdentifierStringExpression(
                        null,
                        scope,
                        idExperssion);
                }

                return rv;
            }

            return new IdentifierStringExpression(
                null,
                scope,
                new IdentifierExpression(
                    Resolve(methodReference),
                    scope));
        }

        /// <summary>
        /// Creates the instance method call expression.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="methodReference">The method reference.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>
        /// Returns expression for methodCall.
        /// </returns>
        public JST.Expression CreateInstanceMethodCallExpression(
            Location location,
            IdentifierScope scope,
            JST.Expression instance,
            MethodReference methodReference,
            IList<JST.Expression> arguments)
        {
            if (methodReference.Resolve().DeclaringType.IsValueOrEnum())
            {
                // Currently only struct method calls are converted to static method calls.
                // In future calls to all methods may become static call.
                var methodIdentifier = IdentifierExpression.Create(
                    location,
                    scope,
                    ResolveStaticMember(methodReference));

                var newArguments = new List<JST.Expression>
                {
                    instance
                };
                if (arguments != null)
                {
                    newArguments.AddRange(arguments);
                }

                return new MethodCallExpression(
                    location,
                    scope,
                    methodIdentifier,
                    newArguments);
            }
            else
            {
                var methodIdentifier = new IndexExpression(
                    location,
                    scope,
                    instance,
                    new IdentifierExpression(
                        Resolve(methodReference),
                        scope));

                return new MethodCallExpression(
                    location,
                    scope,
                    methodIdentifier,
                    arguments);
            }
        }

        /// <summary>
        /// Gets the statement temp.
        /// </summary>
        /// <returns>Temporary variable for current statement.</returns>
        public IIdentifier GetTempVariable()
        {
            if (freeTempVariables.Count == 0)
            {
                IIdentifier identifier = SimpleIdentifier.CreateScopeIdentifier(
                    Scope,
                    string.Format("stmtTemp{0}", freeTempVariables.Count + 1),
                    false);

                statementTemporaryVariables.Add(identifier);
                freeTempVariables.Enqueue(identifier);
            }

            return freeTempVariables.Dequeue();
        }

        public IIdentifier GetConditionalAccessTempVariable()
            => conditionalAccessTempVariable =
                conditionalAccessTempVariable
                ?? SimpleIdentifier.CreateScopeIdentifier(
                    Scope,
                    string.Format("stmtTemp{0}", freeTempVariables.Count + 1),
                    false);

        /// <summary>
        /// Releases the temp variable.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public void ReleaseTempVariable(IIdentifier identifier) => freeTempVariables.Enqueue(identifier);

        /// <summary>
        /// Gets the replacement expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>Canned expression if replacer exists.</returns>
        public JST.Expression GetReplacementExpression(Expression expression)
        {
            for (var replacerScopeIndex = 0; replacerScopeIndex < replacers.Count; replacerScopeIndex++)
            {
                for (var replacerTupleIndex = 0;
                     replacerTupleIndex < replacers[replacerScopeIndex].Length;
                     replacerTupleIndex++)
                {
                    if (replacers[replacerScopeIndex][replacerTupleIndex].Item1.Equals(expression))
                    {
                        return replacers[replacerScopeIndex][replacerTupleIndex].Item2(Scope);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Pushes the expression replacement.
        /// </summary>
        /// <param name="expressionMappers">The expression mappers.</param>
        public void PushExpressionReplacement(
            params Tuple<Expression, Func<IdentifierScope, JST.Expression>>[] expressionMappers) => replacers.Add(expressionMappers);

        /// <summary>
        /// Pops the replacer.
        /// </summary>
        public void PopReplacer() => replacers.RemoveAt(replacers.Count - 1);

        /// <summary>
        /// Pushes the parameter block.
        /// </summary>
        /// <param name="parameterBlock">The parameter block.</param>
        /// <returns></returns>
        public JST.Expression ProcessParameterBlock(
            ParameterBlock parameterBlock,
            IIdentifier localMethodName)
        {
            scopeBlocksStack.AddFirst(parameterBlock);
            parameterBlocksStack.AddFirst(parameterBlock);
            variableNamesGivenStack.AddFirst(new Dictionary<string, int>());
            localVariableToIdentifierMapStack.AddFirst(new Dictionary<string, IIdentifier>());
            argumentVariableToIdentifierMapStack.AddFirst(new Dictionary<string, IIdentifier>());

            try
            {
                var parameterNames = new List<string>();
                foreach (var param in parameterBlock.Parameters)
                {
                    parameterNames.Add(param.Name);
                }

                var identifierScope = new IdentifierScope(
                    Scope,
                    parameterNames,
                    false);

                for (var iParam = 0; iParam < parameterNames.Count; iParam++)
                {
                    argumentVariableToIdentifierMapStack.First.Value.Add(
                        parameterNames[iParam],
                        identifierScope.ParameterIdentifiers[iParam]);
                }

                scopeStack.AddFirst(identifierScope);
                var statements = new List<Statement>();
                try
                {
                    foreach (var statement in parameterBlock.Statements)
                    {
                        var jsStatement =
                            StatementConverterBase.Convert(
                                this,
                                statement);

                        if (jsStatement != null)
                        {
                            statements.Add(jsStatement);
                        }
                    }
                }
                finally
                {
                    scopeStack.RemoveFirst();
                }

                var delegateFunctionNameId =
                    localMethodName
                    ?? SimpleIdentifier.CreateScopeIdentifier(
                        RuntimeManager.Scope,
                        string.Format(
                            "{0}_del{1}",
                            GetMethodName(methodDefinition).OriginalSuggestedName,
                            delegateCount++ == 0
                                ? string.Empty
                                : delegateCount.ToString()),
                        false);

                var rv = new FunctionExpression(
                    parameterBlock.Location,
                    Scope,
                    identifierScope,
                    identifierScope.ParameterIdentifiers,
                    delegateFunctionNameId,
                    parameterBlock.IsAsync);

                rv.AddStatements(statements);

                return rv;
            }
            finally
            {
                scopeBlocksStack.RemoveFirst();
                parameterBlocksStack.RemoveFirst();
                variableNamesGivenStack.RemoveFirst();
                localVariableToIdentifierMapStack.RemoveFirst();
                argumentVariableToIdentifierMapStack.RemoveFirst();
                conditionalAccessTempVariable = null;
            }
        }

        /// <summary>
        /// Pushes the scope block.
        /// </summary>
        /// <param name="scopeBlock">The scope block.</param>
        public void PushScopeBlock(ScopeBlock scopeBlock) => scopeBlocksStack.AddFirst(scopeBlock);

        /// <summary>
        /// Pops the scope block.
        /// </summary>
        public void PopScopeBlock() => scopeBlocksStack.RemoveFirst();

        /// <summary>
        /// Fixes the this variable.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <returns>fixes this keyword inside of script to this_ and this_ variables to other names.</returns>
        private static string FixThisVariable(string script)
        {
            var strBuilder = new StringBuilder();
            int lastIndex = 0, nowIndex = 0;
            var inStr = false;
            var strChar = (char)0;
            for (; nowIndex < script.Length; nowIndex++)
            {
                var ch = script[nowIndex];
                if (inStr)
                {
                    if (ch == '\\')
                    {
                        nowIndex++;
                    }
                    else if (ch == strChar)
                    {
                        CopyStringPart(script, lastIndex, nowIndex + 1, strBuilder);
                        lastIndex = nowIndex + 1;
                        inStr = false;
                    }
                    continue;
                }

                if ((ch >= '0' && ch <= '9')
                    || (ch >= 'a' && ch <= 'z')
                    || (ch >= 'A' && ch <= 'Z')
                    || ch == '_'
                    || ch == '$')
                {
                    continue;
                }

                CopyAfterFixing(script, lastIndex, nowIndex, strBuilder);
                strBuilder.Append(ch);
                lastIndex = nowIndex + 1;

                if (ch == '\'' || ch == '"')
                {
                    inStr = true;
                    strChar = ch;
                }
                else if (ch == '/')
                {
                    if (script.Length > nowIndex + 1)
                    {
                        if (script[nowIndex + 1] == '/')
                        {
                            do
                            {
                                nowIndex++;
                            } while (nowIndex < script.Length
                                && (script[nowIndex] != '\r' || script[nowIndex] != '\n'));

                            continue;
                        }
                        else if (script[nowIndex + 1] == '*')
                        {
                            var endComment = script.IndexOf("*/", nowIndex);
                            if (endComment < 0)
                            {
                                break;
                            }
                            else
                            {
                                nowIndex = endComment + 2;
                            }
                        }
                        else
                        {
                            continue;
                        }

                        CopyStringPart(script, lastIndex, nowIndex, strBuilder);
                    }
                }
            }

            CopyAfterFixing(script, lastIndex, script.Length, strBuilder);

            return strBuilder.ToString();
        }

        /// <summary>
        /// Copies the string part.
        /// </summary>
        /// <param name="fromString">From string.</param>
        /// <param name="startAt">The start at.</param>
        /// <param name="endAt">The end at.</param>
        /// <param name="strBuilder">The STR builder.</param>
        private static void CopyStringPart(string fromString, int startAt, int endAt, StringBuilder strBuilder)
        {
            for (; startAt < endAt && startAt < fromString.Length; startAt++)
            {
                strBuilder.Append(fromString[startAt]);
            }
        }

        /// <summary>
        /// Copies the after fixing.
        /// </summary>
        /// <param name="fromString">From string.</param>
        /// <param name="startAt">The start at.</param>
        /// <param name="endAt">The end at.</param>
        /// <param name="strBuilder">The STR builder.</param>
        private static void CopyAfterFixing(string fromString, int startAt, int endAt, StringBuilder strBuilder)
        {
            const string thisStr = "this";
            if (fromString.IndexOf(thisStr, startAt, endAt - startAt) != startAt)
            {
                CopyStringPart(fromString, startAt, endAt, strBuilder);
                return;
            }

            strBuilder.Append(thisStr);
            startAt += thisStr.Length;

            var matchFailed = false;
            for (var startIndex = startAt; startIndex < endAt && startIndex < fromString.Length; startIndex++)
            {
                if (fromString[startIndex] != '_')
                {
                    matchFailed = true;
                    break;
                }
            }

            CopyStringPart(fromString, startAt, endAt, strBuilder);

            if (!matchFailed)
            {
                strBuilder.Append('_');
            }
        }

        /// <summary>
        /// Gets the name of the method.
        /// </summary>
        /// <param name="methodReference">The method reference.</param>
        /// <returns>Method name identifier.</returns>
        private IIdentifier GetMethodName(MethodReference methodReference) => typeConverter.RuntimeManager.ResolveFunctionName(methodReference);

        /// <summary>
        /// Gets the constructor default initialization statement.
        /// </summary>
        /// <returns></returns>
        private Statement GetConstructorDefaultInitializationStatement() => new ExpressionStatement(
                null,
                Scope,
                new BinaryExpression(
                    null,
                    Scope,
                    BinaryOperator.Assignment,
                    ResolveThis(Scope, null),
                    new MethodCallExpression(
                        null,
                        Scope,
                        new IndexExpression(
                            null,
                            Scope,
                            IdentifierExpression.Create(
                                null,
                                Scope,
                                Resolve(typeConverter.TypeDefinition)),
                            new IdentifierExpression(
                                Resolve(cnvtKnownReferences.GetDefaultMethod), Scope)),
                        new JST.Expression[0])));

        /// <summary>
        /// Imports the js script.
        /// </summary>
        /// <returns>Function Expression after parsing js script.</returns>
        private List<Statement> ImportJsScript()
        {
            var scriptAttribute = methodDefinition.CustomAttributes.SelectAttribute(
                KnownReferences.ScriptAttribute);

            if (scriptAttribute == null
                || scriptAttribute.ConstructorArguments[0].Value == null)
            {
                throw new ApplicationException(
                    string.Format(
                        "Can't convert function {0}, it's extern and does not have script block",
                        methodDefinition));
            }

            JST.ScopeBlock scopeBlock = null;
            try
            {
                var script = (string)scriptAttribute.ConstructorArguments[0].Value;

                if (thisIdentifier != null)
                {
                    script = MethodConverter.FixThisVariable(script);
                }

                scopeBlock = Parser.Parse(
                    script,
                    Scope,
                    new JsniResolver(this));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    string.Format(
                        "Error hit when parsing function{0}\r\nExceptionInfo: ex.Message: {1}",
                        MethodDefinition,
                        ex.Message),
                    ex);
            }

            var returnValue = new List<Statement>();
            if (IsConstructor
                && thisIdentifier != null
                && Scope.UsedLocalIdentifiers.Contains(thisIdentifier))
            {
                // We are in struct constructor. So we need to initialize this.
                returnValue.Add(GetConstructorDefaultInitializationStatement());
            }

            if (HasWrappedField())
            {
                InitializeImportedWrapper(returnValue);
            }

            returnValue.AddRange(scopeBlock.Statements);
            return returnValue;
        }

        /// <summary>
        /// Converts this instance.
        /// </summary>
        /// <returns></returns>
        private FunctionExpression Convert()
        {
            var statements = InnerConvert();
            var functionExpression = GetFunctionExpressionShell();

            foreach (var plugin in context.MethodConverterPlugins)
            {
                switch (plugin.GetInterestLevel(methodDefinition, context))
                {
                    case IntrestLevel.PreEmitStatements:
                        statements.InsertRange(0, plugin.GetPreInsertionStatements(this));
                        break;

                    case IntrestLevel.PostEmitStatements:
                        statements.AddRange(plugin.GetPostInsertionStatements(this));
                        break;

                    case IntrestLevel.Encapsulate:
                        statements = plugin.GetEncapsulationStatements(this, statements);
                        break;

                    case IntrestLevel.Overwrite:
                        statements = plugin.GetOverwrite(this);
                        functionExpression.AddStatements(statements);
                        return functionExpression;

                    case IntrestLevel.None:
                    default:
                        break;
                }
            }

            if (IsIterator)
            {
                HandleIterator(functionExpression, statements);
            }
            else
            {
                functionExpression.AddStatements(statements);
            }

            return functionExpression;
        }

        private void HandleIterator(
            FunctionExpression outerFunction,
            List<Statement> statements)
        {
            scopeStack.AddFirst(new IdentifierScope(Scope));
            try
            {
                var generatorShell = GetGeneratorShell();
                generatorShell.AddStatements(statements);
                MethodReference ctor;
                JST.Expression jstCtor;

                if (MethodDefinition.ReturnType.IsSameDefinition(KnownReferences.IEnumerable)
                    || MethodDefinition.ReturnType.IsSameDefinition(KnownReferences.IEnumerator))
                {
                    ctor = KnownReferences.GeneratorWrapperCtor;
                    jstCtor = IdentifierExpression.Create(null, Scope, ResolveFactory(ctor));
                }
                else
                {
                    ctor = KnownReferences
                        .GeneratorWrapperGenericCtor
                        .FixGenericTypeArguments(MethodDefinition.ReturnType);
                    jstCtor = IdentifierExpression.Create(null, Scope, ResolveFactory(ctor));

                    if (MethodDefinition.ReturnType.ContainsGenericParameter)
                    {
                        var idfier = ResolveFactory(ctor)[0];
                        var ty = ResolveTypeToExpression(
                            KnownReferences.GeneratorWrapperGeneric.Resolve()
                            .FixGenericTypeArguments(
                                MethodDefinition.ReturnType),
                            Scope);

                        var tyCall = new MethodCallExpression(
                            null,
                            Scope,
                            ty,
                            MethodDefinition.ReturnType.GetGenericArguments().Select(
                                genericParam => IdentifierExpression.Create(null,
                                Scope, this.Resolve(genericParam)))
                            .ToArray());

                        var expr = new BinaryExpression(
                            null,
                            Scope,
                            BinaryOperator.Assignment,
                            new IdentifierExpression(idfier, Scope),
                            tyCall);

                        outerFunction.AddStatement(
                            new ExpressionStatement(null, outerFunction.Scope, expr));
                    }
                }

                var wrapperCallExpression = new MethodCallExpression(
                    null, Scope, jstCtor, generatorShell);

                outerFunction.AddStatement(
                    new JST.ReturnStatement(null, outerFunction.Scope, wrapperCallExpression));
            }
            finally
            {
                scopeStack.RemoveFirst();
            }
        }

        /// <summary>
        /// Gets the convert cst.
        /// </summary>
        /// <returns>
        /// object converted to a cst.
        /// </returns>
        private List<Statement> ConvertCST()
        {
            var statements = new List<Statement>();
            var rootBlock = GetRootBlock();

            if (HasWrappedField())
            {
                InitializeImportedWrapper(statements);
            }

            // This means that we have compiler implemented method for Property or Event.
            if (rootBlock == null)
            {
                GenerateCompilerImplemented(statements);
                return statements;
            }

            // Initialize all the scope stacks.
            // Note that argument stack has already been initialized.
            scopeBlocksStack.AddFirst(rootBlock);
            parameterBlocksStack.AddFirst(rootBlock);
            variableNamesGivenStack.AddFirst(new Dictionary<string, int>());
            localVariableToIdentifierMapStack.AddFirst(new Dictionary<string, IIdentifier>());

            // Check if thisIdentifier is really hoisted, then we need to copy this variable in another temporary variable
            if (thisIdentifier == null)
            {
                foreach (var param in rootBlock.Parameters)
                {
                    if (param is ThisVariable)
                    {
                        if (param.IsHoisted)
                        {
                            thisIdentifier = SimpleIdentifier.CreateScopeIdentifier(
                                Scope,
                                ThisArgument,
                                false);

                            statements.Add(
                                ExpressionStatement.CreateAssignmentExpression(
                                    new IdentifierExpression(thisIdentifier, Scope),
                                    new ThisExpression(null, Scope)));

                            break;
                        }
                    }
                }
            }

            foreach (var statement in rootBlock.Statements)
            {
                var jsStatement =
                    StatementConverterBase.Convert(
                        this,
                        statement);

                if (jsStatement != null)
                {
                    statements.Add(jsStatement);
                }
            }

            // Only struct constructors have return (essentially these constructors are factory to begin with).
            if (IsConstructor
                && typeConverter.AllStaticMethods
                && (statements.Count == 0
                    || !(statements[statements.Count - 1] is ReturnStatement)))
            {
                statements.Add(
                    new ReturnStatement(
                        null,
                        Scope,
                        ResolveThis(Scope, null)));
            }

            foreach (var genericArgIdPair in localTypeReferences)
            {
                var genericParam = genericArgIdPair.Key as GenericParameter;

                // Only process the typeReferences that are not generic arguments to this method.
                if (genericParam == null)
                {
                    statements.Insert(
                        0,
                        new ExpressionStatement(
                            null,
                            Scope,
                            new BinaryExpression(
                                null,
                                Scope,
                                BinaryOperator.Assignment,
                                new IdentifierExpression(genericArgIdPair.Value, Scope),
                                ResolveTypeToExpression(
                                    genericArgIdPair.Key,
                                    Scope,
                                    new BooleanLiteralExpression(
                                        Scope,
                                        true)))));
                }
            }

            if (IsConstructor && thisIdentifier != null)
            {
                // We are in struct constructor. So we need to initialize this.
                statements.Insert(
                    0,
                    GetConstructorDefaultInitializationStatement());
            }

            return statements;
        }

        /// <summary>
        /// Gets the inner convert.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        private List<Statement> InnerConvert()
        {
            if (context.IsWrapped(methodDefinition))
            {
                return GenerateWrapperImplementation();
            }

            if (!methodDefinition.HasBody
                || methodDefinition.Body.Instructions.Count == 0)
            {
                return ImportJsScript();
            }

            return ConvertCST();
        }

        private FunctionExpression GetGeneratorShell()
        {
            return new FunctionExpression(
                null,
                Scope,
                new IdentifierScope(Scope, new List<string>(), false),
                new List<IIdentifier>(),
                null,
                IsAsync,
                true);
        }

        /// <summary>
        /// Gets function expression shell.
        /// </summary>
        /// <returns>
        /// The function expression shell.
        /// </returns>
        private FunctionExpression GetFunctionExpressionShell()
        {
            var functionName = GetMethodName(methodDefinition);

            if (IsGlobalStaticImplementation)
            {
                if (IsConstructor && HasStaticImplementation)
                {
                    functionName = RuntimeManager.ResolveFactory(methodDefinition);
                }
                else
                {
                    functionName = RuntimeManager.ResolveStatic(methodDefinition);
                }
            }

            var returnValue = new FunctionExpression(
                null,
                typeConverter.Scope,
                Scope,
                Scope.ParameterIdentifiers,
                functionName,
                !IsIterator && IsAsync);

            return returnValue;
        }

        /// <summary>
        /// Generates the compiler implemented.
        /// </summary>
        /// <exception cref="InvalidProgramException"> Thrown when an Invalid Program error condition
        ///     occurs. </exception>
        /// <param name="returnValue"> The return value. </param>
        private void GenerateCompilerImplemented(List<Statement> returnValue)
        {
            if (HasWrappedField())
            {
                InitializeImportedWrapper(returnValue);
            }

            if (methodDefinition.IsSetter)
            {
                GenerateSetterImplementation(returnValue);
            }
            else if (methodDefinition.IsGetter)
            {
                GenerateGetterImplementation(returnValue);
            }
            else if (methodDefinition.IsAddOn
                || methodDefinition.IsRemoveOn)
            {
                GenerateAddonOrRemoveOnImplementation(returnValue);
            }
            else
            {
                throw new InvalidProgramException();
            }
        }

        /// <summary>
        /// Generates a wrapper implementation.
        /// </summary>
        /// <returns>
        /// The wrapper implementation.
        /// </returns>
        private List<Statement> GenerateWrapperImplementation()
        {
            var functionName = GetMethodName(methodDefinition);

            var returnValue = new List<Statement>();

            if (methodDefinition.IsSetter
                && !context.IsRenamed(methodDefinition))
            {
                GenerateSetterImportedWrapper(returnValue);
            }
            else if (methodDefinition.IsGetter
                && !context.IsRenamed(methodDefinition))
            {
                GenerateGetterImportedWrapper(returnValue);
            }
            else if (methodDefinition.IsAddOn
                || methodDefinition.IsRemoveOn)
            {
                GenerateAddonOrRemoveOnImportedWrapper(returnValue);
            }
            else
            {
                GenerateMethodWrapper(returnValue);
            }

            return returnValue;
        }

        /// <summary>
        /// Generates the addon or remove on implementation.
        /// </summary>
        /// <param name="functionExpression">The function expression.</param>
        private void GenerateAddonOrRemoveOnImplementation(List<Statement> statements)
        {
            var matchOffset = methodDefinition.IsAddOn
                ? "add_".Length
                : "remove_".Length;

            var fieldName = methodDefinition.Name.Substring(matchOffset);
            FieldReference fieldReference = typeConverter.TypeDefinition.Fields.First((fld) => fld.Name == fieldName);

            MethodReference addOrRemoveRef;
            if (methodDefinition.IsAddOn)
            {
                addOrRemoveRef = context.KnownReferences.DelegateCombineMethod;
            }
            else
            {
                addOrRemoveRef = context.KnownReferences.DelegateRemoveMethod;
            }

            var methodCallExpr = new JST.MethodCallExpression(
                null,
                Scope,
                JST.IdentifierExpression.Create(
                    null,
                    Scope,
                    ResolveStaticMember(addOrRemoveRef)),
                fieldReference.Resolve().IsStatic
                    ? JST.IdentifierExpression.Create(
                            null,
                            Scope,
                            ResolveStaticMember(fieldReference))
                    : new JST.IndexExpression(
                        null,
                        Scope,
                        ResolveThis(Scope, null),
                        new JST.IdentifierExpression(Resolve(fieldReference), Scope)),
                new JST.IdentifierExpression(
                    HasStaticImplementation && this.methodDefinition.HasThis
                    ? Scope.ParameterIdentifiers[1]
                    : Scope.ParameterIdentifiers[0],
                    Scope));

            statements.Add(
                JST.ExpressionStatement.CreateAssignmentExpression(
                    fieldReference.Resolve().IsStatic
                        ? JST.IdentifierExpression.Create(
                                null,
                                Scope,
                                ResolveStaticMember(fieldReference))
                        : new JST.IndexExpression(
                            null,
                            Scope,
                            ResolveThis(Scope, null),
                            new JST.IdentifierExpression(Resolve(fieldReference), Scope)),
                    methodCallExpr));
        }

        /// <summary>
        /// Generates an addon or remove on imported wrapper.
        /// </summary>
        /// <param name="statements"> The statements. </param>
        private void GenerateAddonOrRemoveOnImportedWrapper(List<Statement> statements)
        {
            var methodCallExpr = new JST.MethodCallExpression(
                null,
                Scope,
                JST.IdentifierExpression.Create(
                    null,
                    Scope,
                    ResolveStaticMember(
                        MethodDefinition.IsAddOn
                            ? context.KnownReferences.AddEventMethod
                            : context.KnownReferences.RemoveEventMethod)),
                new JST.IdentifierExpression(thisIdentifier, Scope),
                new JST.StringLiteralExpression(Scope, GetNativeEventName()),
                new JST.IdentifierExpression(ResolveArgument(methodDefinition.Parameters[0].Name), Scope),
                new JST.BooleanLiteralExpression(Scope, false));

            statements.Add(new ExpressionStatement(null, Scope, methodCallExpr));
        }

        /// <summary>
        /// Gets the native event name.
        /// </summary>
        /// <returns>
        /// The native event name.
        /// </returns>
        private string GetNativeEventName()
        {
            var eventDefinition = (EventDefinition)methodDefinition.GetAssociatedMember();
            var eventName = eventDefinition.Name;

            var attr = TypeHelpers.SelectAttribute(
                eventDefinition.CustomAttributes,
                KnownReferences.ScriptNameAttribute);

            if (attr != null)
            { return (string)attr.ConstructorArguments[0].Value; }

            if (eventName.StartsWith("On")
                && eventName.Length > 3
                && char.IsUpper(eventName, 2))
            {
                return eventName.Substring(2).ToLowerInvariant();
            }

            return eventName.ToLowerInvariant();
        }

        /// <summary>
        /// Generates the setter implementation.
        /// </summary>
        /// <param name="functionExpression">The function expression.</param>
        private void GenerateSetterImplementation(List<Statement> statements) => statements.Add(
                new JST.ReturnStatement(
                    null,
                    Scope,
                    new JST.BinaryExpression(
                        null,
                        Scope,
                        BinaryOperator.Assignment,
                       new JST.IndexExpression(
                           null,
                           Scope,
                           GetMethodSlotParentExpression(),
                           new JST.IdentifierExpression(
                               RuntimeManager.Resolve(
                                   methodDefinition.GetPropertyDefinition()), Scope)),
                       new JST.IdentifierExpression(
                           ResolveArgument(methodDefinition.Parameters[0].Name), Scope))));

        /// <summary>
        /// Generates the getter implementation.
        /// </summary>
        /// <param name="statements"> The statements. </param>
        private void GenerateGetterImplementation(List<Statement> statements) => statements.Add(
                new JST.ReturnStatement(
                    null,
                    Scope,
                    new JST.IndexExpression(
                        null,
                        Scope,
                        GetMethodSlotParentExpression(),
                        new JST.IdentifierExpression(
                            RuntimeManager.Resolve(
                                methodDefinition.GetPropertyDefinition()), Scope))));

        /// <summary>
        /// Generates a getter imported wrapper.
        /// </summary>
        /// <exception cref="InvalidProgramException"> Thrown when an invalid program error condition
        ///     occurs. </exception>
        /// <param name="functionExpression"> The function expression. </param>
        public void GenerateGetterImportedWrapper(List<Statement> statements)
        {
            var propertyDefinition = (PropertyDefinition)methodDefinition.GetAssociatedMember();
            var valueParameter = methodDefinition.ReturnType;

            if (methodDefinition.IsStatic)
            {
                JST.Expression valueExpression = new JST.ConditionalOperatorExpression(
                    null,
                    Scope,
                    new JST.IdentifierExpression(typeConverter.ResolveStaticMember(propertyDefinition), Scope),
                    MethodConverter.GenerateWrapperExpression(
                        valueParameter,
                        new JST.IdentifierExpression(typeConverter.ResolveStaticMember(propertyDefinition), Scope),
                        this,
                        Scope),
                    new JST.NullLiteralExpression(Scope));

                statements.Add(
                    new JST.ReturnStatement(
                        null,
                        Scope,
                        new JST.BinaryExpression(
                            null,
                            Scope,
                            BinaryOperator.Assignment,
                            new JST.IdentifierExpression(typeConverter.ResolveStaticMember(propertyDefinition), Scope),
                            new JST.BinaryExpression(
                                null,
                                Scope,
                                BinaryOperator.LogicalOr,
                                new JST.IdentifierExpression(typeConverter.ResolveStaticMember(propertyDefinition), Scope),
                                valueExpression))));
            }
            else
            {
                InitializeImportedWrapper(statements);

                JST.Expression valueExpression = new JST.ConditionalOperatorExpression(
                    null,
                    Scope,
                    new JST.IndexExpression(
                        null,
                        Scope,
                        ResolveThis(Scope, null),
                        new JST.IdentifierExpression(typeConverter.Resolve(propertyDefinition), Scope),
                        false),
                    MethodConverter.GenerateWrapperExpression(
                        valueParameter,
                        new JST.IndexExpression(
                            null,
                            Scope,
                            ResolveThis(Scope, null),
                            new JST.IdentifierExpression(typeConverter.Resolve(propertyDefinition), Scope),
                            false),
                        this,
                        Scope),
                    new JST.NullLiteralExpression(Scope));

                statements.Add(
                    new JST.ReturnStatement(
                        null,
                        Scope,
                        new JST.BinaryExpression(
                            null,
                            Scope,
                            BinaryOperator.Assignment,
                            new JST.IdentifierExpression(GetImportedWrapperIdentifier(propertyDefinition), Scope),
                            new JST.BinaryExpression(
                                null,
                                Scope,
                                BinaryOperator.LogicalOr,
                                new JST.IdentifierExpression(GetImportedWrapperIdentifier(propertyDefinition), Scope),
                                valueExpression))));
            }
        }

        /// <summary>
        /// Generates a setter imported wrapper.
        /// </summary>
        /// <exception cref="InvalidProgramException"> Thrown when an invalid program error condition
        ///     occurs. </exception>
        /// <param name="functionExpression"> The function expression. </param>
        private void GenerateSetterImportedWrapper(List<Statement> statements)
        {
            var propertyDefinition = (PropertyDefinition)methodDefinition.GetAssociatedMember();

            if (methodDefinition.IsStatic)
            {
                statements.Add(
                    JST.ExpressionStatement.CreateAssignmentExpression(
                        new JST.IdentifierExpression(typeConverter.ResolveImplementedVersion(propertyDefinition), Scope),
                        new JST.IdentifierExpression(ResolveArgument(methodDefinition.Parameters[0].Name), Scope)));

                statements.Add(
                    JST.ExpressionStatement.CreateAssignmentExpression(
                        new JST.IdentifierExpression(typeConverter.ResolveStaticMember(propertyDefinition), Scope),
                        MethodConverter.GenerateExtrationExpression(
                            methodDefinition.Parameters[0].ParameterType,
                            new JST.IdentifierExpression(ResolveArgument(methodDefinition.Parameters[0].Name), Scope),
                            this,
                            Scope)));
            }
            else
            {
                InitializeImportedWrapper(statements);

                statements.Add(
                    JST.ExpressionStatement.CreateAssignmentExpression(
                        new JST.IdentifierExpression(GetImportedWrapperIdentifier(propertyDefinition), Scope),
                        new JST.IdentifierExpression(ResolveArgument(methodDefinition.Parameters[0].Name), Scope)));

                statements.Add(
                    JST.ExpressionStatement.CreateAssignmentExpression(
                        new JST.IndexExpression(
                            null,
                            Scope,
                            ResolveThis(Scope, null),
                            new JST.IdentifierExpression(typeConverter.Resolve(propertyDefinition), Scope),
                            false),
                        MethodConverter.GenerateExtrationExpression(
                            methodDefinition.Parameters[0].ParameterType,
                            new JST.IdentifierExpression(ResolveArgument(methodDefinition.Parameters[0].Name), Scope),
                            this,
                            Scope)));
            }
        }

        /// <summary>
        /// Generates a method wrapper.
        /// </summary>
        /// <param name="statements"> The statements. </param>
        private void GenerateMethodWrapper(List<Statement> statements)
        {
            var arguments = new List<JST.Expression>();
            for (var iParameter = 0; iParameter < methodDefinition.Parameters.Count; iParameter++)
            {
                var parameterType = methodDefinition.Parameters[iParameter].ParameterType;
                var parameterExpression =
                        new IdentifierExpression(
                            ResolveArgument(methodDefinition.Parameters[iParameter].Name),
                            Scope);
                if (context.IsWrappedType(parameterType))
                {
                    arguments.Add(
                        MethodConverter.GenerateExtrationExpression(
                            parameterType,
                            parameterExpression,
                            this,
                            Scope));
                }
                else
                {
                    arguments.Add(parameterExpression);
                }
            }

            JST.MethodCallExpression methodCallExpression = null;
            JST.Expression methodExpression = null;
            if (methodDefinition.IsStatic)
            {
                methodExpression = new JST.IdentifierExpression(typeConverter.ResolveWrappedMethod(methodDefinition), Scope);
            }
            else
            {
                methodExpression =
                    new IndexExpression(
                        null,
                        Scope,
                        ResolveThis(Scope, null),
                        new JST.IdentifierExpression(
                            typeConverter.ResolveWrappedMethod(methodDefinition),
                            Scope));
            }

            methodCallExpression = new MethodCallExpression(
                null,
                Scope,
                methodExpression,
                arguments);

            if (!methodDefinition.ReturnType.IsSameDefinition(ClrKnownReferences.Void))
            {
                var returnType = methodDefinition.ReturnType;
                if (context.IsWrappedType(returnType))
                {
                    methodCallExpression = MethodConverter.GenerateWrapperExpression(
                        returnType,
                        methodCallExpression,
                        this,
                        Scope);
                }

                statements.Add(
                    new JST.ReturnStatement(
                        null,
                        Scope,
                        methodCallExpression));
            }
            else
            {
                statements.Add(new ExpressionStatement(null, Scope, methodCallExpression));
            }
        }

        /// <summary>
        /// Gets an imported wrapper identifier.
        /// </summary>
        /// <param name="propertyDefinition"> The property definition. </param>
        /// <returns>
        /// The imported wrapper identifier.
        /// </returns>
        private IIdentifier GetImportedWrapperIdentifier(PropertyDefinition propertyDefinition) => new CompoundIdentifier(
                thisIdentifier,
                typeConverter.ResolveImplementedVersion(propertyDefinition));

        /// <summary>
        /// Initializes the imported wrapper.
        /// </summary>
        /// <param name="functionExpression"> The function expression. </param>
        private void InitializeImportedWrapper(List<Statement> statements)
        {
            if (!methodDefinition.HasThis
                || initializeWrapperDone)
            { return; }

            var importedAdapterField = KnownReferences.ImportedExtensionField;
            statements.Add(
                JST.ExpressionStatement.CreateAssignmentExpression(
                    new JST.IndexExpression(
                        null,
                        Scope,
                        ResolveThis(Scope, null),
                        new JST.IdentifierExpression(Resolve(importedAdapterField), Scope),
                        false),
                    new JST.BinaryExpression(
                        null,
                        Scope,
                        BinaryOperator.LogicalOr,
                        new JST.IndexExpression(
                            null,
                            Scope,
                            ResolveThis(Scope, null),
                            new JST.IdentifierExpression(Resolve(importedAdapterField), Scope),
                            false),
                        new JST.MethodCallExpression(
                            null,
                            Scope,
                            JST.IdentifierExpression.Create(
                                null,
                                Scope,
                                ResolveStaticMember(KnownReferences.GetNewImportedExtensionMethod))))));
            initializeWrapperDone = true;
        }

        /// <summary>
        /// Gets the method slot parent expression.
        /// </summary>
        /// <returns></returns>
        private JST.Expression GetMethodSlotParentExpression()
        {
            if (context.IsPsudoType(typeConverter.TypeDefinition))
            {
                if (methodDefinition.IsStatic)
                {
                    throw new InvalidProgramException("Static Compiler generated properties not supported for imported or JsonType");
                }
                else
                {
                    return new JST.IdentifierExpression(
                        new CompoundIdentifier(
                            thisIdentifier,
                            Resolve(KnownReferences.ImportedExtensionField)),
                            Scope);
                }
            }

            if (methodDefinition.IsStatic)
            {
                return JST.IdentifierExpression.Create(
                        null,
                        Scope,
                        Resolve(typeConverter.TypeDefinition));
            }
            else
            {
                return ResolveThis(Scope, null);
            }
        }

        /// <summary>
        /// Gets the argument names.
        /// </summary>
        /// <returns></returns>
        private List<string> GetArgumentNames() => null;

        /// <summary>
        /// Gets the root block.
        /// </summary>
        /// <returns></returns>
        private ParameterBlock GetRootBlock()
        {
            if (RuntimeManager.Context.TryGetMethodAst(methodDefinition, out var rv, out var kind))
            {
                _kind = kind;
                usingMcs = true;
                return rv;
            }

            return null;
        }

        /// <summary>
        /// Query if this object has wrapped field.
        /// </summary>
        /// <returns>
        /// true if wrapped field, false if not.
        /// </returns>
        private bool HasWrappedField() => context.IsPsudoType(typeConverter.TypeDefinition)
                && typeConverter.TypeDefinition.Fields.Count(_ => !_.IsStatic && !_.HasConstant) > 0;

        private IIdentifier ResolveLocalInternal(string localVariable, bool isLocalFunction)
        {
            var scopeNode = scopeBlocksStack.First;
            var parameterNode = parameterBlocksStack.First;
            var identifierScopeNode = scopeStack.First;
            var localsNode = localVariableToIdentifierMapStack.First;
            var variableNamesGivenNode = variableNamesGivenStack.First;

            Func<IIdentifier> localVariableGetter =
                delegate
                {
                    if (!localsNode.Value.TryGetValue(localVariable, out var rv))
                    {
                        string identifierName = null;

                        var match = ConverterContext.GeneratedFieldNameRegex.Match(localVariable);
                        if (match.Success)
                        {
                            identifierName = match.Groups[ConverterContext.RealNameStr].Value;
                        }
                        else if (ConverterContext.GeneratedLocalVarRegex.IsMatch(localVariable))
                        {
                            identifierName = "tmp_";
                        }
                        else
                        {
                            identifierName = localVariable;
                        }

                        variableNamesGivenNode.Value.TryGetValue(identifierName, out var namesGiven);
                        variableNamesGivenNode.Value[identifierName] = namesGiven + 1;

                        if (namesGiven > 0)
                        {
                            identifierName = string.Format("{0}{1}", identifierName, namesGiven);
                        }

                        rv = SimpleIdentifier.CreateScopeIdentifier(
                            identifierScopeNode.Value,
                            identifierName,
                            false);

                        localsNode.Value.Add(localVariable, rv);
                    }

                    return rv;
                };

            if (!usingMcs)
            {
                return localVariableGetter();
            }

            while (scopeNode != null)
            {
                if (isLocalFunction)
                {
                    foreach (var localVar in scopeNode.Value.LocalFunctions)
                    {
                        if (localVar.Name == localVariable)
                        {
                            return localVariableGetter();
                        }
                    }
                }
                else
                {
                    foreach (var localVar in scopeNode.Value.LocalVariables)
                    {
                        if (localVar.Name == localVariable)
                        {
                            return localVariableGetter();
                        }
                    }
                }

                if (scopeNode.Value == parameterNode.Value)
                {
                    parameterNode = parameterNode.Next;
                    localsNode = localsNode.Next;
                    identifierScopeNode = identifierScopeNode.Next;
                    variableNamesGivenNode = variableNamesGivenNode.Next;
                }

                scopeNode = scopeNode.Next;
            }

            return null;
        }
    }
}