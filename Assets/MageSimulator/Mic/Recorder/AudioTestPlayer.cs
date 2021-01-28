using MageSimulator.Mic.Scripts;
using UniRx;
using UnityEngine;

namespace MageSimulator.Mic.Recorder
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioTestPlayer : MonoBehaviour
    {
        public MicRecorder recorder;
        private AudioSource _audioSource;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            recorder.OnRecordFinishObservable
                .Where(x => x != null)
                .Do(x => Debug.Log("TestPlay: " + x.length))
                .Subscribe(_audioSource.PlayOneShot)
                .AddTo(this);
        }
    }
}