using System.Collections.Generic;
using System.Linq;
using _ScriptableObject;
using Skills.ScriptableObject_Effect;
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
            if (AllSkills.Contains(newSkill)) return;
            #if (UNITY_EDITOR)
            allSkills.Add(newSkill);
            EditorUtility.SetDirty(this); 
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            #endif
        }

        public void ClearDataBase()
        {
            allSkills = new List<SkillSO>();
        }

        public SkillSO GetSkillFor(MonsterSO _monster)
        {
            List<SkillSO> ret = allSkills.Where(s => s.Element == _monster.Element && s.Archetype == _monster.Archetype.Type)
                .ToList();
            return ret[Random.Range(0, ret.Count)];
        }
    }
}