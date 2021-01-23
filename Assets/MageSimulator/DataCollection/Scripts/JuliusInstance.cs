using Cysharp.Threading.Tasks;
using Julius.Client;
using Julius.Server;
using UnityEngine;

namespace MageSimulator.DataCollection.Scripts
{
    [CreateAssetMenu(fileName = "New Julius Instance", menuName = "Julius/Instance", order = 0)]
    public class JuliusInstance : ScriptableObject
    {
        public string juliusPath;
        public string jconfPath;
        public int port;
        public JuliusClient Client { get; private set; }
        private ServerActivator _server;

        public void Setup()
        {
            Client = new JuliusClient();
            Client.Settings = new ClientSettings
            {
                Host = "127.0.0.1",
                Port = port,
                DebugOutput = false
            };

            _server = new ServerActivator();
            _server.Settings = new ServerSettings
            {
                JuliusPath = juliusPath,
                JconfPath = jconfPath,
                Port = port,
                DebugShellExecute = true
            };

            _server.StartServer();
            Client.StartAsync().Forget();
        }

        public void Stop()
        {
            Client?.Dispose();
            _server?.Dispose();
        }
    }
}