using System.Xml.Serialization;

namespace Julius.Elements
{
    [XmlRoot("INPUT")]
    public struct InputElement
    {
        public const string StartRecordStatus = "STARTREC";
        public const string EndRecordStatus = "ENDREC";
        public const string ListenStatus = "LISTEN";

        [XmlAttribute("STATUS")] public string Status { get; set; }
        [XmlAttribute("TIME")] public string Time { get; set; }
    }
}