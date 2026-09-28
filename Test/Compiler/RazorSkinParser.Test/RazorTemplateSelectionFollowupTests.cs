using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Cecil;
using NScript.JST;
using NScript.RazorSkin;
using NScript.RazorSkin.CodeGen;
using NScript.RazorSkin.TemplateIR;

namespace RazorSkinParser.Test
{
    [TestClass]
    public class RazorTemplateSelectionFollowupTests
    {
        [TestMethod]
        public void UniqueShortSkinNameFindsCompatibleTemplate()
        {
            var ownerModule = ModuleDefinition.CreateModule("Owner", ModuleKind.Dll);
            var viewModule = ModuleDefinition.CreateModule("Views", ModuleKind.Dll);
            var owner = AddType(ownerModule, "Tests", "Owner");
            var key = Key(viewModule, "Views.Unique.skin.cshtml");
            var templates = Templates((key, "Tests.Owner"));
            var shortNames = Names((key, "Unique"));

            var selected = RazorTemplatingPlugin.SelectTemplateKey(ownerModule, "Unique", owner,
                templates, shortNames, (name, imports) =>
                    RazorSkinJSTGenerator.ResolveSubControlType(new[] { owner }, name, imports),
                out var error);

            selected.Should().Be(key);
            error.Should().BeNull();
        }

        [TestMethod]
        public void UniqueShortSkinNameFindsTemplateFromNonControlHolder()
        {
            var ownerModule = ModuleDefinition.CreateModule("Owner", ModuleKind.Dll);
            var viewModule = ModuleDefinition.CreateModule("Views", ModuleKind.Dll);
            var holder = AddType(ownerModule, "Tests", "SkinCatalog");
            var control = AddType(viewModule, "Tests", "Control");
            var key = Key(viewModule, "Views.Unique.skin.cshtml");
            var templates = Templates((key, control.FullName));
            var shortNames = Names((key, "Unique"));

            var selected = RazorTemplatingPlugin.SelectTemplateKey(ownerModule, "Unique", holder,
                templates, shortNames, (name, imports) =>
                    RazorSkinJSTGenerator.ResolveSubControlType(new[] { holder, control }, name, imports),
                out var error);

            selected.Should().Be(key);
            error.Should().BeNull();
        }

        [TestMethod]
        public void ShortSkinCollisionPrefersMostDerivedCompatibleControl()
        {
            var ownerModule = ModuleDefinition.CreateModule("Owner", ModuleKind.Dll);
            var viewModule = ModuleDefinition.CreateModule("Views", ModuleKind.Dll);
            var baseType = AddType(ownerModule, "Tests", "Base");
            var owner = AddType(ownerModule, "Tests", "Owner", baseType);
            var baseKey = Key(viewModule, "Views.Base.Shared.skin.cshtml");
            var ownerKey = Key(viewModule, "Views.Owner.Shared.skin.cshtml");
            var templates = Templates((baseKey, "Tests.Base"), (ownerKey, "Tests.Owner"));
            var shortNames = Names((baseKey, "Shared"), (ownerKey, "Shared"));

            var selected = RazorTemplatingPlugin.SelectTemplateKey(ownerModule, "Shared", owner,
                templates, shortNames, (name, imports) =>
                    RazorSkinJSTGenerator.ResolveSubControlType(new[] { baseType, owner }, name, imports),
                out var error);

            selected.Should().Be(ownerKey);
            error.Should().BeNull();
        }

        [TestMethod]
        public void ShortSkinCollisionIgnoresIncompatibleControl()
        {
            var ownerModule = ModuleDefinition.CreateModule("Owner", ModuleKind.Dll);
            var viewModule = ModuleDefinition.CreateModule("Views", ModuleKind.Dll);
            var owner = AddType(ownerModule, "Tests", "Owner");
            var other = AddType(ownerModule, "Tests", "Other");
            var ownerKey = Key(viewModule, "Views.Owner.Shared.skin.cshtml");
            var otherKey = Key(viewModule, "Views.Other.Shared.skin.cshtml");
            var templates = Templates((ownerKey, "Tests.Owner"), (otherKey, "Tests.Other"));
            var shortNames = Names((ownerKey, "Shared"), (otherKey, "Shared"));

            var selected = RazorTemplatingPlugin.SelectTemplateKey(ownerModule, "Shared", owner,
                templates, shortNames, (name, imports) =>
                    RazorSkinJSTGenerator.ResolveSubControlType(new[] { owner, other }, name, imports),
                out var error);

            selected.Should().Be(ownerKey);
            error.Should().BeNull();
        }

