using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MageSimulator.Combo;
using TMPro;
using UniRx;
using UnityEngine;

namespace MageSimulator.Talk.Scripts
{
    [RequireComponent(typeof(Animator))]
    public class Talk : MonoBehaviour
    {
        public string isDisplayCompletedTriggerName;

        [Tooltip("話者名を表示するテキストへの参照")]
        public TextMeshProUGUI talkerText;

        [Tooltip("会話内容を表示するテキストへの参照")]
        public TextMeshProUGUI contentText;

        [Tooltip("会話の自動再生スピード（文字数/s）")]
        public float speed;

        public bool isAutoEnter;

        [Min(0f)]
        public float autoEnterTime;

        /// <summary>
        ///     現在設定中のセクション(TalkSection)がすべて表示完了しているかどうか
        /// </summary>
        public bool IsDisplayCompleted => contentText.text.Length <= contentText.maxVisibleCharacters;

        private readonly Subject<Unit> _onEnter = new Subject<Unit>();
        private Animator _animator;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private void Start()
        {
            _animator = GetComponent<Animator>();
            if (_animator == null)
                Debug.LogWarning("Animator が存在しません");
        }

        public void Enter()
        {
            _onEnter.OnNext(Unit.Default);
        }

        public async UniTask RunSection(TalkSection section, CancellationToken token = new CancellationToken())
        {
            if (_animator == null) return;
            foreach (var content in section.contents)
            {
                await DisplayContent(content, token);
                if (token.IsCancellationRequested || _cancellationTokenSource.Token.IsCancellationRequested) return;
            }
        }

        private async UniTask DisplayContent(TalkContent content, CancellationToken token = new CancellationToken())
        {
            // 初期化を行う
            await UniTask.SwitchToMainThread();
            contentText.maxVisibleCharacters = 0;
            contentText.text = content.content;
            talkerText.text = content.talker;

            _animator.SetBool(isDisplayCompletedTriggerName, false);
            var elapsedTime = 0f;
            var enterSkip = _onEnter.First().Subscribe(_ => DisplayAll());

            while (!IsDisplayCompleted)
            {
                // 表示する文字数を経過時間に合わせて更新する
                elapsedTime += Time.deltaTime;
                var timeDisplayCharCount = Mathf.FloorToInt(speed * elapsedTime);
                if (timeDisplayCharCount > contentText.maxVisibleCharacters)
                    contentText.maxVisibleCharacters = timeDisplayCharCount;

                // 1フレーム待機する
                await UniTask.Yield(PlayerLoopTiming.Update, token);
                if (token.IsCancellationRequested || _cancellationTokenSource.Token.IsCancellationRequested) return;
            }

            enterSkip.Dispose();

            if (!isAutoEnter)
                _animator.SetBool(isDisplayCompletedTriggerName, true);

            // ユーザー入力を待つ
            await UniTask.WhenAny(
                _onEnter.First().ToUniTask(cancellationToken: token),
                isAutoEnter
                    ? UniTask.Delay(TimeSpan.FromSeconds(autoEnterTime), cancellationToken: token)
                    : UniTask.Never(token)
            );
        }

        /// <summary>
        ///     すべての文字が表示された状態にする
        /// </summary>
        public void DisplayAll()
        {
            contentText.maxVisibleCharacters = contentText.text.Length;
        }
    }
}