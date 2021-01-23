using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Julius.Client.Actions;
using Julius.Client.Events;
using Julius.Infrastructure;
using UniRx;

namespace Julius.Client
{
    public class JuliusClient : IDisposable
    {
        public static readonly ClientSettings DefaultSettings = new ClientSettings()
        {
            Host = "localhost",
            Port = 10500,
            DebugOutput = false
        };

        public ClientSettings Settings = DefaultSettings;

        public readonly ActionComposite Actions;
        public readonly EventComposite Events;
        public readonly IObservable<string> OnElementReceived;
        public readonly SocketClient Socket = new SocketClient();

        private readonly SignalReceiver _signalReceiver;

        public JuliusClient()
        {
            _signalReceiver = new SignalReceiver(Socket.OnReceived);
            OnElementReceived = _signalReceiver.OnElementReceived;

            Actions = new ActionComposite
            {
                GetStatus = new GetStatusAction(this),
                Die = new DieAction(this),
                Terminate = new TerminateAction(this),
                Pause = new PauseAction(this),
                Resume = new ResumeAction(this),
                ActivateGrammar = new ActivateGrammarAction(this),
                DeactivateGrammar = new DeactivateGrammarAction(this),
                AddGrammar = new AddGrammarAction(this),
                ChangeGrammar = new ChangeGrammarAction(this),
                DeleteGrammar = new DeleteGrammarAction(this),
                SyncGrammar = new SyncGrammarAction(this),
                Recognition = new RecognizeEvent(this)
            };

            Events = new EventComposite
            {
                Input = new InputEvent(this),
                Recognize = new RecognizeEvent(this)
            };
        }

        public async UniTask StartAsync()
        {
            Socket.DebugOutput = Settings.DebugOutput;
            await Socket.Open(Settings.Host, Settings.Port);
        }

        public void Stop()
        {
            Socket.Close();
        }

        public async UniTask SetGrammar(string name, Grammar grammar, CancellationToken token = new CancellationToken())
        {
            await Actions.Terminate.Run();
            if (token.IsCancellationRequested) return;

            await Actions.ChangeGrammar.Run(name, grammar);
            if (token.IsCancellationRequested) return;

            await Actions.ActivateGrammar.Run(name);
            if (token.IsCancellationRequested) return;

            await Actions.SyncGrammar.Run();
            if (token.IsCancellationRequested) return;

            await Actions.Resume.Run();
        }

        public async UniTask WaitRecognition(Action onReadyCallback = default,
            Action onFailedCallback = default, Action onSucceedCallback = default,
            CancellationToken token = new CancellationToken())
        {
            if (await Actions.GetStatus.Run() == JuliusServerStatus.Sleep)
                await Actions.Resume.Run();
            if (token.IsCancellationRequested) return;

            onReadyCallback?.Invoke();
            var onFinish = Events.Recognize.OnRecognizeFailed.Subscribe(_ => onFailedCallback?.Invoke());
            await Events.Recognize.OnRecognizeSucceed.Take(1).ToTask();

            onSucceedCallback?.Invoke();
            onFinish.Dispose();
        }

        public struct ActionComposite
        {
            public GetStatusAction GetStatus { get; set; }
            public DieAction Die { get; set; }
            public TerminateAction Terminate { get; set; }
            public PauseAction Pause { get; set; }
            public ResumeAction Resume { get; set; }
            public ActivateGrammarAction ActivateGrammar { get; set; }
            public DeactivateGrammarAction DeactivateGrammar { get; set; }
            public AddGrammarAction AddGrammar { get; set; }
            public ChangeGrammarAction ChangeGrammar { get; set; }
            public DeleteGrammarAction DeleteGrammar { get; set; }
            public SyncGrammarAction SyncGrammar { get; set; }

            public RecognizeEvent Recognition { get; set; }
        }

        public struct EventComposite
        {
            public InputEvent Input { get; set; }
            public RecognizeEvent Recognize { get; set; }
        }

        public void Dispose()
        {
            Stop();
            _signalReceiver.Dispose();
        }
    }
}