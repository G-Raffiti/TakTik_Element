using System.Collections.Generic;
using Cells;
using Skills._Zone;
using Units;
using UnityEngine;

namespace Skills.ScriptableObject_Effect
{
    [CreateAssetMenu(fileName = "SkillEffect_Healing", menuName = "Scriptable Object/Skills/Skill Effect Healing")]
    public class Healing : SkillEffect
    {
        public override void Use(Cell _cell, SkillInfo _skillInfo)
        {
            int _heal = - _skillInfo.Power.Magic(_skillInfo.Element.Type);
            
            foreach (Cell _cellAffected in Zone.GetZone(_skillInfo.Range, _cell))
            {
                Unit _unitAffected = Zone.GetUnitAffected(_cellAffected, _skillInfo);
                if (_unitAffected != null)
                    _unitAffected.DefendHandler(_skillInfo.Unit, _heal, _skillInfo.Element);
            }
        }

        public override bool CanUse(Cell _cell, SkillInfo _skillInfo)
        {
            return true;
        }
        
        public override string InfoEffect(SkillInfo _skillInfo)
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(_skillInfo.Element.TextColour);
            return $"Healing Power: <color=#{_hexColor}>{_skillInfo.Power.Magic(_skillInfo.Element.Type)}</color>";
        }
        public override string InfoEffect()
        {
            return "heal according to the Unit's Spell Power and Element Affinity";
        }
        
        public override Dictionary<Cell, int> DamageValue(Cell _cell, SkillInfo _skillInfo)
        {
            int _heal = - _skillInfo.Power.Magic(_skillInfo.Element.Type);
            Dictionary<Cell, int> ret = new Dictionary<Cell, int>();
            foreach (Cell _cellInZone in Zone.GetZone(_skillInfo.Range, _cell))
            {
                Unit _unitAffected = Zone.GetUnitAffected(_cellInZone, _skillInfo);
                if (_unitAffected != null)
                    ret.Add(_cellInZone, _unitAffected.DamageTaken(_heal, _skillInfo.Element));
            }

            return ret;
        }
    }
}