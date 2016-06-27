//-----------------------------------------------------------------------
// <copyright file="MethodConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NScript.CLR;
using NScript.CLR.AST;
using NScript.CLR.Decompiler;
using NScript.Converter.StatementsConverter;
using NScript.JSParser;
using NScript.JST;
using NScript.Utils;
using Mono.Cecil;
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
            hasGenericArguments = this.context.HasGenericArguments(methodDefinition);

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

            if (this.HasStaticImplementation
                && this.methodDefinition.HasThis
                && !this.IsConstructor)
            {
                argumentNames.Add(ThisArgument);
            }

            int argumentsStartIndex = argumentNames.Count;
            argumentNames.AddRange(methodDefinition.Parameters.Select(a => a.Name));

            methodScope = new IdentifierScope(
                typeConverter.Scope,
                argumentNames,
                false);

            scopeStack.AddFirst(methodScope);

            if (hasGenericArguments)
            {
                for (int genericArgumentIndex = 0;
                     genericArgumentIndex < this.methodDefinition.GenericParameters.Count;
                     genericArgumentIndex++)
                {
                    localTypeReferences.Add(
                        this.methodDefinition.GenericParameters[genericArgumentIndex],
                        methodScope.ParameterIdentifiers[genericArgumentIndex]);
                }
            }

            argumentVariableToIdentifierMapStack.AddFirst(new Dictionary<string, IIdentifier>());
            for (int argumentIndex = 0;
                 argumentIndex < methodDefinition.Parameters.Count;
                 argumentIndex++)
            {
                argumentVariableToIdentifierMapStack.First.Value.Add(
                    methodDefinition.Parameters[argumentIndex].Name,
                    methodScope.ParameterIdentifiers[argumentsStartIndex + argumentIndex]);
            }

            if (this.HasStaticImplementation
                && this.methodDefinition.HasThis)
            {
                if (this.methodDefinition.GenericParameters.Count != argumentsStartIndex)
                {
                    thisIdentifier = methodScope.ParameterIdentifiers[
                        this.methodDefinition.GenericParameters.Count];
                }
                else if (this.methodDefinition.CustomAttributes.SelectAttribute(
                        this.KnownReferences.IgnoreGenericArgumentsAttribute) != null)
                {
                    this.thisIdentifier = methodScope.ParameterIdentifiers[0];
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

        /// <summary>
        /// Gets the scope.
        /// </summary>
        /// <value>The scope.</value>
        public IdentifierScope Scope
        {
            get { return scopeStack.First.Value; }
        }

        /// <summary>
        /// Gets the runtime manager.
        /// </summary>
        /// <value>The runtime manager.</value>
        public RuntimeScopeManager RuntimeManager
        {
            get { return typeConverter.RuntimeManager; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is constructor.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is constructor; otherwise, <c>false</c>.
        /// </value>
        public bool IsConstructor
        {
            get { return methodDefinition.Name == ".ctor"; }
        }

        /// <summary>
        /// Gets the CLR known references.
        /// </summary>
        public ClrKnownReferences ClrKnownReferences
        {
            get { return clrKnownReferences; }
        }

        /// <summary>
        /// Gets the known references.
        /// </summary>
        public ConverterKnownReferences KnownReferences
        {
            get { return cnvtKnownReferences; }
        }

        /// <summary>
        /// Gets the method definition.
        /// </summary>
        public MethodDefinition MethodDefinition
        {
            get { return methodDefinition; }
        }

        /// <summary>
        /// Gets the method function expression.
        /// </summary>
        public FunctionExpression MethodFunctionExpression
        {
            get { return methodFunctionExpression; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has static implementation.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has static implementation; otherwise, <c>false</c>.
        /// </value>
        public bool HasStaticImplementation
        {
            get
            {
                return
                    (this.methodDefinition.HasThis
                        && (this.context.IsExtended(this.typeConverter.TypeDefinition)
                            || this.context.IsPsudoType(this.typeConverter.TypeDefinition)
                            || this.typeConverter.AllStaticMethods
                            || this.RuntimeManager.ImplementInstanceAsStatic)
                        && this.methodDefinition.CustomAttributes.SelectAttribute(
                                this.KnownReferences.KeepInstanceUsageAttribute) == null)
                    || !this.methodDefinition.HasThis;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is global static implementation.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is global static implementation; otherwise, <c>false</c>.
        /// </value>
        public bool IsGlobalStaticImplementation
        {
            get
            {
                return this.HasStaticImplementation
                    && !this.typeConverter.IsGenericLike;
            }
        }

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
                GenericInstanceType genericInstanceType = expressionType as GenericInstanceType;
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
                GenericInstanceType genericInstanceType = expressionType as GenericInstanceType;
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
                PropertyDefinition propertyDefinition = methodDefinition.GetPropertyDefinition();
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
        public IIdentifier ResolveLocal(string localVariable)
        {
            LinkedListNode<ScopeBlock> scopeNode = scopeBlocksStack.First;
            LinkedListNode<ParameterBlock> parameterNode = parameterBlocksStack.First;
            LinkedListNode<IdentifierScope> identifierScopeNode = scopeStack.First;
            LinkedListNode<Dictionary<string, IIdentifier>> localsNode = localVariableToIdentifierMapStack.First;
            LinkedListNode<Dictionary<string, int>> variableNamesGivenNode = variableNamesGivenStack.First;

            Func<IIdentifier> localVariableGetter =
                delegate
                {
                    IIdentifier rv;
                    if (!localsNode.Value.TryGetValue(localVariable, out rv))
                    {
                        string identifierName = null;

                        Match match = ConverterContext.GeneratedFieldNameRegex.Match(localVariable);
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

                        int namesGiven = 0;
                        variableNamesGivenNode.Value.TryGetValue(identifierName, out namesGiven);
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
                foreach (LocalVariable localVar in scopeNode.Value.LocalVariables)
                {
                    if (localVar.Name == localVariable)
                    {
                        return localVariableGetter();
                    }
                }

                if (scopeNode.Value
                    == parameterNode.Value)
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

        /// <summary>
        /// Resolves the argument.
        /// </summary>
        /// <param name="argumentIndex">Index of the argument.</param>
        /// <returns></returns>
        public IIdentifier ResolveArgument(string argumentName)
        {
            IIdentifier rv;
            if (!TryResolveArgument(argumentName, out rv))
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
            GenericParameterType? typeScope = typeReference.GetGenericTypeScope();

            if (typeScope.HasValue
                && typeScope.Value == GenericParameterType.Method)
            {
                if (!hasGenericArguments)
                {
                    throw new ApplicationException(
                        string.Format(
                            "Can't access generic type ({0}) if they are ignored",
                            typeReference));
                }

                IIdentifier rv;
                if (!localTypeReferences.TryGetValue(typeReference, out rv))
                {
                    var strBuilder = new StringBuilder();

                    RuntimeScopeManager.CalculateFriendlyTypeReferenceName(
                        typeReference,
                        strBuilder,
                        (typeDefinitionBase, typeNameBuilder) =>
                        {
                            string typeName = RuntimeScopeManager.GetSplitName(
                                typeDefinitionBase.Name).Item2;

                            int apposIndex = typeName.LastIndexOf('`');
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
        public IList<IIdentifier> ResolveStaticMember(FieldReference member)
        {
            return typeConverter.ResolveStaticMember(
                member,
                Resolve);
        }

        /// <summary>
        /// Resolves the static member.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>Resolve static member</returns>
        public IList<IIdentifier> ResolveStaticMember(MethodReference member)
        {
            return typeConverter.ResolveStaticMember(
                member,
                Resolve);
        }

        public IList<IIdentifier> ResolveFactory(MethodReference constructor)
        {
            return typeConverter.ResolveFactory(
                constructor,
                Resolve);
        }

        /// <summary>
        /// Resolves the specified member reference.
        /// </summary>
        /// <param name="fieldReference">The member reference.</param>
        /// <returns>Identifier identifying the member.</returns>
        public IIdentifier Resolve(FieldReference fieldReference)
        {
            return typeConverter.Resolve(fieldReference);
        }

        /// <summary>
        /// Resolves the specified member reference.
        /// </summary>
        /// <param name="propertyReference">The member reference.</param>
        /// <returns>Identifier identifying the member.</returns>
        public IIdentifier Resolve(PropertyReference propertyReference)
        {
            return typeConverter.Resolve(propertyReference);
        }

        /// <summary>
        /// Resolves the specified member reference.
        /// </summary>
        /// <param name="memberReference">The member reference.</param>
        /// <returns>Identifier identifying the member.</returns>
        public IIdentifier Resolve(MethodReference memberReference)
        {
            return typeConverter.Resolve(memberReference);
        }

        /// <summary>
        /// Resolves the virtual.
        /// </summary>
        /// <param name="methodReference">The method reference.</param>
        /// <param name="scope">The scope.</param>
        /// <returns>Expression that will resolve in slot for the method.</returns>
        public JST.Expression ResolveVirtualMethod(
            MethodReference methodReference,
            IdentifierScope scope)
        {
            return typeConverter.ResolveVirtualMethod(
                methodReference,
                scope,
                Resolve);
        }

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
                JST.Expression rv = ResolveVirtualMethod(
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
            if (methodReference.Resolve().DeclaringType.IsValueType)
            {
                // Currently only struct method calls are converted to static method calls.
                // In future calls to all methods may become static call.
                JST.Expression methodIdentifier = IdentifierExpression.Create(
                    location,
                    scope,
                    ResolveStaticMember(methodReference));

                var newArguments = new List<JST.Expression>();
                newArguments.Add(instance);
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

        /// <summary>
        /// Releases the temp variable.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public void ReleaseTempVariable(IIdentifier identifier)
        {
            freeTempVariables.Enqueue(identifier);
        }

        /// <summary>
        /// Gets the replacement expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>Canned expression if replacer exists.</returns>
        public JST.Expression GetReplacementExpression(Expression expression)
        {
            for (int replacerScopeIndex = 0; replacerScopeIndex < replacers.Count; replacerScopeIndex++)
            {
                for (int replacerTupleIndex = 0;
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
            params Tuple<Expression, Func<IdentifierScope, JST.Expression>>[] expressionMappers)
        {
            replacers.Add(expressionMappers);
        }

        /// <summary>
        /// Pops the replacer.
        /// </summary>
        public void PopReplacer()
        {
            replacers.RemoveAt(replacers.Count - 1);
        }

        /// <summary>
        /// Pushes the parameter block.
        /// </summary>
        /// <param name="parameterBlock">The parameter block.</param>
        /// <returns></returns>
        public JST.Expression ProcessParameterBlock(ParameterBlock parameterBlock)
        {
            scopeBlocksStack.AddFirst(parameterBlock);
            parameterBlocksStack.AddFirst(parameterBlock);
            variableNamesGivenStack.AddFirst(new Dictionary<string, int>());
            localVariableToIdentifierMapStack.AddFirst(new Dictionary<string, IIdentifier>());
            argumentVariableToIdentifierMapStack.AddFirst(new Dictionary<string, IIdentifier>());

            try
            {
                var parameterNames = new List<string>();
                foreach (ParameterVariable param in parameterBlock.Parameters)
                {
                    parameterNames.Add(param.Name);
                }

                var identifierScope = new IdentifierScope(
                    Scope,
                    parameterNames,
                    false);

                for (int iParam = 0; iParam < parameterNames.Count; iParam++)
                {
                    argumentVariableToIdentifierMapStack.First.Value.Add(
                        parameterNames[iParam],
                        identifierScope.ParameterIdentifiers[iParam]);
                }

                scopeStack.AddFirst(identifierScope);
                var statements = new List<Statement>();
                try
                {
                    foreach (CLR.AST.Statement statement in parameterBlock.Statements)
                    {
                        Statement jsStatement =
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

                IIdentifier delegateFunctionNameId = SimpleIdentifier.CreateScopeIdentifier(
                    RuntimeManager.Scope,
                    string.Format(
                        "{0}_del{1}",
                        GetMethodName(methodDefinition).SuggestedName,
                        delegateCount++ == 0
                            ? string.Empty
                            : delegateCount.ToString()),
                    false);

                var rv = new FunctionExpression(
                    parameterBlock.Location,
                    Scope,
                    identifierScope,
                    identifierScope.ParameterIdentifiers,
                    delegateFunctionNameId);

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
            }
        }

        /// <summary>
        /// Pushes the scope block.
        /// </summary>
        /// <param name="scopeBlock">The scope block.</param>
        public void PushScopeBlock(ScopeBlock scopeBlock)
        {
            scopeBlocksStack.AddFirst(scopeBlock);
        }

        /// <summary>
        /// Pops the scope block.
        /// </summary>
        public void PopScopeBlock()
        {
            scopeBlocksStack.RemoveFirst();
        }

        /// <summary>
        /// Fixes the this variable.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <returns>fixes this keyword inside of script to this_ and this_ variables to other names.</returns>
        private static string FixThisVariable(string script)
        {
            StringBuilder strBuilder = new StringBuilder();
            int lastIndex = 0, nowIndex = 0;
            bool inStr = false;
            char strChar = (char)0;
            for (; nowIndex < script.Length; nowIndex++)
            {
                char ch = script[nowIndex];
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
                            int endComment = script.IndexOf("*/", nowIndex);
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

            bool matchFailed = false;
            for (int startIndex = startAt; startIndex < endAt && startIndex < fromString.Length; startIndex++)
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
        private IIdentifier GetMethodName(MethodReference methodReference)
        {
            return typeConverter.RuntimeManager.ResolveFunctionName(methodReference);
        }

        /// <summary>
        /// Gets the constructor default initialization statement.
        /// </summary>
        /// <returns></returns>
        private Statement GetConstructorDefaultInitializationStatement()
        {
            return new ExpressionStatement(
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
        }

        /// <summary>
        /// Imports the js script.
        /// </summary>
        /// <returns>Function Expression after parsing js script.</returns>
        private List<Statement> ImportJsScript()
        {
            CustomAttribute scriptAttribute = methodDefinition.CustomAttributes.SelectAttribute(
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
                string script = (string)scriptAttribute.ConstructorArguments[0].Value;

                if (this.thisIdentifier != null)
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

            List<Statement> returnValue = new List<Statement>();
            if (IsConstructor
                && thisIdentifier != null
                && Scope.UsedLocalIdentifiers.Contains(thisIdentifier))
            {
                // We are in struct constructor. So we need to initialize this.
                returnValue.Add(GetConstructorDefaultInitializationStatement());
            }

            if (this.HasWrappedField())
            {
                this.InitializeImportedWrapper(returnValue);
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
            List<Statement> statements = this.InnerConvert();
            FunctionExpression functionExpression = this.GetFunctionExpressionShell();

            foreach (var plugin in this.context.MethodConverterPlugins)
            {
                switch (plugin.GetInterestLevel(this.methodDefinition, this.context))
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

            functionExpression.AddStatements(statements);
            return functionExpression;
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
            ParameterBlock rootBlock = GetRootBlock();

            if (this.HasWrappedField())
            {
                this.InitializeImportedWrapper(statements);
            }

            // This means that we have compiler implemented method for Property or Event.
            if (rootBlock == null)
            {
                this.GenerateCompilerImplemented(statements);
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
                foreach (ParameterVariable param in rootBlock.Parameters)
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

            foreach (CLR.AST.Statement statement in rootBlock.Statements)
            {
                Statement jsStatement =
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
                && this.typeConverter.AllStaticMethods
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
            if (this.context.IsWrapped(this.methodDefinition))
            {
                return this.GenerateWrapperImplementation();
            }

            if (!methodDefinition.HasBody
                || methodDefinition.Body.Instructions.Count == 0)
            {
                return ImportJsScript();
            }

            return this.ConvertCST();
        }

        /// <summary>
        /// Gets function expression shell.
        /// </summary>
        /// <returns>
        /// The function expression shell.
        /// </returns>
        private FunctionExpression GetFunctionExpressionShell()
        {
            IIdentifier functionName = this.GetMethodName(methodDefinition);

            if (this.IsGlobalStaticImplementation)
            {
                if (this.IsConstructor && this.HasStaticImplementation)
                {
                    functionName = this.RuntimeManager.ResolveFactory(this.methodDefinition);
                }
                else
                {
                    functionName = this.RuntimeManager.ResolveStatic(this.methodDefinition);
                }
            }

            var returnValue = new FunctionExpression(
                null,
                typeConverter.Scope,
                Scope,
                Scope.ParameterIdentifiers,
                functionName);

            return returnValue;
        }

        /// <summary>
        /// Generates the compiler implemented.
        /// </summary>
        /// <exception cref="InvalidProgramException"> Thrown when an Invalid Program error condition
        ///     occurs. </exception>
        /// <param name="returnValue"> The return value. </param>
        private void GenerateCompilerImplemented(List<Statement> returnValue )
        {
            if (this.HasWrappedField())
            {
                this.InitializeImportedWrapper(returnValue);
            }

            if (this.methodDefinition.IsSetter)
            {
                this.GenerateSetterImplementation(returnValue);
            }
            else if (this.methodDefinition.IsGetter)
            {
                this.GenerateGetterImplementation(returnValue);
            }
            else if (this.methodDefinition.IsAddOn
                || this.methodDefinition.IsRemoveOn)
            {
                this.GenerateAddonOrRemoveOnImplementation(returnValue);
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
            IIdentifier functionName = this.GetMethodName(methodDefinition);

            List<Statement> returnValue = new List<Statement>();

            if (this.methodDefinition.IsSetter
                && !this.context.IsRenamed(this.methodDefinition))
            {
                this.GenerateSetterImportedWrapper(returnValue);
            }
            else if (this.methodDefinition.IsGetter
                && !this.context.IsRenamed(this.methodDefinition))
            {
                this.GenerateGetterImportedWrapper(returnValue);
            }
            else if (this.methodDefinition.IsAddOn
                || this.methodDefinition.IsRemoveOn)
            {
                this.GenerateAddonOrRemoveOnImportedWrapper(returnValue);
            }
            else
            {
                this.GenerateMethodWrapper(returnValue);
            }

            return returnValue;
        }

        /// <summary>
        /// Generates the addon or remove on implementation.
        /// </summary>
        /// <param name="functionExpression">The function expression.</param>
        private void GenerateAddonOrRemoveOnImplementation(List<Statement> statements)
        {
            int matchOffset = this.methodDefinition.IsAddOn
                ? "add_".Length
                : "remove_".Length;

            string fieldName = this.methodDefinition.Name.Substring(matchOffset);
            FieldReference fieldReference = this.typeConverter.TypeDefinition.Fields.First((fld) => fld.Name == fieldName);

            MethodReference addOrRemoveRef;
            if (this.methodDefinition.IsAddOn)
            {
                addOrRemoveRef = this.context.KnownReferences.DelegateCombineMethod;
            }
            else
            {
                addOrRemoveRef = this.context.KnownReferences.DelegateRemoveMethod;
            }

            var methodCallExpr = new JST.MethodCallExpression(
                null,
                this.Scope,
                JST.IdentifierExpression.Create(
                    null,
                    this.Scope,
                    this.ResolveStaticMember(addOrRemoveRef)),
                fieldReference.Resolve().IsStatic
                    ? JST.IdentifierExpression.Create(
                            null,
                            this.Scope,
                            this.ResolveStaticMember(fieldReference))
                    : new JST.IndexExpression(
                        null,
                        this.Scope,
                        this.GetMethodSlotParentExpression(),
                        new JST.IdentifierExpression(this.Resolve(fieldReference), this.Scope)),
                new JST.IdentifierExpression(this.Scope.ParameterIdentifiers[0], this.Scope));

            statements.Add(
                JST.ExpressionStatement.CreateAssignmentExpression(
                    fieldReference.Resolve().IsStatic
                        ? JST.IdentifierExpression.Create(
                                null,
                                this.Scope,
                                this.ResolveStaticMember(fieldReference))
                        : new JST.IndexExpression(
                            null,
                            this.Scope,
                            this.GetMethodSlotParentExpression(),
                            new JST.IdentifierExpression(this.Resolve(fieldReference), this.Scope)),
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
                this.Scope,
                JST.IdentifierExpression.Create(
                    null,
                    this.Scope,
                    this.ResolveStaticMember(
                        this.MethodDefinition.IsAddOn
                            ?  this.context.KnownReferences.AddEventMethod
                            : this.context.KnownReferences.RemoveEventMethod)),
                new JST.IdentifierExpression(this.thisIdentifier, this.Scope),
                new JST.StringLiteralExpression(this.Scope, this.GetNativeEventName()),
                new JST.IdentifierExpression(this.ResolveArgument(this.methodDefinition.Parameters[0].Name), this.Scope),
                new JST.BooleanLiteralExpression(this.Scope, false));

            statements.Add(new ExpressionStatement(null, this.Scope, methodCallExpr));
        }

        /// <summary>
        /// Gets the native event name.
        /// </summary>
        /// <returns>
        /// The native event name.
        /// </returns>
        private string GetNativeEventName()
        {
            EventDefinition eventDefinition = (EventDefinition)this.methodDefinition.GetAssociatedMember();
            string eventName = eventDefinition.Name;

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
        private void GenerateSetterImplementation(List<Statement> statements)
        {
            statements.Add(
                new JST.ReturnStatement(
                    null,
                    this.Scope,
                    new JST.BinaryExpression(
                        null,
                        this.Scope,
                        BinaryOperator.Assignment,
                       new JST.IndexExpression(
                           null,
                           this.Scope,
                           this.GetMethodSlotParentExpression(),
                           new JST.IdentifierExpression(
                               this.RuntimeManager.Resolve(
                                   this.methodDefinition.GetPropertyDefinition()), this.Scope)),
                       new JST.IdentifierExpression(
                           this.ResolveArgument(methodDefinition.Parameters[0].Name), this.Scope))));
        }

        /// <summary>
        /// Generates the getter implementation.
        /// </summary>
        /// <param name="statements"> The statements. </param>
        private void GenerateGetterImplementation(List<Statement> statements)
        {
            statements.Add(
                new JST.ReturnStatement(
                    null,
                    this.Scope,
                    new JST.IndexExpression(
                        null,
                        this.Scope,
                        this.GetMethodSlotParentExpression(),
                        new JST.IdentifierExpression(
                            this.RuntimeManager.Resolve(
                                this.methodDefinition.GetPropertyDefinition()), this.Scope))));
        }

        /// <summary>
        /// Generates a getter imported wrapper.
        /// </summary>
        /// <exception cref="InvalidProgramException"> Thrown when an invalid program error condition
        ///     occurs. </exception>
        /// <param name="functionExpression"> The function expression. </param>
        public void GenerateGetterImportedWrapper(List<Statement> statements)
        {
            PropertyDefinition propertyDefinition = (PropertyDefinition)this.methodDefinition.GetAssociatedMember();
            var valueParameter = this.methodDefinition.ReturnType;

            if (this.methodDefinition.IsStatic)
            {
                JST.Expression valueExpression = new JST.ConditionalOperatorExpression(
                    null,
                    this.Scope,
                    new JST.IdentifierExpression(this.typeConverter.ResolveStaticMember(propertyDefinition), this.Scope),
                    MethodConverter.GenerateWrapperExpression(
                        valueParameter,
                        new JST.IdentifierExpression(this.typeConverter.ResolveStaticMember(propertyDefinition), this.Scope),
                        this,
                        this.Scope),
                    new JST.NullLiteralExpression(this.Scope));

                statements.Add(
                    new JST.ReturnStatement(
                        null,
                        this.Scope,
                        new JST.BinaryExpression(
                            null,
                            this.Scope,
                            BinaryOperator.Assignment,
                            new JST.IdentifierExpression(this.typeConverter.ResolveStaticMember(propertyDefinition), this.Scope),
                            new JST.BinaryExpression(
                                null,
                                this.Scope,
                                BinaryOperator.LogicalOr,
                                new JST.IdentifierExpression(this.typeConverter.ResolveStaticMember(propertyDefinition), this.Scope),
                                valueExpression))));
            }
            else
            {
                this.InitializeImportedWrapper(statements);

                JST.Expression valueExpression = new JST.ConditionalOperatorExpression(
                    null,
                    this.Scope,
                    new JST.IndexExpression(
                        null,
                        this.Scope,
                        this.ResolveThis(this.Scope, null),
                        new JST.IdentifierExpression(this.typeConverter.Resolve(propertyDefinition), this.Scope),
                        false),
                    MethodConverter.GenerateWrapperExpression(
                        valueParameter,
                        new JST.IndexExpression(
                            null,
                            this.Scope,
                            this.ResolveThis(this.Scope, null),
                            new JST.IdentifierExpression(this.typeConverter.Resolve(propertyDefinition), this.Scope),
                            false),
                        this,
                        this.Scope),
                    new JST.NullLiteralExpression(this.Scope));

                statements.Add(
                    new JST.ReturnStatement(
                        null,
                        this.Scope,
                        new JST.BinaryExpression(
                            null,
                            this.Scope,
                            BinaryOperator.Assignment,
                            new JST.IdentifierExpression(this.GetImportedWrapperIdentifier(propertyDefinition), this.Scope),
                            new JST.BinaryExpression(
                                null,
                                this.Scope,
                                BinaryOperator.LogicalOr,
                                new JST.IdentifierExpression(this.GetImportedWrapperIdentifier(propertyDefinition), this.Scope),
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
            PropertyDefinition propertyDefinition = (PropertyDefinition)this.methodDefinition.GetAssociatedMember();

            if (this.methodDefinition.IsStatic)
            {
                statements.Add(
                    JST.ExpressionStatement.CreateAssignmentExpression(
                        new JST.IdentifierExpression(this.typeConverter.ResolveImplementedVersion(propertyDefinition), this.Scope),
                        new JST.IdentifierExpression(this.ResolveArgument(this.methodDefinition.Parameters[0].Name), this.Scope)));

                statements.Add(
                    JST.ExpressionStatement.CreateAssignmentExpression(
                        new JST.IdentifierExpression(this.typeConverter.ResolveStaticMember(propertyDefinition), this.Scope),
                        MethodConverter.GenerateExtrationExpression(
                            this.methodDefinition.Parameters[0].ParameterType,
                            new JST.IdentifierExpression(this.ResolveArgument(this.methodDefinition.Parameters[0].Name), this.Scope),
                            this,
                            this.Scope)));
            }
            else
            {
                this.InitializeImportedWrapper(statements);

                statements.Add(
                    JST.ExpressionStatement.CreateAssignmentExpression(
                        new JST.IdentifierExpression(this.GetImportedWrapperIdentifier(propertyDefinition), this.Scope),
                        new JST.IdentifierExpression(this.ResolveArgument(this.methodDefinition.Parameters[0].Name), this.Scope)));

                statements.Add(
                    JST.ExpressionStatement.CreateAssignmentExpression(
                        new JST.IndexExpression(
                            null,
                            this.Scope,
                            this.ResolveThis(this.Scope, null),
                            new JST.IdentifierExpression(this.typeConverter.Resolve(propertyDefinition), this.Scope),
                            false),
                        MethodConverter.GenerateExtrationExpression(
                            this.methodDefinition.Parameters[0].ParameterType,
                            new JST.IdentifierExpression(this.ResolveArgument(this.methodDefinition.Parameters[0].Name), this.Scope),
                            this,
                            this.Scope)));
            }
        }

        /// <summary>
        /// Generates a method wrapper.
        /// </summary>
        /// <param name="statements"> The statements. </param>
        private void GenerateMethodWrapper(List<Statement> statements)
        {
            List<JST.Expression> arguments = new List<JST.Expression>();
            for (int iParameter = 0; iParameter < this.methodDefinition.Parameters.Count; iParameter++)
            {
                var parameterType = this.methodDefinition.Parameters[iParameter].ParameterType;
                var parameterExpression = 
                        new IdentifierExpression(
                            this.ResolveArgument(this.methodDefinition.Parameters[iParameter].Name),
                            this.Scope);
                if (this.context.IsWrappedType(parameterType))
                {
                    arguments.Add(
                        MethodConverter.GenerateExtrationExpression(
                            parameterType,
                            parameterExpression,
                            this,
                            this.Scope));
                }
                else
                {
                    arguments.Add(parameterExpression);
                }
            }

            JST.MethodCallExpression methodCallExpression = null;
            JST.Expression methodExpression = null;
            if (this.methodDefinition.IsStatic)
            {
                methodExpression = new JST.IdentifierExpression(this.typeConverter.ResolveWrappedMethod(this.methodDefinition), this.Scope);
            }
            else
            {
                methodExpression =
                    new IndexExpression(
                        null,
                        this.Scope,
                        this.ResolveThis(this.Scope, null),
                        new JST.IdentifierExpression(
                            this.typeConverter.ResolveWrappedMethod(this.methodDefinition),
                            this.Scope));
            }

            methodCallExpression = new MethodCallExpression(
                null,
                this.Scope,
                methodExpression,
                arguments);

            if (!this.methodDefinition.ReturnType.IsSameDefinition(this.ClrKnownReferences.Void))
            {
                var returnType = this.methodDefinition.ReturnType;
                if (this.context.IsWrappedType(returnType))
                {
                    methodCallExpression = MethodConverter.GenerateWrapperExpression(
                        returnType,
                        methodCallExpression,
                        this,
                        this.Scope);
                }

                statements.Add(
                    new JST.ReturnStatement(
                        null,
                        this.Scope,
                        methodCallExpression));
            }
            else
            {
                statements.Add(new ExpressionStatement(null, this.Scope, methodCallExpression));
            }
        }

        /// <summary>
        /// Gets an imported wrapper identifier.
        /// </summary>
        /// <param name="propertyDefinition"> The property definition. </param>
        /// <returns>
        /// The imported wrapper identifier.
        /// </returns>
        private IIdentifier GetImportedWrapperIdentifier(PropertyDefinition propertyDefinition)
        {
            return new CompoundIdentifier(
                this.thisIdentifier,
                this.typeConverter.ResolveImplementedVersion(propertyDefinition));
        }

        /// <summary>
        /// Initializes the imported wrapper.
        /// </summary>
        /// <param name="functionExpression"> The function expression. </param>
        private void InitializeImportedWrapper(List<Statement> statements)
        {
            if (!this.methodDefinition.HasThis
                || this.initializeWrapperDone)
            { return; }

            FieldReference importedAdapterField = this.KnownReferences.ImportedExtensionField;
            statements.Add(
                JST.ExpressionStatement.CreateAssignmentExpression(
                    new JST.IndexExpression(
                        null,
                        this.Scope,
                        this.ResolveThis(this.Scope, null),
                        new JST.IdentifierExpression(this.Resolve(importedAdapterField), this.Scope),
                        false),
                    new JST.BinaryExpression(
                        null,
                        this.Scope,
                        BinaryOperator.LogicalOr,
                        new JST.IndexExpression(
                            null,
                            this.Scope,
                            this.ResolveThis(this.Scope, null),
                            new JST.IdentifierExpression(this.Resolve(importedAdapterField), this.Scope),
                            false),
                        new JST.MethodCallExpression(
                            null,
                            this.Scope,
                            JST.IdentifierExpression.Create(
                                null,
                                this.Scope,
                                this.ResolveStaticMember(this.KnownReferences.GetNewImportedExtensionMethod))))));
            this.initializeWrapperDone = true;
        }

        /// <summary>
        /// Gets the method slot parent expression.
        /// </summary>
        /// <returns></returns>
        private JST.Expression GetMethodSlotParentExpression()
        {
            if (this.context.IsPsudoType(this.typeConverter.TypeDefinition))
            {
                if (this.methodDefinition.IsStatic)
                {
                    throw new InvalidProgramException("Static Compiler generated properties not supported for imported or JsonType");
                }
                else
                {
                    return new JST.IdentifierExpression(
                        new CompoundIdentifier(this.thisIdentifier, 
                            this.Resolve(this.KnownReferences.ImportedExtensionField)),
                            this.Scope);
                }
            }

            if (this.methodDefinition.IsStatic)
            {
                return JST.IdentifierExpression.Create(
                        null,
                        this.Scope,
                        this.Resolve(this.typeConverter.TypeDefinition));
            }
            else
            {
                return this.ResolveThis(this.Scope, null);
            }
        }

        /// <summary>
        /// Gets the argument names.
        /// </summary>
        /// <returns></returns>
        private List<string> GetArgumentNames()
        {
            return null;
        }

        /// <summary>
        /// Gets the root block.
        /// </summary>
        /// <returns></returns>
        private ParameterBlock GetRootBlock()
        {
            ParameterBlock rv;
            if (RuntimeManager.Context.TryGetMethodAst(methodDefinition, out rv))
            {
                usingMcs = true;
                return rv;
            }

            var executionBlock = new MethodExecutionBlock(
                context.ClrContext,
                methodDefinition);

            TopLevelBlock csAst = executionBlock.ToAST();

            return csAst.RootBlock;
        }

        /// <summary>
        /// Query if this object has wrapped field.
        /// </summary>
        /// <returns>
        /// true if wrapped field, false if not.
        /// </returns>
        private bool HasWrappedField()
        {
            return this.context.IsPsudoType(this.typeConverter.TypeDefinition) && this.typeConverter.TypeDefinition.HasFields;
        }
    }
}