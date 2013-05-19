//-----------------------------------------------------------------------
// <copyright file="NullableTypeConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.TypeSystemConverter
{
    using System;
    using System.Collections.Generic;
    using Mono.Cecil;

    /// <summary>
    /// Definition for NullableTypeConverter
    /// </summary>
    public class NullableTypeConverter : StructTypeConverter
    {
        private MethodConverter boxMethod;
        private MethodConverter unboxMehtod;

        public NullableTypeConverter(
            RuntimeScopeManager scopeManager,
            TypeDefinition typeDefinition,
            bool isSelectiveInit)
            : base(scopeManager, typeDefinition, isSelectiveInit)
        {
            this.boxMethod = new MethodConverter(
                this,
                this.Context.KnownReferences.NullableBoxMethod.Resolve());

            this.unboxMehtod = new MethodConverter(
                this,
                this.Context.KnownReferences.NullableUnboxMethod.Resolve());
        }

        protected override void InitializeStaticFunctions(List<JST.Statement> statements)
        {
            base.InitializeStaticFunctions(statements);

            // Add binding for BoxMethod.
            statements.Add(
                JST.ExpressionStatement.CreateAssignmentExpression(
                    new JST.IndexExpression(
                        null,
                        this.Scope,
                        JST.IdentifierExpression.Create(
                            null,
                            this.Scope,
                            this.Resolve(this.TypeDefinition)),
                        new JST.IdentifierExpression(
                            this.Resolve(this.Context.KnownReferences.TypeNullableBoxMethod),
                            this.Scope)),
                    this.boxMethod.MethodFunctionExpression));
        }

        protected override List<Tuple<JST.Expression, JST.Expression>> GetPrototypeInitializers(JST.Expression prototype)
        {
            return new List<Tuple<JST.Expression, JST.Expression>>();
        }

        protected override List<JST.ExpressionStatement> InitializeVirtuals(JST.Expression prototype)
        {
            return new List<JST.ExpressionStatement>();
        }
    }
}