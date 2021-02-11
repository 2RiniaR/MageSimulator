using MageSimulator.Combo.Components;
using MageSimulator.Global.Setting;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace MageSimulator.Global.Mic
{
    [RequireComponent(typeof(Slider))]
    public class MicGainSetter : MonoBehaviour
    {
        public ApplicationSettings settings;
        private Slider _slider;

        private void Start()
        {
            _slider = GetComponent<Slider>();
            _slider.value = settings.audioInputGain.Value;
            _slider.OnValueChangedAsObservable().Subscribe(x => settings.audioInputGain.Value = x).AddTo(this);
        }
    }
}