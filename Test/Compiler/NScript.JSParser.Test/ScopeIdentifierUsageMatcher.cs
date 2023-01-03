namespace NScript.JSParser.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using MoreLinq;

    using Newtonsoft.Json;

    using NScript.JST;

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;

    public class ScopeIdentifierUsageMatcher
    {
        private static readonly IDeserializer S_deserializer = new DeserializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .Build();
        private bool _initialized = false;
        private string[] _pis;
        private string[] _dis;
        private string _declaredIdentifiers;
        private string _parameterIdentifiers;

        public string Decls
        {
            get => _declaredIdentifiers;
            set
            {
                _declaredIdentifiers = value;
                _initialized = false;
            }
        }

        public string Params
        {
            get => _parameterIdentifiers;
            set
            {
                _parameterIdentifiers = value;
                _initialized = false;
            }
        }

        public Dictionary<string, int> UsedIdentifiers { get; } = new Dictionary<string, int>();

        public List<ScopeIdentifierUsageMatcher> SubMatchers { get; set; }

        private bool Init()
        {
            if (_initialized) return true;

            UsedIdentifiers.Clear();
            _pis = _parameterIdentifiers?.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
            _dis = _declaredIdentifiers?.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries) ?? new string[0];

            ParseIdentifiers(_pis);
            ParseIdentifiers(_dis);

            return true;
        }

        private void ParseIdentifiers(string[] idents)
        {
            for (int i = 0; i < idents.Length; i++)
            {
                var sp = idents[i].Trim().Split(":");
                idents[i] = sp[0].Trim();
                UsedIdentifiers[sp[0]] = int.Parse(sp[1].Trim());
            }
        }

        public bool IsMatch(IdentifierScope scope)
        {
            return Init()
                && (SubMatchers == null || scope.ChildScopes.Count == SubMatchers.Count)
                && scope.ParameterIdentifiers.Count == (_pis?.Length ?? 0)
                && scope.ScopedIdentifiers.Count == (_dis?.Length ?? 0)
                && scope.ParameterIdentifiers.All(pi => _pis.Contains(pi.SuggestedName))
                && scope.ScopedIdentifiers.All(si => _dis.Contains(si.SuggestedName));
        }

        public void AssertUsage(IdentifierScope scope)
        {
            Assert.IsTrue(IsMatch(scope));
            CheckUsages(scope);

            scope.ChildScopes
                .ForEach(scope =>
                {
                    var matcher = SubMatchers.FirstOrDefault(sm => sm.IsMatch(scope));
                    Assert.IsNotNull(matcher);

                    matcher.AssertUsage(scope);
                });
        }

        public static List<ScopeIdentifierUsageMatcher> ReadJsonFromResource(string resourceFileName)
        {
            var jsScript = JSParserAndGeneratorHelper.GetResourceString(resourceFileName);
            return (List<ScopeIdentifierUsageMatcher>)JsonSerializer.CreateDefault().Deserialize(
                new StringReader(jsScript),
                typeof(List<ScopeIdentifierUsageMatcher>));
        }

        public static List<ScopeIdentifierUsageMatcher> ListReadYamlFromResource(string resourceFileName)
        {
            var jsScript = JSParserAndGeneratorHelper.GetResourceString(resourceFileName);
            return S_deserializer.Deserialize<List<ScopeIdentifierUsageMatcher>>(jsScript);
        }

        public static ScopeIdentifierUsageMatcher ReadYamlFromResource(string resourceFileName)
        {
            var jsScript = JSParserAndGeneratorHelper.GetResourceString(resourceFileName);
            return S_deserializer.Deserialize<ScopeIdentifierUsageMatcher>(jsScript);
        }

        private void CheckUsages(IdentifierScope scope)
        {
            scope.ParameterIdentifiers.ForEach(
                ident => Assert.AreEqual(
                    UsedIdentifiers[ident.SuggestedName],
                    ident.UsageCount,
                    $"Usage for {ident.SuggestedName}"));
        }
    }
}
