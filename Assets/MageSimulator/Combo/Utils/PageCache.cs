using System.Collections.Generic;
using UnityEngine;

namespace MageSimulator.Combo.Utils
{
    public class PageCache : MonoBehaviour
    {
        private readonly Dictionary<string, string> _cache = new Dictionary<string, string>();
        public string linkName;

        public void SetValue(string key, string value)
        {
            if (_cache.ContainsKey(key))
                _cache[key] = value;
            else
                _cache.Add(key, value);
        }

        public string GetValue(string key)
        {
            return _cache.ContainsKey(key) ? _cache[key] : null;
        }

        public void RemoveValue(string key)
        {
            if (!_cache.ContainsKey(key)) return;
            _cache.Remove(key);
        }
    }
}