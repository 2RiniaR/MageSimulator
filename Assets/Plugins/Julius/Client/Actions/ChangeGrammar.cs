using System;
using Cysharp.Threading.Tasks;
using Julius.Elements;
using Julius.Helpers;

namespace Julius.Client.Actions
{
    public class ChangeGrammarAction : JuliusAction
    {
        private const string Command = "CHANGEGRAM";
        private const string DfaSuffix = "DFAEND";
        private const string DicSuffix = "DICEND";

        public ChangeGrammarAction(JuliusClient client) : base(client)
        {
        }

        public async UniTask Run(string name, Grammar grammar)
        {
            if (!grammar.IsLoaded)
                grammar.Load();

            var receiveGrammarTask =
                Client.OnElementReceived.GetXmlElementAsync<GrammarElement>(
                    x => x.Status == GrammarElement.ReceivedStatus, TimeoutTime);
            var receiveGraminfoTask = Client.OnElementReceived.GetXmlElementAsync<GraminfoElement>(TimeoutTime);

            var message = $"{Command} {name}\n{grammar.Dfa}\n{DfaSuffix}\n{grammar.Dict}\n{DicSuffix}\n";
            await Client.Socket.SendAsync(message);

            await receiveGrammarTask;
            await receiveGraminfoTask;
        }
    }
}