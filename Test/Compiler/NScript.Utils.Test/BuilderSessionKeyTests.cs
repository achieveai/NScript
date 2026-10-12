namespace NScript.Utils.Test
{
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NScript.Lib;
    using NScript.Utils;

    /// <summary>
    /// A dev-mode session is reused only while <see cref="BuilderSessions.OptionsKey"/> is the
    /// same, so every option that could change resolution or output must change the key.
    /// </summary>
    [TestClass]
    public class BuilderSessionKeyTests
    {
        private static readonly string Dll = typeof(BuilderSessionKeyTests).Assembly.Location;

        private static readonly string Dir = Path.GetDirectoryName(Dll);

        [TestInitialize]
        public void Setup()
        {
            Logger.Instance = new Logger();
        }

        private static ParseOptions Parse(params string[] extra)
        {
            var args = new System.Collections.Generic.List<string>
            {
                "-outJs", "app.js", "-entryAssembly", Dll, "-references", Dll, "-devMode",
            };
            args.AddRange(extra);
            var options = ParseOptions.ParseArgs(args.ToArray());
            Assert.IsNotNull(options, "Arguments must parse: " + string.Join(" ", args));
            return options;
        }

        [TestMethod]
        public void SameOptionsGiveTheSameKey()
        {
            Assert.AreEqual(BuilderSessions.OptionsKey(Parse()), BuilderSessions.OptionsKey(Parse()));
        }

        [TestMethod]
        public void PluginConfigAndPluginHintPathChangeTheKey()
        {
            var plain = BuilderSessions.OptionsKey(Parse());

            Assert.AreNotEqual(plain, BuilderSessions.OptionsKey(Parse("-pluginConfig", "a.xml")));
            Assert.AreNotEqual(
                BuilderSessions.OptionsKey(Parse("-pluginConfig", "a.xml")),
                BuilderSessions.OptionsKey(Parse("-pluginConfig", "b.xml")));
            Assert.AreNotEqual(plain, BuilderSessions.OptionsKey(Parse("-pluginHintPath", Dir)));
            Assert.AreNotEqual(
                BuilderSessions.OptionsKey(Parse("-pluginHintPath", Dir)),
                BuilderSessions.OptionsKey(Parse("-pluginHintPath", Path.GetDirectoryName(Dir))));
        }

        [TestMethod]
        public void ReferenceHintPathIsKeyedOnceParsed()
        {
            // ParseArgs drops -referenceHintPath today (its value branch is commented out), so
            // it cannot change resolution. The key still covers ReferencePath for when it is wired.
            Assert.AreEqual(0, Parse("-referenceHintPath", Dir).ReferencePath.Count);

            var withHint = Parse();
            withHint.ReferencePath.Add(Dir);
            var otherHint = Parse();
            otherHint.ReferencePath.Add(Path.GetDirectoryName(Dir));

            Assert.AreNotEqual(BuilderSessions.OptionsKey(Parse()), BuilderSessions.OptionsKey(withHint));
            Assert.AreNotEqual(BuilderSessions.OptionsKey(withHint), BuilderSessions.OptionsKey(otherHint));
        }
    }
}
