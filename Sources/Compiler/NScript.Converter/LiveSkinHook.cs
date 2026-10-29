//-----------------------------------------------------------------------
// <copyright file="LiveSkinHook.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter
{
    using NScript.JST;
    using System.Collections.Generic;

    /// <summary>
    /// Live patch emit: the code a template plugin writes so that a patched skin goes live in
    /// the open page (<see cref="ConverterContext.LivePatchHook"/>). The helpers hang off each
    /// template plugin's own document storage getter.
    /// </summary>
    public static class LiveSkinHook
    {
        private const string TrackKey = "__nsTrack";

        private const string ReskinKey = "__nsReskin";

        /// <summary>
        /// The helpers, set once as the bundle loads:
        /// <c>DocStorageGetter.__nsTrack(instance, skin)</c> keeps each skin's instances
        /// (weakly), and <c>DocStorageGetter.__nsReskin(skin, factory, id, name)</c> gives the
        /// skin the patch's factory and skins each control that shows it again. The skin keeps
        /// its identity, so every holder of it (a list's item skin) makes the new look too.
        /// </summary>
        /// <param name="factoryField">Skin's factory field.</param>
        /// <param name="idField">Skin's id field.</param>
        /// <param name="parentField">SkinInstance's control field.</param>
        /// <param name="instanceField">The control's SkinInstance field.</param>
        /// <param name="setSkin"><c>function(c, v) { c.Skin = v; }</c>.</param>
        public static List<Statement> CreateHelpers(
            IdentifierScope root,
            IIdentifier docStorageGetter,
            IIdentifier factoryField,
            IIdentifier idField,
            IIdentifier parentField,
            IIdentifier instanceField,
            Expression setSkin)
        {
            Expression FieldName(IIdentifier field)
                => new IdentifierStringExpression(null, root, new IdentifierExpression(field, root));

            return new List<Statement>
            {
                ExpressionStatement.CreateAssignmentExpression(
                    GetterKey(root, docStorageGetter, TrackKey),
                    new ScriptLiteralExpression(null, root, "function(si,s){(s.__nsInsts||(s.__nsInsts=[])).push(new WeakRef(si));return si;}")),

                // F: the skin's factory field, I: its id, P: an instance's control, K: the control's instance.
                ExpressionStatement.CreateAssignmentExpression(
                    GetterKey(root, docStorageGetter, ReskinKey),
                    new MethodCallExpression(
                        null,
                        root,
                        new ScriptLiteralExpression(
                            null,
                            root,
                            "(function(F,I,P,K,set){return function(s,f,id,name){s[F]=f;s[I]=id;var a=s.__nsInsts||[],n=0;s.__nsInsts=[];"
                            + "a.forEach(function(w){var si=w.deref(),c=si&&si[P];if(c&&c[K]===si){set(c,null);set(c,s);n++;}});"
                            + "console.info('[nscript-hmr] reskinned '+n+' '+name);};})"),
                        FieldName(factoryField),
                        FieldName(idField),
                        FieldName(parentField),
                        FieldName(instanceField),
                        setSkin)),
            };
        }

        /// <summary>A skin factory's result, kept on its skin (<see cref="CreateHelpers"/>).</summary>
        public static Expression CreateTrack(IdentifierScope scope, IIdentifier docStorageGetter, Expression instance, Expression skin)
            => new MethodCallExpression(null, scope, GetterKey(scope, docStorageGetter, TrackKey), instance, skin);

        /// <summary>
        /// The statement a patch-emit bundle has for <c>Name_var = null;</c>:
        /// <c>if (Name_var) DocStorageGetter.__nsReskin(Name_var, Name_factory, "id", "Name");</c>.
        /// It does nothing as the bundle loads. Run again by a live patch, it gives the skin
        /// made before the patch the new factory and skins its controls again.
        /// </summary>
        public static Statement CreateReskin(
            IdentifierScope scope,
            IIdentifier docStorageGetter,
            IIdentifier skinVariable,
            IIdentifier factory,
            string id,
            string name)
            => new IfBlockStatement(
                null,
                scope,
                new IdentifierExpression(skinVariable, scope),
                new ScopeBlock(
                    null,
                    scope,
                    new List<Statement>
                    {
                        ExpressionStatement.CreateMethodCallExpression(
                            GetterKey(scope, docStorageGetter, ReskinKey),
                            new IdentifierExpression(skinVariable, scope),
                            new IdentifierExpression(factory, scope),
                            new StringLiteralExpression(scope, id),
                            new StringLiteralExpression(scope, name)),
                    }),
                null);

        private static Expression GetterKey(IdentifierScope scope, IIdentifier docStorageGetter, string key)
            => new IndexExpression(
                null,
                scope,
                new IdentifierExpression(docStorageGetter, scope),
                new StringLiteralExpression(scope, key));
    }
}
