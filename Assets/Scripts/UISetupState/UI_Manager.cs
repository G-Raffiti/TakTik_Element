using System;
using _SaveSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace UISetupState
{
    [CreateAssetMenu(fileName = "UI_Setup", menuName = "Scriptable Object/Menu/UI Setup")]
    [Serializable]
    public class UIManager : ScriptableObject, ISaveable
    {
        public static bool ActiveLifeBar;
        public static bool CompleteStats;

        private void OnEnable()
        {
            ActiveLifeBar = false;
            CompleteStats = false;
        }

        public void MenuActiveLifeBar(bool _isActive)
        {
            ActiveLifeBar = _isActive;
        }
        
        public void MenuActiveCompleteStats(bool _isActive)
        {
            CompleteStats = _isActive;
        }
        
        [Serializable]
        private class SaveUIOption
        {
            [FormerlySerializedAs("LifeBarActive")]
            public bool lifeBarActive;
            public bool saveCompleteStats;

            public SaveUIOption()
            {
                lifeBarActive = ActiveLifeBar;
                saveCompleteStats = CompleteStats;

            }
        }
        
        public object CaptureState()
        {
            return new SaveUIOption();
        }

        public void RestoreState(object _state)
        {
            SaveUIOption _save = (SaveUIOption) _state;
            ActiveLifeBar = _save.lifeBarActive;
            CompleteStats = _save.saveCompleteStats;
        }
    }
}