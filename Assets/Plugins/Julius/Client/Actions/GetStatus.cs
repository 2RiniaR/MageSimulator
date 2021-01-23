using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Julius.Elements;
using Julius.Helpers;

namespace Julius.Client.Actions
{
    public class GetStatusAction : JuliusAction
    {
        private const string Command = "STATUS";
        private const string ActiveStatus = "ACTIVE";
        private const string SleepStatus = "SLEEP";

        public GetStatusAction(JuliusClient client) : base(client)
        {
        }

        public async UniTask<JuliusServerStatus> Run()
        {
            var receiveTask = Client.OnElementReceived.GetXmlElementAsync<SysinfoElement>(TimeoutTime);

            await Client.Socket.SendAsync($"{Command}\n");
            Console.WriteLine("<<< " + Command);

            var received = await receiveTask;
            switch (received.Process)
            {
                case ActiveStatus:
                    return JuliusServerStatus.Active;
                case SleepStatus:
                    return JuliusServerStatus.Sleep;
                default:
                    throw new InvalidDataException("An invalid process status was returned.");
            }
        }
    }

    public enum JuliusServerStatus
    {
        Active,
        Sleep
    }
}