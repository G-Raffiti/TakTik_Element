using System;
using System.Collections.Generic;
using _Instances;
using _ScriptableObject;
using NUnit.Framework;
using Skills;
using Skills._Zone;
using Skills.ScriptableObject_Effect;
using Skills.ScriptableObject_GridEffect;
using Skills.ScriptableObject_RelicEffect;
using Stats;
using UnityEditor;
using UnityEngine;

namespace Editor.Data
{
    public static class SOGenerator
    {
        [MenuItem("Data to Object/Generate Skills from CSV")]
        public static void GenerateSkills()
        {
            if (GameObject.Find("DataBaseAndSaveSystem") == null)
            {
                GameObject DataBaseAndSaveSystem = new GameObject();
                GameObject instance = GameObject.Instantiate(DataBaseAndSaveSystem);
                instance.AddComponent<DataBase>();
            }
            GameObject.Find("DataBaseAndSaveSystem").GetComponent<DataBase>().InstantiateDataBases();
            
            if (Selection.activeObject == null) return;
            string path = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(),
                AssetDatabase.GetAssetPath(Selection.activeObject));

            if (!IsCSVFile(path))
            {
                Debug.LogError("you need to select a valid CSV File");
                return;
            }

            List<Dictionary<string, object>> rawData = CSVReader.Read(path);

            if (rawData.Count <= 0)
            {
                Debug.LogError("you need to select a valid CSV File");
                return;
            }

            bool confirmed = EditorUtility.DisplayDialog("Creation of the Skills as Scriptable Objects",
                $"Are you sure you want to create {rawData.Count} Skills ?", "Yes", "No");
            if (confirmed)
            {
                PerformGeneration(rawData);
            }
        }

        private static void PerformGeneration(List<Dictionary<string, object>> csvData)
        {
            for (int i = 0; i < csvData.Count; i++)
            {
                Dictionary<string, object> _potentialSkill = csvData[i];
                _potentialSkill.TryGetValue("Skill", out object value);
                if ( value == null || value.ToString() != "Skill") continue;
                CreateScriptableObjectSkill(_potentialSkill);
            }
            
        }

        private static void CreateScriptableObjectSkill(Dictionary<string, object> csvSkill)
        {
            SkillSO newSkill = ScriptableObject.CreateInstance<SkillSO>();
            rawSkill rawSkill = new rawSkill(csvSkill);
            newSkill.SetDATA(rawSkill);
            
            AssetDatabase.CreateAsset(newSkill, $"Assets/testCreation/Skill_{rawSkill.Element.Type}_{rawSkill.Name}.asset");
            AssetDatabase.SaveAssets();
        }

        private static bool IsCSVFile(string path)
        {
            return path.ToLower().EndsWith(".csv");
        }

        
        
    }
}