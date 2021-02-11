using MageSimulator.Global.Mic;
using UnityEngine;

namespace MageSimulator.VoiceRecognition.Performance
{
    [CreateAssetMenu(fileName = "New Performance Judge", menuName = "Voice/PerformanceJudge", order = 0)]
    public class PerformanceJudge : ScriptableObject
    {
        [Header("録音")]
        public MicRecorder recorder;

        [Range(0f, 1f)]
        public float threshold;

        public void StartJudgeRecording()
        {
            if (recorder.IsRecording) return;
            recorder.StartRecording();
        }

        public bool StopJudgeRecording()
        {
            if (!recorder.IsRecording) return false;
            recorder.StopRecording();
            return Judge(recorder.LastRecodingClip);
        }

        public void CancelJudgeRecording()
        {
            recorder.CancelRecording();
        }

        public bool Judge(AudioClip clip)
        {
            return 0.5 < threshold;
        }
    }
}