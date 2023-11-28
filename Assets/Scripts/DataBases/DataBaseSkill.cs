using System.Collections.Generic;
using System.Linq;
using _Extension;
using _ScriptableObject;
using Skills;
using Stats;
using Units;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DataBases
{
    [CreateAssetMenu(fileName = "DataBase_Skill", menuName = "Scriptable Object/DataBase/Skill")]
    public class DataBaseSkill : ScriptableObject
    {
        [SerializeField] private List<SkillSo> allSkills;
        [SerializeField] private List<Element> elements;
        public List<SkillSo> AllSkills => allSkills;

        public SkillSo GetRandom()
        {
            return AllSkills[Random.Range(0, AllSkills.Count)];
        }


        public void AddSkill(SkillSo _newSkill)
        {
            if (AllSkills.Contains(_newSkill)) return;
            #if (UNITY_EDITOR)
            allSkills.Add(_newSkill);
            EditorUtility.SetDirty(this); 
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            if (_newSkill.Monster != null)
                _newSkill.Monster.SetSkill(_newSkill);
#endif
        }

        public void ClearDataBase()
        {
            allSkills = new List<SkillSo>();
        }

        public SkillSo GetSkillFor(MonsterSo _monster)
        {
            List<SkillSo> _ret = allSkills.Where(_s => _s.Element == _monster.Element && _s.Archetype == _monster.Archetype.Type)
                .ToList();
            return _ret.GetRandom();
        }
    }
}