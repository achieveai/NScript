//-----------------------------------------------------------------------
// <copyright file="ForLoopConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.StatementsConverter
{
    using System.Collections.Generic;
    using NScript.CLR.AST;
    using NScript.Converter.ExpressionsConverter;
    using NScript.Converter.TypeSystemConverter;
    using Mono.Cecil;
    using NScript.CLR;

    /// <summary>
    /// Definition for ForLoopConverter
    /// </summary>
    public static class ForLoopConverter
    {
        /// <summary>
        /// Converts the specified converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="forLoop">For loop.</param>
        /// <returns>ForLoop</returns>
        public static JST.Statement Convert(
            IMethodScopeConverter converter,
            ForLoop forLoop)
        {
            converter.PushScopeBlock(forLoop);
            try
            {
                return new JST.ForLoop(
                    forLoop.Location,
                    converter.Scope,
                    ExpressionsConverter.ExpressionConverterBase.Convert(converter, forLoop.Condition),
                    StatementConverterBase.Convert(converter, forLoop.InitializeStatement),
                    StatementConverterBase.Convert(converter, forLoop.IncrementStatement),
                    StatementConverterBase.Convert(converter, forLoop.Loop));
            }
            finally
            {
                converter.PopScopeBlock();
            }
        }

        /// <summary>
        /// Converts the specified converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="forEachLoop">For each loop.</param>
        /// <returns>Converted foreach loop.</returns>
        public static JST.Statement Convert(
            IMethodScopeConverter converter,
            ForEachLoop forEachLoop)
        {
            converter.PushScopeBlock(forEachLoop);
            try
            {
                if (forEachLoop.Collection.ResultType.FullName == "System.Collections.Dictionary")
                {
                    return ForLoopConverter.ConvertDictionary(
                        converter,
                        forEachLoop);
                }

                JST.IdentifierExpression enumerator =
                    new JST.IdentifierExpression(
                        converter.GetTempVariable(),
                        converter.Scope);

                JST.Expression getEnumerator =
                    ForLoopConverter.GetEnumeratorExpression(
                        converter,
                        forEachLoop.Collection);

                JST.Expression condition =
                    ForLoopConverter.GetMoveNextExpression(
                    converter,
                    enumerator);

                return new JST.ForLoop(
                    forEachLoop.Location,
                    converter.Scope,
                    condition,
                    JST.ExpressionStatement.CreateAssignmentExpression(
                        enumerator,
                        getEnumerator),
                    null,
                    ForLoopConverter.GetScopeBlock(
                        converter,
                        enumerator,
                        forEachLoop.Variable,
                        forEachLoop.Scope));
            }
            finally
            {
                converter.PopScopeBlock();
            }
        }

        private static JST.Statement ConvertDictionary(
            IMethodScopeConverter converter,
            ForEachLoop forEachLoop)
        {
            TypeReference dictionaryEntryTypeReference = converter.KnownReferences.DictionaryEntry;
            PropertyReference keyReference = converter.KnownReferences.DictEntryKey;
            PropertyReference valueReference = converter.KnownReferences.DictEntryValue;

            JST.IIdentifier keyIdentifier = converter.GetTempVariable();
            JST.IIdentifier dictIdentifier = converter.GetTempVariable();

            List<JST.Statement> statements = new List<JST.Statement>();
            JST.ScopeBlock scopeBlock = new JST.ScopeBlock(
                forEachLoop.Scope.Location,
                converter.Scope,
                statements);

            JST.ForInLoop forInLoop = new JST.ForInLoop(
                forEachLoop.Location,
                converter.Scope,
                new JST.BinaryExpression(
                    forEachLoop.Collection.Location,
                    converter.Scope,
                    JST.BinaryOperator.Assignment,
                    new JST.IdentifierExpression(dictIdentifier, converter.Scope),
                    ExpressionConverterBase.Convert(
                        converter,
                        forEachLoop.Collection)),
                new JST.IdentifierExpression(keyIdentifier, converter.Scope),
                scopeBlock);

            // Let's add first statement that initializes dictionaryEntry
            JST.InlineObjectInitializer dictEntry = new JST.InlineObjectInitializer(
                null,
                converter.Scope);

            dictEntry.AddInitializer(
                converter.Resolve(keyReference),
                new JST.IdentifierExpression(keyIdentifier, converter.Scope));

            dictEntry.AddInitializer(
                converter.Resolve(valueReference),
                new JST.IndexExpression(
                    null,
                    converter.Scope,
                    new JST.IdentifierExpression(dictIdentifier, converter.Scope),
                    new JST.IdentifierExpression(keyIdentifier, converter.Scope),
                    true));

            JST.Expression initDictEntry = new JST.BinaryExpression(
                forEachLoop.Collection.Location,
                converter.Scope,
                JST.BinaryOperator.Assignment,
                new JST.IdentifierExpression(
                    converter.ResolveLocal(forEachLoop.Variable.Name),
                    converter.Scope),
                dictEntry);

            statements.Add(new JST.ExpressionStatement(null, converter.Scope, initDictEntry));

            for (int statementIndex = 0; statementIndex < forEachLoop.Scope.Statements.Count; statementIndex++)
            {
                JST.Statement statement = StatementConverterBase.Convert(
                    converter,
                    forEachLoop.Scope.Statements[statementIndex]);

                if (statement != null)
                {
                    statements.Add(statement);
                }
            }

            return forInLoop;
        }

        /// <summary>
        /// Gets the enumerator expression.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        private static JST.Expression GetEnumeratorExpression(
            IMethodScopeConverter converter,
            Expression collection)
        {
            JST.Expression collectionEnumeration = ExpressionConverterBase.Convert(
                converter,
                collection);

            MethodReference getEnumeratorFunc = converter.KnownReferences.GetEnumeratorIEnumerableMethod;

            return new JST.MethodCallExpression(
                collectionEnumeration.Location,
                converter.Scope,
                new JST.IndexExpression(
                    collectionEnumeration.Location,
                    converter.Scope,
                    collectionEnumeration,
                    converter.ResolveVirtualMethod(
                        getEnumeratorFunc,
                        converter.Scope)),
                new List<JST.Expression>());
        }

        /// <summary>
        /// Gets the move next expression.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="enumerator">The enumerator.</param>
        /// <returns></returns>
        private static JST.Expression GetMoveNextExpression(
            IMethodScopeConverter converter,
            JST.Expression enumerator)
        {
            MethodReference moveNextFunc = converter.KnownReferences.MoveNextEnumeratorMethod;

            return new JST.MethodCallExpression(
                    enumerator.Location,
                    converter.Scope,
                    new JST.IndexExpression(
                        enumerator.Location,
                        converter.Scope,
                        enumerator,
                        converter.ResolveVirtualMethod(
                            moveNextFunc,
                            converter.Scope)),
                    new List<JST.Expression>());
        }

        /// <summary>
        /// Gets the current expression.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="enumerator">The enumerator.</param>
        /// <returns></returns>
        private static JST.Expression GetCurrentExpression(
            IMethodScopeConverter converter,
            TypeReference expectedType,
            JST.Expression enumerator)
        {
            TypeDefinition type = converter.KnownReferences.IEnumerator.Resolve();
            MethodReference getCurrentFunc = converter.KnownReferences.GetCurrentIEnumeratorMethod;

            JST.Expression rv = new JST.MethodCallExpression(
                enumerator.Location,
                converter.Scope,
                new JST.IndexExpression(
                    enumerator.Location,
                    converter.Scope,
                    enumerator,
                    converter.ResolveVirtualMethod(
                        getCurrentFunc,
                        converter.Scope)),
                new List<JST.Expression>());

            // Here we can check if genericParameter is restricted to referenceTypes
            // but for now not needed.
            if (expectedType.IsValueOrEnum()
                || expectedType.IsGenericParameter)
            {
                rv = MethodCallExpressionConverter.CreateMethodCallExpression(
                    new MethodCallContext(
                        converter.KnownReferences.UnboxMethod,
                        enumerator.Location,
                        converter.Scope),
                    new JST.Expression[]
                    {
                        JST.IdentifierExpression.Create(
                            enumerator.Location,
                            converter.Scope,
                            converter.Resolve(expectedType)),
                        rv
                    },
                    converter,
                    converter.RuntimeManager);
            }

            return rv;
        }

        /// <summary>
        /// Gets the scope block.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="enumerator">The enumerator.</param>
        /// <param name="loopVariable">The loop variable.</param>
        /// <param name="scopeBlock">The scope block.</param>
        /// <returns>Scope block for foreachBlock</returns>
        private static JST.ScopeBlock GetScopeBlock(
            IMethodScopeConverter converter,
            JST.Expression enumerator,
            LocalVariable loopVariable,
            ScopeBlock scopeBlock)
        {
            JST.Statement block =
                ScopeBlockConverter.Convert(
                    converter,
                    scopeBlock);

            List<JST.Statement> statements = new List<JST.Statement>();
            JST.ScopeBlock returnValue = block as JST.ScopeBlock;

            if (returnValue == null)
            {
                statements.Add(block);
            }
            else
            {
                statements.AddRange(returnValue.Statements);
            }

            statements.Insert(
                0,
                new JST.ExpressionStatement(
                    enumerator.Location,
                    converter.Scope,
                    new JST.BinaryExpression(
                        enumerator.Location,
                        converter.Scope,
                        JST.BinaryOperator.Assignment,
                        new JST.IdentifierExpression(
                            converter.ResolveLocal(
                                loopVariable.Name),
                            converter.Scope),
                        ForLoopConverter.GetCurrentExpression(
                            converter,
                            loopVariable.Type,
                            enumerator))));

            return new JST.ScopeBlock(
                scopeBlock.Location,
                converter.Scope,
                statements);
        }
    }
}