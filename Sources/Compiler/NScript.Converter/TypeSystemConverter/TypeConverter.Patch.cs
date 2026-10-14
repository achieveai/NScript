namespace NScript.Converter.TypeSystemConverter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Mono.Cecil;
    using NScript.CLR;
    using NScript.JST;

    /// <summary>
    /// Dev patch emit (<see cref="DevPatchSession"/>): converts a method of an emitted type
    /// again and makes the statements that put it in place in a written bundle, or in a
    /// running page. Each statement works wherever it runs after the type's section.
    /// </summary>
    public abstract partial class TypeConverter
    {
        /// <summary>
        /// Dev mode: the copies <c>prototype[slot] = prototype[source]</c> this type's section
        /// makes while the bundle loads. A new function in <c>source</c> must be copied again.
        /// The slot is an expression: an interface slot is <c>"Name_" + I.typeId</c>.
        /// </summary>
        private List<(Expression Slot, IIdentifier Source)> slotCopies;

        /// <summary>Dev mode: see <see cref="slotCopies"/>.</summary>
        internal IReadOnlyList<(Expression Slot, IIdentifier Source)> SlotCopies
            => (IReadOnlyList<(Expression Slot, IIdentifier Source)>)this.slotCopies
                ?? Array.Empty<(Expression Slot, IIdentifier Source)>();

        /// <summary>The methods this type emitted.</summary>
        internal IReadOnlyList<MethodDefinition> ImplementedMethods => this.implementedMethods.Keys.ToList();

        /// <summary>Why no method of this type can be patched, or null.</summary>
        internal string PatchMiss()
        {
            if (this.IsGenericLike && !this.context.LivePatchHook)
            {
                return "generic type";
            }

            if (!(this is ReferenceTypeConverter)
                || this.typeDefinition.IsInterface
                || this.context.IsExtended(this.typeDefinition)
                || this.context.IsPsudoType(this.typeDefinition)
                || this.context.IsJsonType(this.typeDefinition))
            {
                return "type kind";
            }

            return null;
        }

        /// <summary>Why this method cannot be patched, or null.</summary>
        internal string PatchMiss(MethodDefinition method)
        {
            if (method.HasGenericParameters)
            {
                return "generic method";
            }

            return null;
        }

        /// <summary>The method's emitted converter.</summary>
        internal MethodConverter ImplementedMethod(MethodDefinition method)
            => this.GetMethodConverter(method);

        /// <summary>Converts the method again. The type keeps the old one until <see cref="ReplaceMethod"/>.</summary>
        internal MethodConverter ConvertAgain(MethodDefinition method)
            => new MethodConverter(this, method);

        /// <summary>Makes <paramref name="converter"/> the method's emitted converter.</summary>
        internal void ReplaceMethod(MethodDefinition method, MethodConverter converter)
            => this.implementedMethods[method] = converter;

        /// <summary>
        /// The statement that puts the converter's function in the method's place: a global
        /// static function is declared again (in a bundle the last declaration wins), a
        /// static member or a prototype slot is assigned. Null when the section does not
        /// emit the method in one of these ways.
        /// </summary>
        internal Statement CreateOverride(MethodDefinition method, MethodConverter converter)
        {
            if (converter.IsGlobalStaticImplementation)
            {
                return new ExpressionStatement(null, this.Scope, converter.MethodFunctionExpression);
            }

            if (method.IsStatic)
            {
                return ExpressionStatement.CreateAssignmentExpression(
                    IdentifierExpression.Create(null, this.Scope, this.ResolveStaticMember(method)),
                    converter.MethodFunctionExpression);
            }

            // As GetPrototypeInitializers: these instance methods get no prototype slot.
            if (method.CustomAttributes.SelectAttribute(this.Context.KnownReferences.MakeStaticUsageAttribute) != null
                || (method.IsConstructor && this.Context.IsExtended(this.TypeDefinition)))
            {
                return null;
            }

            return ExpressionStatement.CreateAssignmentExpression(
                new IndexExpression(
                    null,
                    this.Scope,
                    this.CreatePrototypeExpression(),
                    new IdentifierExpression(this.Resolve(method), this.Scope)),
                converter.MethodFunctionExpression);
        }

        /// <summary>The identifier of the prototype slot an instance method is emitted in.</summary>
        internal IIdentifier ResolvePrototypeSlot(MethodDefinition method)
            => this.Resolve(method);

        /// <summary><c>T.prototype[slot] = T.prototype[source]</c>.</summary>
        internal Statement CreateSlotCopy(Expression slot, IIdentifier source)
            => ExpressionStatement.CreateAssignmentExpression(
                new IndexExpression(
                    null,
                    this.Scope,
                    this.CreatePrototypeExpression(),
                    slot),
                new IndexExpression(
                    null,
                    this.Scope,
                    this.CreatePrototypeExpression(),
                    new IdentifierExpression(source, this.Scope)));

        /// <summary><c>T.prototype</c>, which unlike the section's prototype variable holds anywhere.</summary>
        private Expression CreatePrototypeExpression()
            => new IndexExpression(
                null,
                this.Scope,
                IdentifierExpression.Create(null, this.Scope, this.Resolve(this.typeDefinition)),
                new IdentifierExpression(this.Resolve(this.cnvtKnownRefs.PrototypeField), this.Scope));

        /// <summary>
        /// Whether this type is a generic factory with a marker and eval hook in its closure
        /// (<see cref="AddFactoryPatchHook"/>): its patch goes before that marker.
        /// </summary>
        internal bool HasFactoryHook => this.IsGenericLike && this.context.LivePatchHook && this.PatchMiss() == null;

        /// <summary>
        /// Live patch emit, in a generic type's factory after its prototype: the marker a
        /// patch goes before, then <c>T["__nsEval"]</c>, an eval in the factory's closure,
        /// the instantiation added to <c>factory["__nsInsts"]</c>, and the live patches a
        /// page took run again for this instantiation (<c>factory["__nsPatches"]</c>).
        /// </summary>
        private void AddFactoryPatchHook(List<Statement> statements)
        {
            if (!this.HasFactoryHook)
            {
                return;
            }

            Expression TypeVariable() => IdentifierExpression.Create(null, this.Scope, this.Resolve(this.typeDefinition));
            Expression Factory() => IdentifierExpression.Create(null, this.Scope, this.RuntimeManager.ResolveType(this.typeDefinition));
            Expression Key(Expression target, string key) => new IndexExpression(null, this.Scope, target, new StringLiteralExpression(this.Scope, key));

            statements.Add(new MarkerStatement(this.Scope, MarkerStatement.ForFactory(this.typeDefinition.FullName)));
            statements.Add(ExpressionStatement.CreateAssignmentExpression(
                Key(TypeVariable(), EvalKey),
                new ScriptLiteralExpression(null, this.Scope, "function(){return eval(arguments[0]);}")));
            statements.Add(new ExpressionStatement(
                null,
                this.Scope,
                new MethodCallExpression(
                    null,
                    this.Scope,
                    Key(
                        new BinaryExpression(
                            null,
                            this.Scope,
                            BinaryOperator.LogicalOr,
                            Key(Factory(), InstancesKey),
                            new BinaryExpression(
                                null,
                                this.Scope,
                                BinaryOperator.Assignment,
                                Key(Factory(), InstancesKey),
                                new InlineNewArrayInitialization(null, this.Scope, new List<Expression>()))),
                        "push"),
                    TypeVariable())));
            statements.Add(new IfBlockStatement(
                null,
                this.Scope,
                Key(Factory(), PatchesKey),
                new ScopeBlock(
                    null,
                    this.Scope,
                    new List<Statement>
                    {
                        new ExpressionStatement(
                            null,
                            this.Scope,
                            new MethodCallExpression(
                                null,
                                this.Scope,
                                Key(Key(Factory(), PatchesKey), "forEach"),
                                Key(TypeVariable(), EvalKey))),
                    }),
                null));
        }

        /// <summary>
        /// The live form of a patch to a generic type's factory, run in the bundle's closure:
        /// <paramref name="fragment"/> runs in the closure of each instantiation made so far,
        /// and is kept for the ones made later.
        /// </summary>
        internal List<Statement> CreateLiveFactoryPatch(string fragment, IdentifierScope root)
        {
            Expression Factory() => IdentifierExpression.Create(null, root, this.RuntimeManager.ResolveType(this.typeDefinition));
            Expression Key(Expression target, string key) => new IndexExpression(null, root, target, new StringLiteralExpression(root, key));
            Expression OrNew(string key) => new BinaryExpression(
                null,
                root,
                BinaryOperator.LogicalOr,
                Key(Factory(), key),
                new BinaryExpression(
                    null,
                    root,
                    BinaryOperator.Assignment,
                    Key(Factory(), key),
                    new InlineNewArrayInitialization(null, root, new List<Expression>())));

            return new List<Statement>
            {
                new ExpressionStatement(
                    null,
                    root,
                    new MethodCallExpression(null, root, Key(OrNew(PatchesKey), "push"), new StringLiteralExpression(root, fragment))),
                new ExpressionStatement(
                    null,
                    root,
                    new MethodCallExpression(
                        null,
                        root,
                        Key(OrNew(InstancesKey), "forEach"),
                        new ScriptLiteralExpression(
                            null,
                            root,
                            "function(t){t[\"" + EvalKey + "\"](\"" + NScript.JST.Utils.ToJSString(fragment) + "\");}"))),
            };
        }

        private const string EvalKey = "__nsEval";
        private const string InstancesKey = "__nsInsts";
        private const string PatchesKey = "__nsPatches";

        private void RecordSlotCopies(Expression prototype, List<ExpressionStatement> virtualInitializers)
        {
            if (!this.context.DevMode)
            {
                return;
            }

            this.slotCopies = new List<(Expression Slot, IIdentifier Source)>();
            foreach (var statement in virtualInitializers)
            {
                if (statement.Expression is BinaryExpression { Operator: BinaryOperator.Assignment } assignment
                    && assignment.Left is IndexExpression left
                    && assignment.Right is IndexExpression { RightExpression: IdentifierExpression source } right
                    && left.LeftExpression == prototype
                    && right.LeftExpression == prototype)
                {
                    this.slotCopies.Add((left.RightExpression, source.Identifier));
                }
            }
        }
    }
}
