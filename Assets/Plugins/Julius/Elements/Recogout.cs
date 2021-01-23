using System.Collections.Generic;
using System.Xml.Serialization;

namespace Julius.Elements
{
    [XmlRoot("RECOGOUT")]
    public struct RecogoutElement
    {
        [XmlElement("SHYPO")] public List<ShypoElement> Shypo { get; set; }
    }
}