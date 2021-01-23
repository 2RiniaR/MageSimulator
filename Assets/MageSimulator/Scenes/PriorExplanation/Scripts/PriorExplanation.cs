using MageSimulator.Config.Scripts;
using MageSimulator.Interactive.Scripts;
using UnityEngine;

namespace MageSimulator.Scenes.PriorExplanation.Scripts
{
    public class PriorExplanation : MonoBehaviour
    {
        public ItemSelectPage confirmAllowRecordPage;
        public Animator thanksForCooperatePageAnimator;
        public string allowRecordingAnimatorParameterName;
        private ApplicationSettings _applicationSettings;

        private void Start()
        {
            _applicationSettings = Resources.Load<ApplicationSettings>("ApplicationSettings");
            if (confirmAllowRecordPage != null)
                confirmAllowRecordPage.onMoveToNext.AddListener(OnSubmitConfirmAllowRecord);
        }

        private void OnSubmitConfirmAllowRecord()
        {
            if (confirmAllowRecordPage == null)
                return;

            var allowRecording = confirmAllowRecordPage.selectingItem == 0;
            if (_applicationSettings != null)
            {
                _applicationSettings.allowRecording = allowRecording;
            }

            if (allowRecordingAnimatorParameterName != null)
            {
                thanksForCooperatePageAnimator.SetBool(allowRecordingAnimatorParameterName, allowRecording);
            }
        }
    }
}
