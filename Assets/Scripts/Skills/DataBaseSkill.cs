using System.Collections.Generic;
using System.ComponentModel;
using _ScriptableObject;
using Skills.ScriptableObject_Effect;
using Skills.ScriptableObject_GridEffect;
using Stats;
using Units;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Skills
{
    [CreateAssetMenu(fileName = "DataBase_Skill", menuName = "Scriptable Object/DataBase/Skill")]
    public class DataBaseSkill : ScriptableObject
    {
        [SerializeField] private List<SkillSO> allSkills;
        [SerializeField] private List<Element> elements;
        public List<SkillSO> AllSkills => allSkills;

        public SkillSO GetRandom()
        {
            return AllSkills[Random.Range(0, AllSkills.Count)];
        }

        public Element GetElement(EElement type)
        {
            return elements.Find(ele => ele.Type == type);
        }

        [SerializeField] private SkillSO learning;
        [SerializeField] private SkillSO monsterAttack;
        public SkillSO Learning => learning;
        public SkillSO MonsterAttack => monsterAttack;

        
        public void AddSkill(SkillSO newSkill)
        {
            #if (UNITY_EDITOR)
            allSkills.Add(newSkill);
            EditorUtility.SetDirty(this); 
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            #endif
        }
    }
}