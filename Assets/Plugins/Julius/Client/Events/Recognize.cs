using System;
using UniRx;
using System.Linq;
using Julius.Elements;
using Julius.Helpers;

namespace Julius.Client.Events
{
    public class RecognizeEvent : JuliusEvent
    {
        public RecognizeState State { get; private set; }
        public readonly IObservable<RecognizeState> OnStateChanged;
        public readonly IObservable<RecognitionResult> OnRecognizeSucceed;
        public readonly IObservable<Unit> OnRecognizeFailed;

        public RecognizeEvent(JuliusClient client) : base(client)
        {
            OnStateChanged = Observable.Merge(
                client.OnElementReceived
                    .GetXmlElementObservable<StartrecogElement>()
                    .Select(_ => RecognizeState.Active),
                client.OnElementReceived
                    .GetXmlElementObservable<EndrecogElement>()
                    .Select(_ => RecognizeState.Inactive)
            )
                .Do(x => State = x)
                .Publish()
                .RefCount();

            OnRecognizeSucceed = client.OnElementReceived
                .GetXmlElementObservable<RecogoutElement>()
                .Select(x => new RecognitionResult
                {
                    PredictedGramsId = x.Shypo.Select(y => int.Parse(y.Gram)).ToList()
                })
                .Publish()
                .RefCount();

            OnRecognizeFailed = client.OnElementReceived
                .GetXmlElementObservable<RecogfailElement>()
                .AsUnitObservable()
                .Publish()
                .RefCount();
        }
    }
}