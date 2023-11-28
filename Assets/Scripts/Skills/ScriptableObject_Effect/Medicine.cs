using System.Collections.Generic;
using Cells;
using Skills._Zone;
using Units;
using UnityEngine;

namespace Skills.ScriptableObject_Effect
{
    [CreateAssetMenu(fileName = "SkillEffect_Medicine", menuName = "Scriptable Object/Skills/Skill Effect Medicine")]
    public class Medicine : SkillEffect
    {
        public override void Use(Cell _targetCell, SkillInfo _skillInfo)
        {
            int _heal = - _skillInfo.GetPower();
            
            foreach (Cell _cellAffected in Zone.GetZone(_skillInfo.skill.GridRange, _targetCell))
            {
                Unit _unitAffected = Zone.GetUnitAffected(_cellAffected, _skillInfo);
                if (_unitAffected != null)
                    _unitAffected.DefendHandler(_skillInfo.unit, _heal, _skillInfo.skill.Element);
            }
        }

        public override bool CanUse(Cell _cell, SkillInfo _skillInfo)
        {
            return true;
        }
        
        public override string InfoEffect(SkillInfo _skillInfo)
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(_skillInfo.skill.Element.TextColour);
            return $"Healing Power: <color=#{_hexColor}>{_skillInfo.GetPower()}</color>";
        }
        public override string InfoEffect()
        {
            return "heal according to the Unit's Skill Power and Element Affinity";
        }
        
        public override Dictionary<Cell, int> DamageValue(Cell _cell, SkillInfo _skillInfo)
        {
            int _heal = - _skillInfo.GetPower();
            Dictionary<Cell, int> _ret = new Dictionary<Cell, int>();
            foreach (Cell _cellInZone in Zone.GetZone(_skillInfo.skill.GridRange, _cell))
            {
                Unit _unitAffected = Zone.GetUnitAffected(_cellInZone, _skillInfo);
                if (_unitAffected != null)
                    _ret.Add(_cellInZone, _unitAffected.DamageTaken(_heal, _skillInfo.skill.Element));
            }

            return _ret;
        }
    }
}