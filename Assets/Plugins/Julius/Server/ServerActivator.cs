using System;
using System.Diagnostics;
using System.IO;

namespace Julius.Server
{
    public class ServerActivator : IDisposable
    {
        public static readonly ServerSettings DefaultSettings = new ServerSettings()
        {
            JuliusPath = "./julius.exe",
            JconfPath = "./main.jconf",
            Port = 10500,
            DebugShellExecute = false
        };

        public ServerSettings Settings = DefaultSettings;
        private Process _process;

        public void StartServer()
        {
            _process = new Process();

            // ディレクトリの取得
            var directoryInfo = new DirectoryInfo(Settings.JconfPath).Parent;
            if (directoryInfo == null)
            {
                throw new DirectoryNotFoundException("Julius実行バイナリの親ディレクトリが存在しません。");
            }
            var directoryPath = directoryInfo.FullName;

            _process.StartInfo.WorkingDirectory = directoryPath;
            _process.StartInfo.FileName = Settings.JuliusPath;
            _process.StartInfo.CreateNoWindow = !Settings.DebugShellExecute;
            _process.StartInfo.UseShellExecute = Settings.DebugShellExecute;
            _process.StartInfo.Arguments = GetExecuteArguments(Settings.JconfPath, Settings.Port);

            _process.Start();
        }

        private static string GetExecuteArguments(string jconfPath, int port)
        {
            return $"-module {port.ToString()} -C {jconfPath} -input mic";
        }

        public void StopServer()
        {
            _process.Kill();
        }

        public void Dispose()
        {
            StopServer();
            _process?.Dispose();
        }
    }
}