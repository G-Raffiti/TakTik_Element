using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class GridHelperUtils
    {
        public static bool CheckMissingParameters(Dictionary<string, object> _parameterValues)
        {
            List<string> _missingParams = new List<string>();
            foreach (KeyValuePair<string, object> _entry in _parameterValues)
            {
                if (_entry.Value == null)
                {
                    _missingParams.Add(_entry.Key);
                }
            }

            if (_missingParams.Count != 0)
            {
                string _dialogTitle = string.Format("Parameter{0} missing", _missingParams.Count > 1 ? "s" : "");

                StringBuilder _dialogMessage = new StringBuilder();
                _dialogMessage.AppendFormat("Please fill in the missing parameter{0} first:\n", _missingParams.Count > 1 ? "s" : "");

                foreach (string _missingParam in _missingParams)
                {
                    _dialogMessage.AppendLine(string.Format("   -{0}", _missingParam));
                }

                string _dialogOk = "Ok";
                EditorUtility.DisplayDialog(_dialogTitle, _dialogMessage.ToString(), _dialogOk);
                return true;
            }
            return false;
        }
        public static void ClearScene()
        {
            GameObject[] _objects = GameObject.FindObjectsOfType<GameObject>();
            List<GameObject> _toDestroy = new List<GameObject>();

            foreach (GameObject _obj in _objects)
            {
                bool _isChild = _obj.transform.parent != null;

                if (_isChild)
                    continue;

                _toDestroy.Add(_obj);
            }
            _toDestroy.ForEach(_o => GameObject.DestroyImmediate(_o));
        }
    }
}