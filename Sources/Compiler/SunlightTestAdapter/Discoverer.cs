namespace SunlightTestAdapter;

using System.Reflection;
using System.Xml;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

[DefaultExecutorUri(Constants.ExecutorUri)]
[FileExtension(".dll")]
public class Discoverer : ITestDiscoverer
{
    private const string TestFixtureAttributeFullName = "SunlightUnit.TestFixtureAttribute";
    private const string TestAttributeFullName = "SunlightUnit.TestAttribute";

    public void DiscoverTests(
        IEnumerable<string> sources,
        IDiscoveryContext discoveryContext,
        IMessageLogger logger,
        ITestCaseDiscoverySink discoverySink)
    {
        foreach (var source in sources)
        {
            var effectiveSource = ResolveTestSourceAssembly(source, discoveryContext) ?? source;

            if (!string.Equals(effectiveSource, source, StringComparison.OrdinalIgnoreCase))
            {
                logger.SendMessage(
                    TestMessageLevel.Informational,
                    $"Discovering from TestSourceAssembly='{effectiveSource}' instead of '{source}'.");
            }

            var testCases = GetTests(effectiveSource);

            logger.SendMessage(
                TestMessageLevel.Informational,
                $"Sending {testCases.Length} test cases from source: {effectiveSource}");

            Array.ForEach(testCases, discoverySink.SendTestCase);
        }
    }

    public static TestCase[] GetTests(string source)
    {
        if (!File.Exists(source))
        {
            return Array.Empty<TestCase>();
        }

        // MetadataLoadContext reads type and attribute metadata without resolving
        // runtime references. The test DLL is NScript-compiled against a custom
        // mscorlib facade — its references can't be loaded into a normal .NET 8
        // AppDomain, so Assembly.LoadFile + GetTypes/GetCustomAttributes fails.
        // MetadataLoadContext sidesteps that entirely.
        var resolver = new PathAssemblyResolver(BuildResolverPaths(source));
        using var mlc = new MetadataLoadContext(resolver);

        Assembly asm;
        try
        {
            asm = mlc.LoadFromAssemblyPath(source);
        }
        catch
        {
            return Array.Empty<TestCase>();
        }

        var rv = new List<TestCase>();

        Type[] types;
        try
        {
            types = asm.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            types = ex.Types.Where(t => t != null).ToArray()!;
        }

        foreach (var ty in types)
        {
            if (ty == null || !HasAttribute(SafeGetAttributesData(() => ty.GetCustomAttributesData()), TestFixtureAttributeFullName))
            {
                continue;
            }

            MethodInfo[] methods;
            try
            {
                methods = ty.GetMethods();
            }
            catch
            {
                continue;
            }

            foreach (var method in methods)
            {
                if (!HasAttribute(SafeGetAttributesData(() => method.GetCustomAttributesData()), TestAttributeFullName))
                {
                    continue;
                }

                rv.Add(new TestCase
                {
                    ExecutorUri = new Uri(Constants.ExecutorUri),
                    DisplayName = method.Name,
                    FullyQualifiedName = (ty.Namespace ?? string.Empty) + "." + ty.Name + "." + method.Name,
                    Source = source,
                });
            }
        }

        return rv.ToArray();
    }

    private static IList<CustomAttributeData> SafeGetAttributesData(Func<IList<CustomAttributeData>> get)
    {
        try
        {
            return get();
        }
        catch
        {
            return Array.Empty<CustomAttributeData>();
        }
    }

    private static bool HasAttribute(IList<CustomAttributeData> attrs, string fullName)
    {
        foreach (var attr in attrs)
        {
            if (attr.AttributeType.FullName == fullName)
            {
                return true;
            }
        }
        return false;
    }

    private static IEnumerable<string> BuildResolverPaths(string source)
    {
        // Probe sibling DLLs (test DLL's own dependencies including SunlightUnit,
        // NScript framework facades, etc.) plus the .NET runtime directory so
        // CustomAttributeData can resolve attribute types.
        var paths = new List<string>();

        var sourceDir = Path.GetDirectoryName(source);
        if (!string.IsNullOrEmpty(sourceDir) && Directory.Exists(sourceDir))
        {
            paths.AddRange(Directory.GetFiles(sourceDir, "*.dll"));
        }

        var runtimeDir = Path.GetDirectoryName(typeof(object).Assembly.Location);
        if (!string.IsNullOrEmpty(runtimeDir) && Directory.Exists(runtimeDir))
        {
            paths.AddRange(Directory.GetFiles(runtimeDir, "*.dll"));
        }

        return paths.Distinct(StringComparer.OrdinalIgnoreCase);
    }

    internal static string? ResolveTestSourceAssembly(string source, IDiscoveryContext? discoveryContext)
    {
        var configured = ReadTestSourceAssemblySetting(discoveryContext?.RunSettings?.SettingsXml);
        return ResolveAssemblyPath(source, configured);
    }

    internal static string? ResolveTestSourceAssembly(string source, IRunContext? runContext)
    {
        var configured = ReadTestSourceAssemblySetting(runContext?.RunSettings?.SettingsXml);
        return ResolveAssemblyPath(source, configured);
    }

    private static string? ResolveAssemblyPath(string source, string? configured)
    {
        if (string.IsNullOrEmpty(configured))
        {
            return null;
        }

        if (Path.IsPathRooted(configured) && File.Exists(configured))
        {
            return configured;
        }

        var sourceDir = Path.GetDirectoryName(source) ?? string.Empty;
        var sibling = Path.GetFullPath(Path.Combine(sourceDir, configured));
        if (File.Exists(sibling))
        {
            return sibling;
        }

        return File.Exists(configured) ? Path.GetFullPath(configured) : null;
    }

    private static string? ReadTestSourceAssemblySetting(string? settingsXml)
    {
        if (string.IsNullOrEmpty(settingsXml))
        {
            return null;
        }

        try
        {
            var doc = new XmlDocument();
            doc.LoadXml(settingsXml);
            var node = doc.SelectSingleNode(
                $"//RunSettings/{Constants.SettingsName}/{Constants.TestSourceAssemblyStr}");
            var text = node?.InnerText?.Trim();
            return string.IsNullOrEmpty(text) ? null : text;
        }
        catch
        {
            return null;
        }
    }
}
