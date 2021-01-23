using System.Xml.Serialization;

namespace Julius.Elements
{
    public struct WhypoElement
    {
        [XmlAttribute("WORD")] public string Word { get; set; }
        [XmlAttribute("CLASSID")] public string ClassId { get; set; }
        [XmlAttribute("PHONE")] public string Phone { get; set; }
        [XmlAttribute("CM")] public string Cm { get; set; }
    }
}