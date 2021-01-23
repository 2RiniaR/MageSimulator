using System;
using Cysharp.Threading.Tasks;

namespace Julius.Client.Actions
{
    public class TerminateAction : JuliusAction
    {
        private const string Command = "TERMINATE";

        public TerminateAction(JuliusClient client) : base(client)
        {
        }

        public async UniTask Run()
        {
            await Client.Socket.SendAsync($"{Command}\n");
            Console.WriteLine("<<< " + Command);
        }
    }
}