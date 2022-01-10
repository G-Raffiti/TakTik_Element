using Cells;
using Decks;
using Skills._Zone;
using Units;
using UnityEngine;

namespace Skills.ScriptableObject_Effect
{
    [CreateAssetMenu(fileName = "SkillEffect_Learning", menuName = "Scriptable Object/Skills/Learning")]
    public class Learning : SkillEffect
    {
        public override void Use(Cell _cell, SkillInfo _skillInfo)
        {
            if (Zone.GetUnitAffected(_cell, _skillInfo) is Monster _monster)
            {
                FindObjectOfType<AllDecksMono>().LearnSkill(_monster.monsterSkill.BaseSkill, _skillInfo.skill);
            }
        }

        public override bool CanUse(Cell _cell, SkillInfo _skillInfo)
        {
            return Zone.GetUnitAffected(_cell, _skillInfo) is Monster;
        }
        
        public override string InfoEffect(SkillInfo _skillInfo)
        {
            return $"Learn the Monster's Skill\nAdd it to this Deck";
        }
        
        public override string InfoEffect()
        {
            return $"Learn the Monster's Skill\nAdd it to this Deck";
        }
    }
}