using System.Xml.Serialization;

namespace Julius.Elements
{
    [XmlRoot("INPUTPARAM")]
    public struct InputparamElement
    {
        [XmlAttribute("FRAMES")] public string Frames { get; set; }
        [XmlAttribute("MSEC")] public string MSec { get; set; }
    }
}