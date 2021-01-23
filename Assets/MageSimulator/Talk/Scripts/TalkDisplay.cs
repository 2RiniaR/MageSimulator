using System;
using System.Collections;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MageSimulator.Talk.Scripts
{
    [RequireComponent(typeof(Animator))]
    public class TalkDisplay : MonoBehaviour
    {
        [Serializable]
        public struct StateComposite
        {
            [Header("トリガー")]
            public string isDisplayTriggerName;
            public string isDisplayCompletedTriggerName;
        }

        [Tooltip("アニメーターの状態")]
        public StateComposite states;

        [Tooltip("話者名を表示するテキストへの参照")]
        public TextMeshProUGUI talkerText;

        [Tooltip("会話内容を表示するテキストへの参照")]
        public TextMeshProUGUI contentText;

        [Tooltip("会話を送るための入力操作")]
        public InputAction enterAction;

        [Tooltip("会話の自動再生スピード（文字数/s）")]
        public float speed;

        /// <summary>
        ///     現在設定中のセクション(TalkSection)がすべて表示完了しているかどうか
        /// </summary>
        public bool IsDisplayCompleted => contentText.text.Length <= contentText.maxVisibleCharacters;

        /// <summary>
        ///     会話を送る入力操作がされているとき、trueになるフラグ
        /// </summary>
        private bool _enterFlag;

        private Animator _animator;

        private void OnEnable()
        {
            Observable.FromEvent<InputAction.CallbackContext>(
                    h => enterAction.performed += h,
                    h => enterAction.performed -= h)
                .DoOnSubscribe(enterAction.Enable)
                .DoOnCancel(enterAction.Disable)
                .Subscribe(OnEnter)
                .AddTo(this);
        }

        private void Start()
        {
            _animator = GetComponent<Animator>();
            if (_animator == null)
                Debug.LogWarning("Animator が存在しません");
        }

        private void OnEnter(InputAction.CallbackContext ctx)
        {
            if (!_animator.GetBool(states.isDisplayTriggerName)) return;

            if (IsDisplayCompleted)
            {
                _enterFlag = true;
            }
            else
            {
                DisplayAll();
                _enterFlag = false;
            }
        }

        public IEnumerator RunTalkSection(TalkSection section)
        {
            _animator.SetBool(states.isDisplayTriggerName, true);
            foreach (var content in section.contents)
                yield return DisplayContent(content);
            _animator.SetBool(states.isDisplayTriggerName, false);
        }

        /// <summary>
        ///     現在の会話内容(TalkContent)の表示完了を待機するコルーチン
        /// </summary>
        private IEnumerator DisplayContent(TalkContent content)
        {
            contentText.maxVisibleCharacters = 0;
            contentText.text = content.content;
            talkerText.text = content.talker;
            _animator.SetBool(states.isDisplayCompletedTriggerName, false);

            var elapsedTime = 0f;
            while (!IsDisplayCompleted)
            {
                elapsedTime += Time.deltaTime;
                var timeDisplayCharCount = Mathf.FloorToInt(speed * elapsedTime);
                if (timeDisplayCharCount > contentText.maxVisibleCharacters)
                    contentText.maxVisibleCharacters = timeDisplayCharCount;
                yield return null;
            }
            _animator.SetBool(states.isDisplayCompletedTriggerName, true);

            while (!_enterFlag) yield return null;
            _enterFlag = false;
        }

        /// <summary>
        ///     すべての文字が表示された状態にする
        /// </summary>
        public void DisplayAll()
        {
            contentText.maxVisibleCharacters = contentText.text.Length;
            _animator.SetBool(states.isDisplayCompletedTriggerName, true);
        }
    }
}