using Cells;
using Skills._Zone;
using UnityEngine;

namespace Skills.ScriptableObject_Effect
{
    [CreateAssetMenu(fileName = "SkillEffect_FloorStatus", menuName = "Scriptable Object/Skills/Skill Effect FloorStatus")]
    public class FloorStatus : SkillEffect
    {
        public override void Use(Cell _cell, SkillInfo _skillInfo)
        {
            Zone.GetZone(_skillInfo.skill.Range, _cell).ForEach(c =>
            {
                _skillInfo.skill.Buffs.ForEach(c.AddBuff);
            });
        }

        public override bool CanUse(Cell _cell, SkillInfo _skillInfo)
        {
            return _cell != null;
        }

        public override string InfoEffect(SkillInfo _skillInfo)
        {
            string str = "Apply ";
            _skillInfo.skill.Buffs.ForEach(_buff => str += $"{_buff.Effect.Name} ");
            str += "to the floor";
            return str;
        }
        
        public override string InfoEffect()
        {
            return "Apply All Buff of the skill to the floor";
        }
        
    }
}