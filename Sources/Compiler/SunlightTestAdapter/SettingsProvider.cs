namespace SunlightTestAdapter;

using System.Xml;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

[SettingsName(Constants.SettingsName)]
public class SettingsProvider : ISettingsProvider
{
    public Settings Settings { get; private set; } = new Settings();

    public void Load(XmlReader reader)
    {
        var xml = new XmlDocument();
        xml.Load(reader);

        Settings = new Settings
        {
            JsFilePath = ReadFirstText(xml, Constants.JsFilePathStr),
            TestSourceAssembly = ReadFirstText(xml, Constants.TestSourceAssemblyStr),
            LogEndpoint = ReadFirstText(xml, Constants.LogEndpointStr),
        };
    }

    private static string? ReadFirstText(XmlDocument xml, string element)
    {
        var xpath = $"//RunSettings/{Constants.SettingsName}/{element}";
        var nodes = xml.SelectNodes(xpath);
        if (nodes == null || nodes.Count == 0)
        {
            return null;
        }

        var text = nodes[0]?.InnerText?.Trim();
        return string.IsNullOrEmpty(text) ? null : text;
    }
}
