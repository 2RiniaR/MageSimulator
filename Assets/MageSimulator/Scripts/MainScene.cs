using System;
using System.Collections;
using MageSimulator.Scripts.Talk;
using UnityEngine;

namespace MageSimulator.Scripts
{
    public class MainScene : MonoBehaviour
    {
        [Serializable]
        public struct SceneAnimator
        {
            public Animator animator;
            public string onBattleStartedTriggerName;
            public string onBattle1FinishedTriggerName;
            public string onBattle2FinishedTriggerName;
        }

        [Serializable]
        public struct Talk
        {
            public TalkSection beforeStartBattle;
            public TalkSection beforeBattle1;
            public TalkSection afterBattle1;
            public TalkSection beforeBattle2;
            public TalkSection afterBattle2;
            public TalkSection beforeBattle3;
            public TalkSection afterBattle3;
        }

        public SceneAnimator sceneAnimator;
        public TalkDisplay talkDisplay;
        public Talk talk;

        private void Start()
        {
            // StartCoroutine(Flow());
        }

        private IEnumerator Flow()
        {
            // 会話
            yield return talkDisplay.RunTalkSection(talk.beforeStartBattle);

            // ステートをBattle1へ変更
            sceneAnimator.animator.SetTrigger(sceneAnimator.onBattleStartedTriggerName);

            // 会話
            yield return talkDisplay.RunTalkSection(talk.beforeBattle1);
            // 音声入力
            // 演出
            // 会話
            yield return talkDisplay.RunTalkSection(talk.afterBattle1);
            // ステートをBattle2へ変更
            sceneAnimator.animator.SetTrigger(sceneAnimator.onBattle1FinishedTriggerName);

            // 会話
            yield return talkDisplay.RunTalkSection(talk.beforeBattle2);
            // 音声入力
            // 演出
            // 会話
            yield return talkDisplay.RunTalkSection(talk.afterBattle2);

            // ステートをBattle3へ変更
            sceneAnimator.animator.SetTrigger(sceneAnimator.onBattle2FinishedTriggerName);

            // 会話
            yield return talkDisplay.RunTalkSection(talk.beforeBattle3);
            // 音声入力
            // 演出
            // 会話
            yield return talkDisplay.RunTalkSection(talk.afterBattle3);
        }
    }
}
