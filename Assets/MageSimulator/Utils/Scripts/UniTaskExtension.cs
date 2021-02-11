using System.Threading;
using Cysharp.Threading.Tasks;

namespace MageSimulator.Utils.Scripts
{
    public static class UniTaskExtension
    {
        private static readonly CancellationToken CancelledToken = new CancellationToken(true);

        public static CancellationToken ToCancellationToken(this UniTask task)
        {
            if (task.Status.IsCompleted())
            {
                return CancelledToken;
            }

            var source = new CancellationTokenSource();
            Fire(source, task).Forget();
            return source.Token;
        }

        private static async UniTaskVoid Fire(CancellationTokenSource source, UniTask task)
        {
            try
            {
                await task;
            }
            catch
            {
                // ignored
            }

            source.Cancel();
        }
    }
}