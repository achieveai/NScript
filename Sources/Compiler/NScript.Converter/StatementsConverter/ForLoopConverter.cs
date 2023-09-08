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
    using System;

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
                    ExpressionConverterBase.Convert(converter, forLoop.Condition),
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

                // for (enumTemp1 = collection.GetEnumerator(); await enumTemp1.MoveNext();) {
                //      looopVariable = enumTemp1.Current
                // 

                var forLoopInitialization = GetForLoopInitialization(
                    converter,
                    forEachLoop,
                    out var collectionTempIdentifierExpr,
                    out var enumeratorTempIdentifierExpr);

                var condition = GetForLoopCondition(
                    converter,
                    enumeratorTempIdentifierExpr,
                    forEachLoop);

                return new JST.ForLoop(
                    forEachLoop.Location,
                    converter.Scope,
                    condition,
                    forLoopInitialization,
                    null,
                    GetScopeBlock(
                        converter,
                        enumeratorTempIdentifierExpr,
                        forEachLoop.Variable,
                        forEachLoop.Scope,
                        forEachLoop));
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
        /// Gets the current expression.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="enumerator">The enumerator.</param>
        /// <returns></returns>
        private static JST.Expression GetCurrentExpression(
            IMethodScopeConverter converter,
            TypeReference expectedType,
            JST.Expression enumerator,
            ForEachLoop forEachLoop)
        {
            var isAsyncEnumerator = forEachLoop.IsAsync;

            var getCurrentFunc = isAsyncEnumerator
                ? converter.KnownReferences.GetCurrentIAsyncEnumeratorMethod
                : converter.KnownReferences.GetCurrentIEnumeratorMethod;

            if (isAsyncEnumerator)
            {
                getCurrentFunc = getCurrentFunc.FixGenericTypeArguments(forEachLoop.Collection.ResultType);
            }

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
            if (!forEachLoop.IsAsync
                && (expectedType.IsValueOrEnum()
                || expectedType.IsGenericParameter))
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

        private static JST.Statement GetForLoopInitialization(
            IMethodScopeConverter converter,
            ForEachLoop forEachLoop,
            out JST.IdentifierExpression collectionTempIdentifier,
            out JST.IdentifierExpression enumeratorTempIdentifier)
        {
            var getAsyncEnumerator = forEachLoop.IsAsync;

            // TODO: Generic case
            var method = getAsyncEnumerator
                ? converter.KnownReferences.GetAsyncEnumeratorIAsyncEnumerableMethod
                : converter.KnownReferences.GetEnumeratorIEnumerableMethod;

            if (getAsyncEnumerator)
            {
                method = method.FixGenericTypeArguments(forEachLoop.Collection.ResultType);
            }

            enumeratorTempIdentifier = new JST.IdentifierExpression(
                converter.GetTempVariable(),
                converter.Scope);

            collectionTempIdentifier= new JST.IdentifierExpression(
                converter.GetTempVariable(),
                converter.Scope);

            var collectionAssignmentStatement =
                new JST.ExpressionStatement(
                    null,
                    converter.Scope,
                    new JST.BinaryExpression(
                        null,
                        converter.Scope,
                        JST.BinaryOperator.Assignment,
                        collectionTempIdentifier,
                        ExpressionConverterBase.Convert(converter, forEachLoop.Collection)));

            var enumeratorAssignmentStatement = new JST.ExpressionStatement(
                null,
                converter.Scope,
                new JST.BinaryExpression(
                    null,
                    converter.Scope,
                    JST.BinaryOperator.Assignment,
                    enumeratorTempIdentifier,
                    new JST.MethodCallExpression(
                        forEachLoop.Location,
                        converter.Scope,
                        new JST.IndexExpression(
                            forEachLoop.Location,
                            converter.Scope,
                            collectionTempIdentifier,
                            converter.ResolveVirtualMethod(
                                method,
                                converter.Scope)),
                        new List<JST.Expression>())));

            return new JST.ScopeBlock(
                null,
                converter.Scope,
                new()
                {
                    collectionAssignmentStatement,
                    enumeratorAssignmentStatement
                });
        }

        private static JST.Expression GetForLoopCondition(
            IMethodScopeConverter converter,
            JST.IdentifierExpression enumeratorIdentifier,
            ForEachLoop forEachLoop)
        {
            var method = forEachLoop.IsAsync
                ? converter.KnownReferences.GetMoveNextAsyncIAsyncEnumeratorMethod(
                    forEachLoop.Collection.ResultType)
                : converter.KnownReferences.MoveNextEnumeratorMethod;

            // if (forEachLoop.IsAsync)
            // {
            //     method = method.FixGenericTypeArguments(forEachLoop.Collection.ResultType);
            // }

            JST.Expression rv = new JST.MethodCallExpression(
                forEachLoop.Location,
                converter.Scope,
                new JST.IndexExpression(
                    forEachLoop.Location,
                    converter.Scope,
                    enumeratorIdentifier,
                    converter.ResolveVirtualMethod(
                        method,
                        converter.Scope)),
                new List<JST.Expression>());

            if (forEachLoop.IsAsync)
            {
                // Wrap in an await expression
                rv = new JST.AwaitExpression(forEachLoop.Collection.Location, converter.Scope, rv);
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
            ScopeBlock scopeBlock,
            ForEachLoop forEachLoop)
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
                        GetCurrentExpression(
                            converter,
                            loopVariable.Type,
                            enumerator,
                            forEachLoop))));

            return new JST.ScopeBlock(
                scopeBlock.Location,
                converter.Scope,
                statements);
        }
    }
}