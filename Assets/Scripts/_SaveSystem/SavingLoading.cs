﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Score;
using UnityEngine;

namespace _SaveSystem
{
    [Serializable]
    public class SavingLoading : MonoBehaviour
    {
        private string SavePath(int _saveNumber) => $"{Application.persistentDataPath}/TakTik_Ele_Save{_saveNumber}.txt";

        [ContextMenu("save")]
        public void Save()
        {
            int _saveNumber = PersistentData.Instance.saveNumber;
            Dictionary<string, object> _state = LoadFile(_saveNumber);
            CaptureState(_state);
            SaveFile(_state, _saveNumber);
        }

        [ContextMenu("load")]
        public void Load(int _saveNumber)
        {
            Dictionary<string, object> _state = LoadFile(_saveNumber);
            RestoreState(_state);
        }

        private Dictionary<string, object> LoadFile(int _saveNumber)
        {
            if (!File.Exists(SavePath(_saveNumber)))
            {
                return new Dictionary<string, object>();
            }

            using (FileStream _stream = File.Open(SavePath(_saveNumber), FileMode.Open))
            {
                BinaryFormatter _formatter = new BinaryFormatter();
                return (Dictionary<string, object>)_formatter.Deserialize(_stream);
            }
        }

        private void SaveFile(object _state, int _saveNumber)
        {
            using (FileStream _stream = File.Open(SavePath(_saveNumber), FileMode.Create))
            {
                BinaryFormatter _formatter = new BinaryFormatter();
                _formatter.Serialize(_stream, _state);
            }
        }

        private void CaptureState(Dictionary<string, object> _state)
        {
            _state[PersistentData.Instance.Id + PersistentData.Instance.saveNumber] = PersistentData.Instance.CaptureState();
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
            if (_state.TryGetValue(PersistentData.Instance.Id, out  object _persistentData))
            {
                PersistentData.Instance.RestoreState(_persistentData);
            }
            
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