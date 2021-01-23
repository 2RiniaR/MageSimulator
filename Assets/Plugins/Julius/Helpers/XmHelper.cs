using System;
using System.IO;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Julius.Helpers
{
    public static class XmHelper
    {
        public static IObservable<T> GetXmlElementObservable<T>(this IObservable<string> source)
        {
            var serializer = new XmlSerializer(typeof(T));
            return source
                .Select(x => XmlReader.Create(new StringReader(x)))
                .Where(serializer.CanDeserialize)
                .Select(serializer.Deserialize)
                .Cast<object, T>();
        }

        public static UniTask<T> GetXmlElementAsync<T>(this IObservable<string> source, Func<T, bool> predicate, TimeSpan timeoutTime)
        {
            return source.GetXmlElementObservable<T>()
                .Where(predicate)
                .Timeout(timeoutTime)
                .Take(1)
                .ToUniTask();
        }
        public static UniTask<T> GetXmlElementAsync<T>(this IObservable<string> source, TimeSpan timeoutTime)
        {
            return source.GetXmlElementAsync<T>(_ => true, timeoutTime);
        }
    }
}