using Cysharp.Threading.Tasks;
using Julius.Elements;
using Julius.Helpers;

namespace Julius.Client.Actions
{
    public class SyncGrammarAction : JuliusAction
    {
        private const string Command = "SYNCGRAM";

        public SyncGrammarAction(JuliusClient client) : base(client)
        {
        }

        public async UniTask Run()
        {
            var receiveGrammarTask = Client.OnElementReceived.GetXmlElementAsync<GrammarElement>(TimeoutTime);
            await Client.Socket.SendAsync($"{Command}\n");
            await receiveGrammarTask;
        }
    }
}