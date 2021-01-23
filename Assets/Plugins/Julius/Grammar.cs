using System.IO;
using System.Text;

namespace Julius
{
    public class Grammar
    {
        private static readonly Encoding FileEncode = Encoding.UTF8;

        private readonly string _dfaFilePath;
        private readonly string _dictFilePath;

        public Grammar(string dfaFilePath, string dictFilePath)
        {
            _dfaFilePath = dfaFilePath;
            _dictFilePath = dictFilePath;
        }

        public string Dfa { get; private set; }
        public string Dict { get; private set; }
        public bool IsLoaded { get; private set; }

        public void Load()
        {
            using (var reader = new StreamReader(_dfaFilePath, FileEncode))
            {
                Dfa = reader.ReadToEnd();
            }

            using (var reader = new StreamReader(_dictFilePath, FileEncode))
            {
                Dict = reader.ReadToEnd();
            }

            IsLoaded = true;
        }
    }
}