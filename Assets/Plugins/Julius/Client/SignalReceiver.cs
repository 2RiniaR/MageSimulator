using System;
using UniRx;

namespace Julius.Client
{
    public class SignalReceiver : IDisposable
    {
        public Subject<string> OnElementReceived => _onElementReceived;

        private readonly IDisposable _subscribe;
        private readonly Subject<string> _onElementReceived = new Subject<string>();
        private string _buffer = "";
        private const string Splitter = ".\n";

        public SignalReceiver(IObservable<string> source)
        {
            _subscribe = source.Subscribe(OnSignalReceived);
        }

        private void OnSignalReceived(string signal)
        {
            _buffer += signal;
            while (true)
            {
                var index = _buffer.IndexOf(Splitter, StringComparison.Ordinal);
                if (index < 0) break;
                _onElementReceived.OnNext(_buffer.Substring(0, index));

                var nextElementStartIndex = index + Splitter.Length;
                _buffer = _buffer.Length <= nextElementStartIndex ? "" : _buffer.Substring(nextElementStartIndex);
            }
        }

        public void Dispose()
        {
            _subscribe?.Dispose();
            _onElementReceived?.Dispose();
        }
    }
}