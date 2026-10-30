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
        /// The queue's flush: rebuilds the open controls of each skin the patch gave a new
        /// factory, outer controls first (by element depth). A control an outer rebuild disposed
        /// no longer shows its instance and is skipped: the outer made it again, with the new
        /// inner factory. A bundle's XWML and Razor helpers share the queue on the global.
        /// </summary>
        private const string Flush =
            "globalThis.__nsFlush=globalThis.__nsFlush||function(){var q=globalThis.__nsQ||[],w=[];globalThis.__nsQ=[];"
            + "q.forEach(function(r){r.n=0;r.a.forEach(function(x){var si=x.deref(),c=si&&si[r.P];"
            + "if(c&&c[r.K]===si){for(var d=0,e=c[r.E];e;e=e.parentNode)d++;w.push([d,r,c,si]);}});});"
            + "w.sort(function(x,y){return x[0]-y[0];});"
            + "w.forEach(function(x){var r=x[1],c=x[2];if(c[r.K]===x[3]){r.set(c,null);r.set(c,r.s);r.n++;}});"
            + "q.forEach(function(r){var n=(r.s.__nsInsts||[]).length;"
            + "console.info('[nscript-hmr] reskinned '+n+' '+r.name+(n?' (reset: focus, caret, scroll, unbound typed text)':'')+(n>r.n?' ('+(n-r.n)+' by an outer skin)':''));});};";

        /// <summary>
        /// The helpers, set once as the bundle loads:
        /// <c>DocStorageGetter.__nsTrack(instance, skin)</c> keeps each skin's instances
        /// (weakly), and <c>DocStorageGetter.__nsReskin(skin, factory, id, name)</c> gives the
        /// skin the patch's factory and queues the controls that show it. The patch's last
        /// statement (<see cref="CreateFlush"/>) skins them again, once each, after every skin
        /// has its new factory. The skin keeps its identity, so every holder of it (a list's
        /// item skin) makes the new look too.
        /// </summary>
        /// <param name="factoryField">Skin's factory field.</param>
        /// <param name="idField">Skin's id field.</param>
        /// <param name="parentField">SkinInstance's control field.</param>
        /// <param name="instanceField">The control's SkinInstance field.</param>
        /// <param name="elementField">The control's element field.</param>
        /// <param name="setSkin"><c>function(c, v) { c.Skin = v; }</c>.</param>
        public static List<Statement> CreateHelpers(
            IdentifierScope root,
            IIdentifier docStorageGetter,
            IIdentifier factoryField,
            IIdentifier idField,
            IIdentifier parentField,
            IIdentifier instanceField,
            IIdentifier elementField,
            Expression setSkin)
        {
            Expression FieldName(IIdentifier field)
                => new IdentifierStringExpression(null, root, new IdentifierExpression(field, root));

            return new List<Statement>
            {
                ExpressionStatement.CreateAssignmentExpression(
                    GetterKey(root, docStorageGetter, TrackKey),
                    new ScriptLiteralExpression(null, root, "function(si,s){(s.__nsInsts||(s.__nsInsts=[])).push(new WeakRef(si));return si;}")),

                new ExpressionStatement(null, root, new ScriptLiteralExpression(null, root, Flush)),

                // F: the skin's factory field, I: its id, P: an instance's control, K: the control's instance, E: its element.
                ExpressionStatement.CreateAssignmentExpression(
                    GetterKey(root, docStorageGetter, ReskinKey),
                    new MethodCallExpression(
                        null,
                        root,
                        new ScriptLiteralExpression(
                            null,
                            root,
                            "(function(F,I,P,K,E,set){return function(s,f,id,name){s[F]=f;s[I]=id;"
                            + "(globalThis.__nsQ||(globalThis.__nsQ=[])).push({s:s,a:s.__nsInsts||[],name:name,P:P,K:K,E:E,set:set});s.__nsInsts=[];};})"),
                        FieldName(factoryField),
                        FieldName(idField),
                        FieldName(parentField),
                        FieldName(instanceField),
                        FieldName(elementField),
                        setSkin)),
            };
        }

        /// <summary>
        /// A live patch script's last statement: skins again the controls its re-skins queued
        /// (<see cref="CreateHelpers"/>). A bundle with no live skin helpers has no flush.
        /// </summary>
        public static Statement CreateFlush(IdentifierScope scope)
            => new ExpressionStatement(null, scope, new ScriptLiteralExpression(null, scope, "globalThis.__nsFlush&&globalThis.__nsFlush()"));

        /// <summary>A skin factory's result, kept on its skin (<see cref="CreateHelpers"/>).</summary>
        public static Expression CreateTrack(IdentifierScope scope, IIdentifier docStorageGetter, Expression instance, Expression skin)
            => new MethodCallExpression(null, scope, GetterKey(scope, docStorageGetter, TrackKey), instance, skin);

        /// <summary>
        /// The statement a patch-emit bundle has for <c>Name_var = null;</c>:
        /// <c>if (Name_var) DocStorageGetter.__nsReskin(Name_var, Name_factory, "id", "Name");</c>.
        /// It does nothing as the bundle loads. Run again by a live patch, it gives the skin
        /// made before the patch the new factory and queues its controls for the flush.
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

                            // A watch build's XWML template id is its path: the console names the file.
                            new StringLiteralExpression(scope, name.Substring(name.LastIndexOfAny(new[] { '\\', '/' }) + 1))),
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
