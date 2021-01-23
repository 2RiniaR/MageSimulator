using System.Collections.Generic;
using System.Xml.Serialization;

namespace Julius.Elements
{
    public struct ShypoElement
    {
        [XmlElement("WHYPO")] public List<WhypoElement> Whypo { get; set; }
        [XmlAttribute("RANK")] public string Rank { get; set; }
        [XmlAttribute("SCORE")] public string Score { get; set; }
        [XmlAttribute("GRAM")] public string Gram { get; set; }
    }
}