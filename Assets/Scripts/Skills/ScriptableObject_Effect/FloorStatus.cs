using Cells;
using Skills._Zone;
using UnityEngine;

namespace Skills.ScriptableObject_Effect
{
    [CreateAssetMenu(fileName = "SkillEffect_FloorStatus", menuName = "Scriptable Object/Skills/Skill Effect FloorStatus")]
    public class FloorStatus : SkillEffect
    {
        public override void Use(Cell _targetCell, SkillInfo _skillInfo)
        {
            Zone.GetZone(_skillInfo.skill.GridRange, _targetCell).ForEach(_c =>
            {
                _skillInfo.skill.Buffs.ForEach(_c.AddBuff);
            });
        }

        public override bool CanUse(Cell _cell, SkillInfo _skillInfo)
        {
            return _cell != null;
        }

        public override string InfoEffect(SkillInfo _skillInfo)
        {
            string _str = "Apply ";
            _skillInfo.skill.Buffs.ForEach(_buff => _str += $"{_buff.Effect.Name} ");
            _str += "to the floor";
            return _str;
        }
        
        public override string InfoEffect()
        {
            return "Apply All Buff of the skill to the floor";
        }
        
    }
}