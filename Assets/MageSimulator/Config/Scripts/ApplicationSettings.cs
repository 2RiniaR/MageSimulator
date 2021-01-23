using UnityEngine;

namespace MageSimulator.Config.Scripts
{
    [CreateAssetMenu(fileName = "New Application Settings", menuName = "MageSimulator/Settings")]
    public class ApplicationSettings : ScriptableObject
    {
        [Header("システム")]
        [Tooltip("プレイ中の録音を許可するか")] public bool allowRecording;

        [Header("マイク")]
        [Tooltip("使用するマイクのデバイス名")] public string audioInputDeviceName;
        [Tooltip("録音時に補正する音量の倍率")] public float audioInputGain;

        [Header("コントローラ")]
        [Tooltip("使用するコントローラの種類")] public ControllerDeviceType deviceType;
        [Tooltip("使用するWiimoteのID")] public string wiimotePid;
    }
}
