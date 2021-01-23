using System;

namespace Julius.Client.Events
{
    public abstract class JuliusEvent
    {
        protected readonly JuliusClient Client;

        protected JuliusEvent(JuliusClient client)
        {
            Client = client;
        }
    }
}