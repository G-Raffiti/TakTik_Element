using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace TbsFramework.EditorUtils
{
    public class GridHelperUtils
    {
        public static bool CheckMissingParameters(Dictionary<string, object> parameterValues)
        {
            List<string> missingParams = new List<string>();
            foreach (KeyValuePair<string, object> entry in parameterValues)
            {
                if (entry.Value == null)
                {
                    missingParams.Add(entry.Key);
                }
            }

            if (missingParams.Count != 0)
            {
                string dialogTitle = string.Format("Parameter{0} missing", missingParams.Count > 1 ? "s" : "");

                StringBuilder dialogMessage = new StringBuilder();
                dialogMessage.AppendFormat("Please fill in the missing parameter{0} first:\n", missingParams.Count > 1 ? "s" : "");

                foreach (string missingParam in missingParams)
                {
                    dialogMessage.AppendLine(string.Format("   -{0}", missingParam));
                }

                string dialogOK = "Ok";
                EditorUtility.DisplayDialog(dialogTitle, dialogMessage.ToString(), dialogOK);
                return true;
            }
            return false;
        }
        public static void ClearScene()
        {
            GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();
            List<GameObject> toDestroy = new List<GameObject>();

            foreach (GameObject obj in objects)
            {
                bool isChild = obj.transform.parent != null;

                if (isChild)
                    continue;

                toDestroy.Add(obj);
            }
            toDestroy.ForEach(o => GameObject.DestroyImmediate(o));
        }
    }
}