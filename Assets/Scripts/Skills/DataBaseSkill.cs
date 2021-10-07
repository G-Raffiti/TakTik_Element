using System.Collections.Generic;
using Units;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Skills
{
    [CreateAssetMenu(fileName = "DataBase_Skill", menuName = "Scriptable Object/DataBase/Skill")]
    public class DataBaseSkill : ScriptableObject
    {
        [SerializeField] private List<SkillSO> allSkills;
        public List<SkillSO> AllSkills => allSkills;

        public SkillSO GetRandom()
        {
            return AllSkills[Random.Range(0, AllSkills.Count)];
        }

        [SerializeField] private SkillSO learning;
        [SerializeField] private SkillSO monsterAttack;
        public SkillSO Learning => learning;
        public SkillSO MonsterAttack => monsterAttack;

    }
}