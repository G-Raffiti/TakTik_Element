using System;
using _SaveSystem;
using UnityEngine;

namespace UISetupState
{
    [CreateAssetMenu(fileName = "UI_Setup", menuName = "Scriptable Object/Menu/UI Setup")]
    [Serializable]
    public class UI_Manager : ScriptableObject, ISaveable
    {
        public static bool ActiveLifeBar;

        private void OnEnable()
        {
            ActiveLifeBar = false;
        }

        public void MenuActiveLifeBar(bool isActive)
        {
            ActiveLifeBar = isActive;
        }
        
        [Serializable]
        private class SaveUI_Option
        {
            public bool LifeBarActive;

            public SaveUI_Option()
            {
                LifeBarActive = ActiveLifeBar;
            }
        }
        
        public object CaptureState()
        {
            return new SaveUI_Option();
        }

        public void RestoreState(object _state)
        {
            SaveUI_Option save = (SaveUI_Option) _state;
            ActiveLifeBar = save.LifeBarActive;
        }
    }
}