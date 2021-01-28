namespace MageSimulator.BrowserUI
{
    public class LinkBrowserEvent : BrowserEvent
    {
        public string Name { get; set; }
        public Page Target { get; set; }
    }
}