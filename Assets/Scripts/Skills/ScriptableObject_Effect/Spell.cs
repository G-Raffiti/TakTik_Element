﻿using System;
using System.Collections.Generic;
using _ScriptableObject;
using Cells;
using Grid;
using Skills._Zone;
using Units;
using UnityEngine;

namespace Skills.ScriptableObject_Effect
{
    [CreateAssetMenu(fileName = "SkillEffect_Spell", menuName = "Scriptable Object/Skills/Skill Effect Spell")]
    public class Spell : SkillEffect
    {
        public override void Use(Cell _cell, SkillInfo _skillInfo)
        {
            int _damage = _skillInfo.Power.Magic(_skillInfo.Element.Type);
            
            foreach (Cell _cellAffected in Zone.GetZone(_skillInfo.Range, _cell))
            {
                Unit _unitAffected = Zone.GetUnitAffected(_cellAffected, _skillInfo);
                if (_unitAffected != null)
                    _unitAffected.DefendHandler(_skillInfo.Unit, _damage, _skillInfo.Element);
            }
        }

        public override bool CanUse(Cell _cell, SkillInfo _skillInfo)
        {
            return true;
        }
        
        public override string InfoEffect(SkillInfo _skillInfo)
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(_skillInfo.Element.TextColour);
            return $"Damage: <color=#{_hexColor}>{_skillInfo.Power.Magic(_skillInfo.Element.Type)}</color>";
        }
        public override string InfoEffect()
        {
            return "do Damage according to the Unit's Spell Power and Element Affinity";
        }
        
        public override Dictionary<Cell, int> DamageValue(Cell _cell, SkillInfo _skillInfo)
        {
            int _damage = _skillInfo.Power.Magic(_skillInfo.Element.Type);
            Dictionary<Cell, int> ret = new Dictionary<Cell, int>();
            foreach (Cell _cellInZone in Zone.GetZone(_skillInfo.Range, _cell))
            {
                Unit _unitAffected = Zone.GetUnitAffected(_cellInZone, _skillInfo);
                if (_unitAffected != null)
                    ret.Add(_cellInZone, _unitAffected.DamageTaken(_skillInfo.Unit, _damage, _skillInfo.Element));
            }

            return ret;
        }
        
        
    }
}