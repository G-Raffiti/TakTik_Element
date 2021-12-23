﻿using System;
using System.Linq;
using Cells;
using Units;
using UnityEngine;

namespace StatusEffect
{
    [CreateAssetMenu(fileName = "Status_Drowned", menuName = "Scriptable Object/Status Effects/Drowned")]
    public class Drowned : StatusSO
    {
        [SerializeField] private int percent;
        [SerializeField] private int WaterBonus;
        public override void ActiveEffect(Buff _buff, Unit _unit)
        {
            if (((TileIsometric) _unit.Cell).CellSO.Type == ECellType.Water)
            {
                _unit.DefendHandler(_unit, Math.Min(_unit.BattleStats.HP * percent/100f , _unit.BattleStats.HP-1), Element);
            }
        }

        public override void PassiveEffect(Buff _buff, Unit _unit)
        {
            _unit.BattleStats.Affinity.Water += WaterBonus;
        }

        public override void EndPassiveEffect(Buff _buff, Unit _unit)
        {
            _unit.BattleStats.Affinity.Water -= WaterBonus;
        }

        public override float GetBuffValue(Unit sender)
        {
            return percent;
        }

        public override int GetBuffDuration(Unit sender)
        {
            return Math.Max(baseDuration, sender.BattleStats.Focus);
        }

        public override void OnUnitTakeCell(Buff _buff, Unit _unit)
        {
            if (_unit.Buffs.Any(b => b.Effect == this))
            {
                ActiveEffect(_buff, _unit);
                return;
            }
            
            _unit.ApplyBuff(_buff);
        }

        public override Buff AddBuff(Buff a, Buff b)
        {
            if (a.Effect != b.Effect) return a;
            Buff ret = new Buff(a);
            ret.Value = Math.Max(a.Value, b.Value);
            ret.Duration += b.Duration;
            return ret;
        }

        public override string InfoEffect(Buff _buff)
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(Element.TextColour);
            return $"At the end of the Unit's turn if it stays on Water, it Loose {_buff.Value}% of your <sprite name=HP> \n the Unit have a bonus of {WaterBonus} <sprite name=Water> \n Duration : {_buff.Duration}";
        }

        public override string InfoOnUnit(Buff _buff, Unit _unit)
        {
            return InfoEffect(_buff);
        }

        public override string InfoOnFloor(Buff _buff)
        {
            return $"if any Unit Move in Water it become Drowned,\n if a Drowned Unit step on Water it Loose {_buff.Value}% of their <sprite name=HP> \n the Unit have a bonus of {WaterBonus} <sprite name=Water> \n {_buff.Duration}";
        }
    }
}