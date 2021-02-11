using UniRx;
using UnityEngine;

namespace MageSimulator.Global.Setting
{
    [CreateAssetMenu(fileName = "New Application Settings", menuName = "MageSimulator/Settings")]
    public class ApplicationSettings : ScriptableObject
    {
        [Header("システム")]
        [Tooltip("プレイ中の録音を許可するか")] public bool allowRecording;

        public void SetAllowRecording(bool isAllowed)
        {
            allowRecording = isAllowed;
        }

        [Header("マイク")]
        [Tooltip("使用するマイクのデバイス名")] public StringReactiveProperty audioInputDeviceName;

        [Tooltip("録音時に補正する音量の倍率")] public FloatReactiveProperty audioInputGain;

        public string recordingSaveDirectoryPath;
    }
}
