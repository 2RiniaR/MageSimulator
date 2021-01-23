using System.Xml.Serialization;

namespace Julius.Elements
{
    public struct RejectedElement
    {
        [XmlAttribute("REASON")] public string Reason { get; set; }
    }
}