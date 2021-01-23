using System.Xml.Serialization;

namespace Julius.Elements
{
    [XmlRoot("GRAMMAR")]
    public struct GrammarElement
    {
        public const string ReceivedStatus = "RECEIVED";
        public const string ReadyStatus = "READY";
        public const string ErrorStatus = "ERROR";

        [XmlAttribute("STATUS")] public string Status { get; set; }
        [XmlAttribute("REASON")] public string Reason { get; set; }
    }
}