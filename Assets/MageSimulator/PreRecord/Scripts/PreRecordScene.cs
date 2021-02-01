using System;
using UnityEngine;
using UnityEngine.Animations;

namespace MageSimulator.PreRecord.Scripts
{
    public class PreRecordScene : MonoBehaviour
    {
        public ParentConstraint wallCanvas;
        public GuidReference wall;

        private void Start()
        {
            wallCanvas.AddSource(new ConstraintSource {sourceTransform = wall.gameObject.transform, weight = 1f});
        }

        // private async UniTask Talk(TalkSection section)
        // {
        //     await assets.talkDisplay.RunTalkSection(section).WithCancellation(_onExitState.Token);
        //     Next();
        // }
        //
        // private async UniTask Record(JuliusGrammar grammar, string savePath = null)
        // {
        //     var recording = await assets.speakIndicator.Record(grammar, _onExitState.Token);
        //     if (savePath != null)
        //     {
        //         WavUtility.SaveAsFile(recording, savePath);
        //         Debug.Log("Recording was saved at " + savePath);
        //     }
        //     Next();
        // }
    }
}
