using System;

namespace Julius.Client.Actions
{
    public abstract class JuliusAction
    {
        public static readonly TimeSpan TimeoutTime = TimeSpan.FromSeconds(20000);
        protected readonly JuliusClient Client;

        protected JuliusAction(JuliusClient client)
        {
            Client = client;
        }
    }
}