        [TestMethod]
        public void ShortSkinControlNameUsesTemplateImports()
        {
            var ownerModule = ModuleDefinition.CreateModule("Owner", ModuleKind.Dll);
            var viewModule = ModuleDefinition.CreateModule("Views", ModuleKind.Dll);
            var owner = AddType(ownerModule, "Tests", "Owner");
            var conflicting = AddType(ownerModule, "Other", "Owner");
            var key = Key(viewModule, "Views.Imported.skin.cshtml");
            var templates = Templates((key, "Owner"));
            templates[key].UsingNamespaces.Add("Tests");
            var shortNames = Names((key, "Imported"));

            var selected = RazorTemplatingPlugin.SelectTemplateKey(ownerModule, "Imported", owner,
                templates, shortNames, (name, imports) =>
                    RazorSkinJSTGenerator.ResolveSubControlType(new[] { owner, conflicting }, name, imports),
                out var error);

            selected.Should().Be(key);
            error.Should().BeNull();
        }

        [TestMethod]
        public void UnresolvedShortSkinCollisionProducesRuntimeThrowWithCandidates()
        {
            var ownerModule = ModuleDefinition.CreateModule("Owner", ModuleKind.Dll);
            var viewModule = ModuleDefinition.CreateModule("Views", ModuleKind.Dll);
            var owner = AddType(ownerModule, "Tests", "Owner");
            var firstKey = Key(viewModule, "Views.A.Shared.skin.cshtml");
            var secondKey = Key(viewModule, "Views.B.Shared.skin.cshtml");
            var templates = Templates((firstKey, "Tests.Owner"), (secondKey, "Tests.Owner"));
            var shortNames = Names((firstKey, "Shared"), (secondKey, "Shared"));

            var selected = RazorTemplatingPlugin.SelectTemplateKey(ownerModule, "Shared", owner,
                templates, shortNames, (name, imports) =>
                    RazorSkinJSTGenerator.ResolveSubControlType(new[] { owner }, name, imports),
                out var error);

            selected.Should().BeNull();
            error.Should().Contain("Views.A.Shared.skin.cshtml").And.Contain("Views.B.Shared.skin.cshtml");
            var statement = RazorTemplatingPlugin.CreateTemplateAmbiguityThrow(new IdentifierScope(false), error)
                .Should().BeOfType<ThrowStatement>().Which;
            statement.Expression.Should().BeOfType<StringLiteralExpression>().Which.StringLiteral
                .Should().Contain("Views.A.Shared.skin.cshtml");
        }

        [TestMethod]
        public void ExactOwnResourceIdentityWinsEvenWithShortNameCollision()
        {
            var ownerModule = ModuleDefinition.CreateModule("Owner", ModuleKind.Dll);
            var otherModule = ModuleDefinition.CreateModule("Other", ModuleKind.Dll);
            var owner = AddType(ownerModule, "Tests", "Owner");
            var resource = "Views.Shared.skin.cshtml";
            var ownKey = Key(ownerModule, resource);
            var otherKey = Key(otherModule, resource);
            var templates = Templates((ownKey, "Tests.Owner"), (otherKey, "Tests.Owner"));
            var shortNames = Names((ownKey, "Shared"), (otherKey, "Shared"));

            var selected = RazorTemplatingPlugin.SelectTemplateKey(ownerModule, resource, owner,
                templates, shortNames, (name, imports) =>
                    RazorSkinJSTGenerator.ResolveSubControlType(new[] { owner }, name, imports),
                out var error);

            selected.Should().Be(ownKey);
            error.Should().BeNull();
        }

        [TestMethod]
        public void DemandTypeLookupUsesImportsAndRejectsAmbiguity()
        {
            var module = ModuleDefinition.CreateModule("Types", ModuleKind.Dll);
            var first = AddType(module, "First", "SearchBox");
            var second = AddType(module, "Second", "SearchBox");
            var types = new[] { first, second };

            RazorSkinJSTGenerator.ResolveSubControlType(types, "SearchBox", new[] { "Second" })
                .Should().BeSameAs(second);
            Action ambiguous = () => RazorSkinJSTGenerator.ResolveSubControlType(types,
                "SearchBox", new[] { "First", "Second" });
            ambiguous.Should().Throw<InvalidOperationException>()
                .WithMessage("*Ambiguous sub-control*First.SearchBox*Second.SearchBox*");
        }

        private static TypeDefinition AddType(ModuleDefinition module, string ns, string name,
            TypeReference baseType = null)
        {
            var type = new TypeDefinition(ns, name, TypeAttributes.Public, baseType);
            module.Types.Add(type);
            return type;
        }

        private static string Key(ModuleDefinition module, string resource)
            => module.Mvid.ToString("N") + "|" + resource;

        private static Dictionary<string, SkinTemplateNode> Templates(params (string Key, string Control)[] entries)
        {
            var result = new Dictionary<string, SkinTemplateNode>();
            foreach (var entry in entries)
                result.Add(entry.Key, new SkinTemplateNode { ControlTypeName = entry.Control });
            return result;
        }

        private static Dictionary<string, string> Names(params (string Key, string ShortName)[] entries)
        {
            var result = new Dictionary<string, string>();
            foreach (var entry in entries)
                result.Add(entry.Key, entry.ShortName);
            return result;
        }
    }
}
