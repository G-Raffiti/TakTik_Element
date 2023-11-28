using System.Linq;
using Cells;
using Skills._Zone;
using UnityEngine;

namespace Skills.ScriptableObject_Effect
{
    [CreateAssetMenu(fileName = "SkillEffect_ApplyBuff", menuName = "Scriptable Object/Skills/Skill Effect Buff")]
    public class ApplyBuff : SkillEffect
    {
        public override void Use(Cell _targetCell, SkillInfo _skillInfo)
        {
            foreach (Cell _cell in Zone.GetZone(_skillInfo.skill.GridRange, _targetCell).Where(_cell => _cell.CurrentUnit != null))
            {
                _skillInfo.skill.Buffs.ForEach(_buff =>
                {
                    _cell.CurrentUnit.ApplyBuff(_buff);
                });
            }
        }

        public override bool CanUse(Cell _cell, SkillInfo _skillInfo)
        {
            return _cell != null;
        }

        public override string InfoEffect(SkillInfo _skillInfo)
        {
            string _str = "Apply ";
            _skillInfo.skill.Buffs.ForEach(_buff => _str += $"{_buff.Effect.Name} ");
            return _str;
        }
        
        public override string InfoEffect()
        {
            return "";
        }
    }
}