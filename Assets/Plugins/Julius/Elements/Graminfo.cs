using System.Xml.Serialization;

namespace Julius.Elements
{
    [XmlRoot("GRAMINFO")]
    public struct GraminfoElement
    {
        [XmlElement] public string Content { get; set; }
    }
}