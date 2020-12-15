using System.Collections;
using TMPro;
using UnityEngine;

namespace MageSimulator.Scripts.Talk
{
    public class TalkDisplay : MonoBehaviour
    {
        public TextMeshProUGUI talkerText;
        public TextMeshProUGUI contentText;
        public float speed;
        private string _displayingContent;
        private int _displayCharCount;
        private float _elapsedTime;

        public void SetVisible(bool isVisible)
        {

        }

        public IEnumerator RunTalkSection(TalkSection section)
        {
            foreach (var content in section.contents)
            {
                Display(content);
                yield return WaitDisplayFinish();
            }
        }

        public IEnumerator WaitDisplayFinish()
        {
            while (_displayingContent.Length < _displayCharCount)
                yield return null;
        }

        public void Display(TalkContent content)
        {
            _elapsedTime = 0;
            _displayCharCount = 0;
            _displayingContent = content.content;
            talkerText.text = content.talker;
        }

        public void DisplayAll()
        {
            if (_displayingContent == null) return;
            _displayCharCount = _displayingContent.Length;
            UpdateDisplay();
        }

        private void Update()
        {
            _elapsedTime += Time.deltaTime;
            var timeDisplayCharCount = Mathf.FloorToInt(_elapsedTime / speed);
            if (timeDisplayCharCount < _displayCharCount)
                return;
            _displayCharCount = timeDisplayCharCount;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (_displayingContent == null) return;
            contentText.text = _displayingContent.Substring(0, Mathf.Min(_displayCharCount, _displayingContent.Length));
        }
    }
}