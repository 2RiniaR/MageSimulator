using System;
using System.Net.Sockets;
using UniRx;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Julius.Infrastructure
{
    public class SocketClient : IDisposable
    {
        private const int BufferSize = 2048;
        private byte[] _buffer = new byte[BufferSize];
        private TcpClient _client;
        private readonly Subject<string> _onReceived = new Subject<string>();
        private NetworkStream _stream;
        public IObservable<string> OnReceived => _onReceived;
        public bool DebugOutput { get; set; } = false;

        public void Dispose()
        {
            Close();
        }

        public async UniTask Open(string host, int port)
        {
            _client = new TcpClient();
            await _client.ConnectAsync(host, port);
            _stream = _client.GetStream();

            if (_stream.CanRead)
                _stream.BeginRead(_buffer, 0, BufferSize, ReceiveCallback, null);
        }

        public void Close()
        {
            _stream.Close();
            _client.Close();
        }

        public async UniTask SendAsync(string message)
        {
            try
            {
                var bytes = Encoding.UTF8.GetBytes(message);
                await _stream.WriteAsync(bytes, 0, bytes.Length);
                if (DebugOutput)
                    Debug.Log("<<<<< SEND START <<<<<\n" + message + "\n<<<<< SEND END <<<<<\n");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            try
            {
                var bytesRead = _stream.EndRead(asyncResult);
                if (bytesRead > 0)
                {
                    var received = Encoding.UTF8.GetString(_buffer, 0, bytesRead);
                    if (DebugOutput)
                        Debug.Log(">>>>> RECEIVED START >>>>>\n" + received + "\n>>>>> RECEIVED END >>>>>\n");
                    _onReceived.OnNext(received);
                    _buffer = new byte[BufferSize];
                    _stream.BeginRead(_buffer, 0, BufferSize, ReceiveCallback, null);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}