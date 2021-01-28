using Cysharp.Threading.Tasks;

namespace MageSimulator.BrowserUI
{
    public interface IBrowserInitializer
    {
        void InitializeBrowser(Browser browser);
    }
}