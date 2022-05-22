namespace SunlightTestAdapter;

using System.Collections.Generic;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using SunlightUnit;
using System.Reflection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

[DefaultExecutorUri(Constants.ExecutorUri)]
[FileExtension(".dll")]
public class Discoverer : ITestDiscoverer
{
    public void DiscoverTests(
        IEnumerable<string> sources,
        IDiscoveryContext discoveryContext,
        IMessageLogger logger,
        ITestCaseDiscoverySink discoverySink)
    {
        foreach (var source in sources)
        {
            var testCases = GetTests(source);

            logger.SendMessage(
                TestMessageLevel.Informational,
                $"Sending {testCases.Count()} test cases from source: {source}");

            foreach (var tesCase in testCases)
            {
                discoverySink.SendTestCase(tesCase);
            }
        }
    }

    public static List<TestCase> GetTests(string source)
    {
        // 1. Load assembly's methods
        // 2. Pick the ones which match [Test] attributes

        var assembly = Assembly.LoadFile(source);
        var rv = new List<TestCase>();

        foreach (var ty in assembly.GetTypes())
        {
            var isTesClass = ty.GetCustomAttributes()
                .Where(attr => attr.GetType() == typeof (TestFixtureAttribute))
                .FirstOrDefault() != null;

            if (!isTesClass)
            {
                continue;
            }

            foreach (var method in ty.GetMethods())
            {
                var isTestMethod = method.GetCustomAttributes()
                    .Where(attr => attr.GetType() == typeof (TestAttribute))
                    .FirstOrDefault() != null;

                if (!isTestMethod)
                {
                    continue;
                }

                rv.Add(new TestCase()
                {
                    ExecutorUri = new Uri(Constants.ExecutorUri),
                    DisplayName = method.Name,
                    FullyQualifiedName = ty.Name + "." + method.Name,
                    Source = source,
                });
            }
        }

        return rv;
    }
}