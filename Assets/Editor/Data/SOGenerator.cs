using System.Collections.Generic;
using _CSVFiles;
using _Instances;
using Gears;
using Skills;
using Units;
using UnityEditor;
using UnityEngine;

namespace Editor.Data
{
    public static class SOGenerator
    {
        [MenuItem("Data to Object/Generate Skills from CSV")]
        public static void GenerateSkills()
        {
            InstantiateDataBase();
            
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
            
            bool clear = EditorUtility.DisplayDialog("Creation of the Skills as Scriptable Objects",
                $"do you want to erase all Skills from the DataBase and replace them with the new Ones ? \n (this will not delete the Scriptable Objects)", "Yes", "No");
            if (clear)
            {
                DataBase.Skill.ClearDataBase();
            }
            
            bool confirmed = EditorUtility.DisplayDialog("Creation of the Skills as Scriptable Objects",
                $"Are you sure you want to create {rawData.Count} Skills ?", "Yes", "No");
            if (confirmed)
            {
                PerformGenerationSkill(rawData);
            }
        }

        [MenuItem("Data to Object/Generate Gears from CSV")]
        public static void GenerateGears()
        {
            InstantiateDataBase();
            
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
            
            bool clear = EditorUtility.DisplayDialog("Creation of the Skills as Scriptable Objects",
                $"do you want to erase all Gears from the DataBase and replace them with the new Ones ? \n (this will not delete the Scriptable Objects)" , "Yes", "No");
            if (clear)
            {
                DataBase.Gear.ClearDataBase();
            }
            
            bool confirmed = EditorUtility.DisplayDialog("Creation of the Skills as Scriptable Objects",
                $"Are you sure you want to create {rawData.Count} Gears ?", "Yes", "No");
            if (confirmed)
            {
                PerformGenerationGear(rawData);
            }
        }
        
        [MenuItem("Data to Object/Generate Monsters from CSV")]
        public static void GenerateMonsters()
        {
            InstantiateDataBase();
            
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
            
            bool clear = EditorUtility.DisplayDialog("Creation of the Monsters as Scriptable Objects",
                $"do you want to erase all Monsters from the DataBase and replace them with the new Ones ? \n (this will not delete the Scriptable Objects)" , "Yes", "No");
            if (clear)
            {
                DataBase.Monster.ClearDataBase();
            }
            
            bool confirmed = EditorUtility.DisplayDialog("Creation of the Monsters as Scriptable Objects",
                $"Are you sure you want to create {rawData.Count} Monsters ?", "Yes", "No");
            if (confirmed)
            {
                PerformGenerationMonster(rawData);
            }
        }
        
        private static void InstantiateDataBase()
        {
            if (GameObject.Find("DataBaseAndSaveSystem") == null)
            {
                GameObject DataBaseAndSaveSystem = new GameObject();
                GameObject instance = GameObject.Instantiate(DataBaseAndSaveSystem);
                instance.AddComponent<DataBase>();
            }
            GameObject.Find("DataBaseAndSaveSystem").GetComponent<DataBase>().InstantiateDataBases();
        }

        private static void PerformGenerationSkill(List<Dictionary<string, object>> csvData)
        {
            for (int i = 0; i < csvData.Count; i++)
            {
                Dictionary<string, object> _potentialSkill = csvData[i];
                _potentialSkill.TryGetValue("Skill", out object value);
                if ( value == null || value.ToString() != "Skill") continue;
                CreateScriptableObjectSkill(_potentialSkill);
            }  
        }
        
        private static void PerformGenerationGear(List<Dictionary<string, object>> csvData)
        {
            for (int i = 0; i < csvData.Count; i++)
            {
                Dictionary<string, object> _potentialSkill = csvData[i];
                _potentialSkill.TryGetValue("Gear", out object value);
                if ( value == null || value.ToString() != "Gear") continue;
                CreateScriptableObjectGear(_potentialSkill);
            }  
        }
        
        private static void PerformGenerationMonster(List<Dictionary<string, object>> csvData)
        {
            for (int i = 0; i < csvData.Count; i++)
            {
                Dictionary<string, object> _potentialMonster = csvData[i];
                _potentialMonster.TryGetValue("Monster", out object value);
                if ( value == null || value.ToString() != "Monster") continue;
                CreateScriptableObjectMonster(_potentialMonster);
            }  
        }

        private static void CreateScriptableObjectSkill(Dictionary<string, object> csvSkill)
        {
            SkillSO newSkill = ScriptableObject.CreateInstance<SkillSO>();
            RawSkill rawSkill = new RawSkill(csvSkill);
            newSkill.SetDATA(rawSkill);
            
            if (DataBase.Skill.AllSkills.Find(skill => skill.Name == newSkill.Name)) return;
            
            AssetDatabase.CreateAsset(newSkill, $"Assets/Resources/ScriptableObject/Skills/Skill_{rawSkill.Element.Type}_{rawSkill.Name}.asset");
            AssetDatabase.SaveAssets();

            DataBase.Skill.AddSkill(
                UnityEngine.Resources.Load<SkillSO>(
                    $"ScriptableObject/Skills/Skill_{rawSkill.Element.Type}_{rawSkill.Name}"));
        }
        
        private static void CreateScriptableObjectGear(Dictionary<string, object> csvGear)
        {
            GearSO newGear = ScriptableObject.CreateInstance<GearSO>();
            RawGear rawGear = new RawGear(csvGear);
            newGear.SetDATA(rawGear);
            
            if (DataBase.Gear.Gears.Find(gear => gear.Name == newGear.Name)) return;
            
            AssetDatabase.CreateAsset(newGear, $"Assets/Resources/ScriptableObject/Gears/Gear_{rawGear.Type}_{rawGear.Rarity.Name}_{rawGear.Name}.asset");
            AssetDatabase.SaveAssets();

            DataBase.Gear.AddGear(
                UnityEngine.Resources.Load<GearSO>(
                    $"ScriptableObject/Gears/Gear_{rawGear.Type}_{rawGear.Rarity.Name}_{rawGear.Name}"));
        }
        
        private static void CreateScriptableObjectMonster(Dictionary<string, object> csvMonster)
        {
            MonsterSO newMonster = ScriptableObject.CreateInstance<MonsterSO>();
            RawMonster rawMonster = new RawMonster(csvMonster);
            newMonster.SetDATA(rawMonster);
            
            if (DataBase.Monster.Monsters.Count != 0)
                if (DataBase.Monster.Monsters.Find(monster => monster.Name == newMonster.Name)) return;
            
            AssetDatabase.CreateAsset(newMonster, $"Assets/Resources/ScriptableObject/Monsters/{rawMonster.Type}_{newMonster.Archetype.Type}_{newMonster.Element.Type}_{rawMonster.UnitName}.asset");
            AssetDatabase.SaveAssets();

            DataBase.Monster.AddMonster(
                UnityEngine.Resources.Load<MonsterSO>(
                    $"ScriptableObject/Monsters/{rawMonster.Type}_{newMonster.Archetype.Type}_{newMonster.Element.Type}_{rawMonster.UnitName}"));
        }
        
        private static bool IsCSVFile(string path)
        {
            return path.ToLower().EndsWith(".csv");
        }
    }
}