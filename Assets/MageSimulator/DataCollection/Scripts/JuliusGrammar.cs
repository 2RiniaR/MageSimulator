using System.IO;
using Julius;
using UnityEngine;

namespace MageSimulator.DataCollection.Scripts
{
    [CreateAssetMenu(fileName = "New Julius Grammar", menuName = "Julius/Grammar", order = 0)]
    public class JuliusGrammar : ScriptableObject
    {
        public string display;
        public string dfaFilePath;
        public string dictFilePath;
        public Grammar Grammar { get; private set; }
        public bool IsLoaded { get; private set; } = false;

        private void OnEnable()
        {
            Unload();
            if (File.Exists(dfaFilePath) && File.Exists(dictFilePath))
                Load();
        }

        public void Load()
        {
            Grammar = new Grammar(dfaFilePath, dictFilePath);
            Grammar.Load();
            IsLoaded = Grammar.IsLoaded;
        }

        public void Unload()
        {
            Grammar = null;
            IsLoaded = false;
        }
    }
}