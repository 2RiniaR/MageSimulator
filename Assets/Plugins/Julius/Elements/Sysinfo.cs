using System.Xml.Serialization;

namespace Julius.Elements
{
    [XmlRoot("SYSINFO")]
    public struct SysinfoElement
    {
        [XmlAttribute("PROCESS")] public string Process { get; set; }
    }
}