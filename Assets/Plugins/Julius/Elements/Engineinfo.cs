using System.Xml.Serialization;

namespace Julius.Elements
{
    public struct EngineinfoElement
    {
        [XmlAttribute("TYPE")] public string Type { get; set; }
        [XmlAttribute("VERSION")] public string Version { get; set; }
        [XmlAttribute("CONF")] public string Conf { get; set; }
    }
}