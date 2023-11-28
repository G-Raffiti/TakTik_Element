using System;
using System.Linq;
using Cells;
using Units;
using UnityEngine;
using UnityEngine.Serialization;

namespace Buffs
{
    [CreateAssetMenu(fileName = "Status_Drowned", menuName = "Scriptable Object/Status Effects/Drowned")]
    public class Drowned : StatusSo
    {
        [SerializeField] private int percent;
        [FormerlySerializedAs("WaterBonus")]
        [SerializeField] private int waterBonus;
        public override void ActiveEffect(Buff _buff, Unit _unit)
        {
            if (_buff.onFloor) return;
            if (((TileIsometric) _unit.Cell).CellSo.Type == ECellType.Water)
            {
                _unit.DefendHandler(_unit, Math.Min(_unit.battleStats.hp * percent/100f , _unit.battleStats.hp-1), Element);
            }
        }

        public override void PassiveEffect(Buff _buff, Unit _unit)
        {
            _unit.battleStats.affinity.water += waterBonus;
        }

        public override void EndPassiveEffect(Buff _buff, Unit _unit)
        {
            _unit.battleStats.affinity.water -= waterBonus;
        }

        public override float GetBuffValue(Unit _sender)
        {
            return percent;
        }

        public override int GetBuffDuration(Unit _sender)
        {
            return Math.Max(baseDuration, _sender.battleStats.focus);
        }

        public override void OnUnitTakeCell(Buff _buff, Unit _unit)
        {
            if (_unit.Buffs.Any(_b => _b.Effect == this))
            {
                _unit.DefendHandler(_unit, Math.Min(_unit.battleStats.hp * percent/100f , _unit.battleStats.hp-1), Element);
                return;
            }
            
            _unit.ApplyBuff(_buff);
        }

        public override Buff AddBuff(Buff _a, Buff _b)
        {
            if (_a.Effect != _b.Effect) return _a;
            Buff _ret = new Buff(_a);
            _ret.value = Math.Max(_a.value, _b.value);
            _ret.duration += _b.duration;
            return _ret;
        }

        public override string InfoEffect(Buff _buff)
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(Element.TextColour);
            return $"{Name}: -{_buff.value}% of <sprite name=HP> if it End a Turn on <color=#{_hexColor}>Water</color> \n+{waterBonus} of <sprite name=Water> \n<sprite name=Duration>: {_buff.duration}";
        }

        public override string InfoOnUnit(Buff _buff, Unit _unit)
        {
            return InfoEffect(_buff);
        }

        public override string InfoOnFloor(Cell _cell, Buff _buff)
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(Element.TextColour);
            string _str = $"{Name}: if any Unit Move in Water it become {Name}\n if a {Name} Unit step on <color=#{_hexColor}>Water</color> it Loose {_buff.value}% of <sprite name=HP> \nthe Unit have a bonus of {waterBonus} <sprite name=Water>";
            if (_cell.CellSo.BasicBuff.Effect != this)
                _str += $"\n<sprite name=Duration>: {_buff.duration} Turn";
            return _str;
        }
    }
}