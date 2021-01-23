using System.Xml.Serialization;

namespace Julius.Elements
{
    public struct GmmElement
    {
        [XmlAttribute("RESULT")] public string Result { get; set; }
        [XmlAttribute("CMSCORE")] public string Cmscore { get; set; }
    }
}