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
    public static class SoGenerator
    {
        [MenuItem("Data to Object/Generate Skills from CSV")]
        public static void GenerateSkills()
        {
            InstantiateDataBase();
            
            if (Selection.activeObject == null) return;
            string _path = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(),
                AssetDatabase.GetAssetPath(Selection.activeObject));

            if (!IsCsvFile(_path))
            {
                Debug.LogError("you need to select a valid CSV File");
                return;
            }

            List<Dictionary<string, object>> _rawData = CsvReader.Read(_path);

            if (_rawData.Count <= 0)
            {
                Debug.LogError("you need to select a valid CSV File");
                return;
            }

            bool _warning = EditorUtility.DisplayDialog("You Need to Create the Monsters First !",
                $"Did you allready create the Monsters ?", "Yes", "No");
            if(!_warning) return;
            
            bool _clear = EditorUtility.DisplayDialog("Creation of the Skills as Scriptable Objects",
                $"do you want to erase all Skills from the DataBase and replace them with the new Ones ? \n (this will not delete the Scriptable Objects)", "Yes", "No");
            if (_clear)
            {
                DataBase.Skill.ClearDataBase();
            }
            
            bool _confirmed = EditorUtility.DisplayDialog("Creation of the Skills as Scriptable Objects",
                $"Are you sure you want to create {_rawData.Count} Skills ?", "Yes", "No");
            if (_confirmed)
            {
                PerformGenerationSkill(_rawData);
            }
        }

        [MenuItem("Data to Object/Generate Gears from CSV")]
        public static void GenerateGears()
        {
            InstantiateDataBase();
            
            if (Selection.activeObject == null) return;
            string _path = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(),
                AssetDatabase.GetAssetPath(Selection.activeObject));

            if (!IsCsvFile(_path))
            {
                Debug.LogError("you need to select a valid CSV File");
                return;
            }

            List<Dictionary<string, object>> _rawData = CsvReader.Read(_path);

            if (_rawData.Count <= 0)
            {
                Debug.LogError("you need to select a valid CSV File");
                return;
            }
            
            bool _clear = EditorUtility.DisplayDialog("Creation of the Skills as Scriptable Objects",
                $"do you want to erase all Gears from the DataBase and replace them with the new Ones ? \n (this will not delete the Scriptable Objects)" , "Yes", "No");
            if (_clear)
            {
                DataBase.Gear.ClearDataBase();
            }
            
            bool _confirmed = EditorUtility.DisplayDialog("Creation of the Skills as Scriptable Objects",
                $"Are you sure you want to create {_rawData.Count} Gears ?", "Yes", "No");
            if (_confirmed)
            {
                PerformGenerationGear(_rawData);
            }
        }
        
        [MenuItem("Data to Object/Generate Monsters from CSV")]
        public static void GenerateMonsters()
        {
            InstantiateDataBase();
            
            if (Selection.activeObject == null) return;
            string _path = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(),
                AssetDatabase.GetAssetPath(Selection.activeObject));

            if (!IsCsvFile(_path))
            {
                Debug.LogError("you need to select a valid CSV File");
                return;
            }

            List<Dictionary<string, object>> _rawData = CsvReader.Read(_path);

            if (_rawData.Count <= 0)
            {
                Debug.LogError("you need to select a valid CSV File");
                return;
            }
            
            bool _clear = EditorUtility.DisplayDialog("Creation of the Monsters as Scriptable Objects",
                $"do you want to erase all Monsters from the DataBase and replace them with the new Ones ? \n (this will not delete the Scriptable Objects)" , "Yes", "No");
            if (_clear)
            {
                DataBase.Monster.ClearDataBase();
            }
            
            bool _confirmed = EditorUtility.DisplayDialog("Creation of the Monsters as Scriptable Objects",
                $"Are you sure you want to create {_rawData.Count} Monsters ?", "Yes", "No");
            if (_confirmed)
            {
                PerformGenerationMonster(_rawData);
            }
        }
        
        private static void InstantiateDataBase()
        {
            DataBase _data = ScriptableObject.CreateInstance<DataBase>();
            _data.InstantiateDataBases();
        }

        private static void PerformGenerationSkill(List<Dictionary<string, object>> _csvData)
        {
            for (int _i = 0; _i < _csvData.Count; _i++)
            {
                Dictionary<string, object> _potentialSkill = _csvData[_i];
                _potentialSkill.TryGetValue("Skill", out object _value);
                if ( _value == null || _value.ToString() != "Skill") continue;
                CreateScriptableObjectSkill(_potentialSkill);
            }  
        }
        
        private static void PerformGenerationGear(List<Dictionary<string, object>> _csvData)
        {
            for (int _i = 0; _i < _csvData.Count; _i++)
            {
                Dictionary<string, object> _potentialSkill = _csvData[_i];
                _potentialSkill.TryGetValue("Gear", out object _value);
                if ( _value == null || _value.ToString() != "Gear") continue;
                CreateScriptableObjectGear(_potentialSkill);
            }  
        }
        
        private static void PerformGenerationMonster(List<Dictionary<string, object>> _csvData)
        {
            for (int _i = 0; _i < _csvData.Count; _i++)
            {
                Dictionary<string, object> _potentialMonster = _csvData[_i];
                _potentialMonster.TryGetValue("Monster", out object _value);
                if ( _value == null || _value.ToString() != "Monster") continue;
                CreateScriptableObjectMonster(_potentialMonster);
            }  
        }

        private static void CreateScriptableObjectSkill(Dictionary<string, object> _csvSkill)
        {
            SkillSo _newSkill = ScriptableObject.CreateInstance<SkillSo>();
            RawSkill _rawSkill = new RawSkill(_csvSkill);
            _newSkill.SetData(_rawSkill);
            
            if (DataBase.Skill.AllSkills.Find(_skill => _skill.Name == _newSkill.Name)) return;
            
            AssetDatabase.CreateAsset(_newSkill, $"Assets/Resources/ScriptableObject/Skills/Skill_{_rawSkill.Element.Type}_{_rawSkill.Name}.asset");
            AssetDatabase.SaveAssets();

            DataBase.Skill.AddSkill(
                UnityEngine.Resources.Load<SkillSo>(
                    $"ScriptableObject/Skills/Skill_{_rawSkill.Element.Type}_{_rawSkill.Name}"));
        }
        
        private static void CreateScriptableObjectGear(Dictionary<string, object> _csvGear)
        {
            GearSo _newGear = ScriptableObject.CreateInstance<GearSo>();
            RawGear _rawGear = new RawGear(_csvGear);
            _newGear.SetData(_rawGear);
            
            if (DataBase.Gear.Gears.Find(_gear => _gear.Name == _newGear.Name)) return;
            
            AssetDatabase.CreateAsset(_newGear, $"Assets/Resources/ScriptableObject/Gears/Gear_{_rawGear.Type}_{_rawGear.Rarity.Name}_{_rawGear.Name}.asset");
            AssetDatabase.SaveAssets();

            DataBase.Gear.AddGear(
                UnityEngine.Resources.Load<GearSo>(
                    $"ScriptableObject/Gears/Gear_{_rawGear.Type}_{_rawGear.Rarity.Name}_{_rawGear.Name}"));
        }
        
        private static void CreateScriptableObjectMonster(Dictionary<string, object> _csvMonster)
        {
            MonsterSo _newMonster = ScriptableObject.CreateInstance<MonsterSo>();
            RawMonster _rawMonster = new RawMonster(_csvMonster);
            _newMonster.SetData(_rawMonster);
            
            if (DataBase.Monster.Monsters.Count != 0)
                if (DataBase.Monster.Monsters.Find(_monster => _monster.Name == _newMonster.Name)) return;
            
            AssetDatabase.CreateAsset(_newMonster, $"Assets/Resources/ScriptableObject/Monsters/{_rawMonster.Type}_{_newMonster.Archetype.Type}_{_newMonster.Element.Type}_{_rawMonster.UnitName}.asset");
            AssetDatabase.SaveAssets();

            DataBase.Monster.AddMonster(
                UnityEngine.Resources.Load<MonsterSo>(
                    $"ScriptableObject/Monsters/{_rawMonster.Type}_{_newMonster.Archetype.Type}_{_newMonster.Element.Type}_{_rawMonster.UnitName}"));
        }
        
        private static bool IsCsvFile(string _path)
        {
            return _path.ToLower().EndsWith(".csv");
        }
    }
}