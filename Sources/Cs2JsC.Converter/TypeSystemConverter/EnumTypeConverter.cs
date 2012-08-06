//-----------------------------------------------------------------------
// <copyright file="EnumTypeConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.TypeSystemConverter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cs2JsC.CLR;
    using Cs2JsC.JST;
    using Mono.Cecil;

    /// <summary>
    /// Definition for EnumTypeConverter
    /// </summary>
    public class EnumTypeConverter : StructTypeConverter
    {
        public EnumTypeConverter(
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
            get { return this.Context.KnownReferences.RegisterEnumMethod; }
        }

        /// <summary>
        /// Initializes the static variables.
        /// </summary>
        /// <param name="statements">The statements.</param>
        protected override void InitializeStaticVariables(System.Collections.Generic.List<Cs2JsC.JST.Statement> statements)
        {
            base.InitializeStaticVariables(statements);

            Expression thisTypeExpression = IdentifierExpression.Create(
                null,
                this.Scope,
                this.Resolve(this.TypeDefinition));

            Expression enumStrToValueMapping = new IndexExpression(
                null,
                this.Scope,
                thisTypeExpression,
                new IdentifierExpression(
                    this.Resolve(this.Context.KnownReferences.EnumStrToValueMapField),
                    this.Scope));

            var mapping = new InlineObjectInitializer(
                null,
                this.Scope);

            // This is Enum so let's initialize enum strings.
            foreach (var field in this.TypeDefinition.Fields)
            {
                if (!field.HasConstant)
                {
                    continue;
                }

                if (!field.FieldType.IsSameDefinition(this.TypeDefinition))
                {
                    continue;
                }

                Expression numberLiteralExpression =
                    TypeConverter.ConvertConstValue(field.Constant);

                mapping.AddInitializer(
                    field.Name,
                    numberLiteralExpression);
            }

            statements.Add(
                new ExpressionStatement(
                    null,
                    this.Scope,
                    new BinaryExpression(
                        null,
                        this.Scope,
                        BinaryOperator.Assignment,
                        enumStrToValueMapping,
                        mapping)));
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
        protected override IList<Expression>  RegisterTypeMethodArguments(
            Expression typeNameExpression,
            Expression parentTypeExpression,
            List<Expression> interfaces)
        {
            return new Expression[]
                {
                    typeNameExpression,
                    new BooleanLiteralExpression(
                        this.Scope,
                        null != this.TypeDefinition.CustomAttributes.SelectAttribute(
                            this.Context.ClrKnownReferences.FlagsAttributeType))
                };
        }
    }
}
