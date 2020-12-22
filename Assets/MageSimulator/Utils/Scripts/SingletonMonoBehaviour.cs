using UnityEngine;

namespace MageSimulator.Utils.Scripts
{
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance => _instance ? _instance : _instance = FindObjectOfType<T>();

        protected void OnDestroy()
        {
            if(_instance == this) _instance = null;
        }
    }
}