using System;
using System.Collections.Generic;
using UnityEngine;

namespace _SaveSystem
{
    [Serializable]
    public class SavableEntity : MonoBehaviour
    {
        [SerializeField] private string id = string.Empty;

        public string Id => id;

        [ContextMenu("Generate ID")]
        
        public void GenerateIId() => id = Guid.NewGuid().ToString();
        
        
        public object CaptureState()
        {
            Dictionary<string, object> _state = new Dictionary<string, object>();

            foreach (ISaveable _saveable in GetComponents<ISaveable>())
            {
                _state[_saveable.GetType().ToString()] = _saveable.CaptureState();
            }
            return _state;
        }
        
        
        public void RestoreState(object _state)
        {
            Dictionary<string, object> _stateDictionary = (Dictionary<string, object>)_state;

            foreach (ISaveable _saveable in GetComponents<ISaveable>())
            {
                string _typeName = _saveable.GetType().ToString();

                if (_stateDictionary.TryGetValue(_typeName, out object _value))
                {
                    _saveable.RestoreState(_value);
                }
            }
        }
    }
}