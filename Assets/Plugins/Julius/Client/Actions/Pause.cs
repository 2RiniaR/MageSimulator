using System;
using Cysharp.Threading.Tasks;

namespace Julius.Client.Actions
{
    public class PauseAction : JuliusAction
    {
        private const string Command = "PAUSE";

        public PauseAction(JuliusClient client) : base(client)
        {
        }

        public async UniTask Run()
        {
            await Client.Socket.SendAsync($"{Command}\n");
            Console.WriteLine("<<< " + Command);
        }
    }
}