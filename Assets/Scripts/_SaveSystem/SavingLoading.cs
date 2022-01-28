using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace _SaveSystem
{
    [Serializable]
    public class SavingLoading : MonoBehaviour
    {
        private string SavePath => $"C:/Program Perso/Unity Project/SaveData/TakTik_Ele_Save.txt";

        [ContextMenu("save")]
        public void Save()
        {
            Dictionary<string, object> _state = LoadFile();
            CaptureState(_state);
            SaveFile(_state);
        }

        [ContextMenu("load")]
        public void Load()
        {
            Dictionary<string, object> _state = LoadFile();
            RestoreState(_state);
        }

        private Dictionary<string, object> LoadFile()
        {
            if (!File.Exists(SavePath))
            {
                return new Dictionary<string, object>();
            }

            using (FileStream _stream = File.Open(SavePath, FileMode.Open))
            {
                BinaryFormatter _formatter = new BinaryFormatter();
                return (Dictionary<string, object>)_formatter.Deserialize(_stream);
            }
        }

        private void SaveFile(object _state)
        {
            using (FileStream _stream = File.Open(SavePath, FileMode.Create))
            {
                BinaryFormatter _formatter = new BinaryFormatter();
                _formatter.Serialize(_stream, _state);
            }
        }

        private void CaptureState(Dictionary<string, object> _state)
        {
            foreach (SavableEntity _savable in FindObjectsOfType<SavableEntity>())
            {
                if (_savable.Id == String.Empty)
                {
                    _savable.GenerateIId();
                }
                _state[_savable.Id] = _savable.CaptureState();
            }
        }

        private void RestoreState(Dictionary<string, object> _state)
        {
            foreach (SavableEntity _saveable in FindObjectsOfType<SavableEntity>())
            {
                if (_state.TryGetValue(_saveable.Id, out  object _value))
                {
                    _saveable.RestoreState(_value);
                }
            }
        }
    }
}