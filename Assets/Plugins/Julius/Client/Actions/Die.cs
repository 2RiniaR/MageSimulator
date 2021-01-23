using System;
using Cysharp.Threading.Tasks;

namespace Julius.Client.Actions
{
    public class DieAction : JuliusAction
    {
        private const string Command = "DIE";

        public DieAction(JuliusClient client) : base(client)
        {
        }

        public async UniTask Run()
        {
            await Client.Socket.SendAsync($"{Command}\n");
            Console.WriteLine("<<< " + Command);
        }
    }
}