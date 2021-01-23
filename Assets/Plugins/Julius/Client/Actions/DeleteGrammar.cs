using Cysharp.Threading.Tasks;

namespace Julius.Client.Actions
{
    public class DeleteGrammarAction : JuliusAction
    {
        private const string Command = "DELGRAM";

        public DeleteGrammarAction(JuliusClient client) : base(client)
        {
        }

        public async UniTask Run(string name)
        {
            await Client.Socket.SendAsync($"{Command}\n{name}\n");
        }

        public async UniTask Run(int id)
        {
            await Client.Socket.SendAsync($"{Command}\n{id}\n");
        }
    }
}