using System;
using UniRx;

namespace Julius.Helpers
{
    public static class ObservableHelper
    {
        public static IObservable<T> DoOnSubscribe<T>(this IObservable<T> source, Action action)
        {
            return Observable.Defer(() =>
            {
                action();
                return source;
            });
        }
    }
}