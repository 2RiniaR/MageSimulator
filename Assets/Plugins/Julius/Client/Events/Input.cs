using System;
using UniRx;
using Julius.Elements;
using Julius.Helpers;

namespace Julius.Client.Events
{
    public class InputEvent : JuliusEvent
    {
        public InputState CurrentState { get; private set; }
        public IObservable<InputState> OnStateChanged;

        public InputEvent(JuliusClient client) : base(client)
        {
            OnStateChanged = client.OnElementReceived
                .GetXmlElementObservable<InputElement>()
                .Select(x =>
                {
                    InputState state;
                    switch (x.Status)
                    {
                        case InputElement.StartRecordStatus:
                            state = InputState.Recording;
                            break;
                        case InputElement.EndRecordStatus:
                            state = InputState.Closed;
                            break;
                        case InputElement.ListenStatus:
                            state = InputState.Listen;
                            break;
                        default:
                            throw new ApplicationException();
                    }

                    CurrentState = state;
                    return state;
                })
                .Publish()
                .RefCount();
        }
    }
}