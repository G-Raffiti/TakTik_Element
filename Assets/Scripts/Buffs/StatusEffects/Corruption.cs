using System;
using Cells;
using Units;
using UnityEngine;

namespace Buffs
{
    [CreateAssetMenu(fileName = "Status_Corruption", menuName = "Scriptable Object/Status Effects/Corruption")]
    public class Corruption : StatusSo
    {
        public override void ActiveEffect(Buff _buff, Unit _unit)
        {
            _unit.DefendHandler(_unit, 1+(int)((_buff.value / 100) * _unit.battleStats.hp), Element);
        }

        public override void OnUnitTakeCell(Buff _buff, Unit _unit)
        {
            _unit.DefendHandler(_unit, Math.Min(1, (int) ((_buff.value / 100) * _unit.battleStats.hp * 0.5)), Element);
        }

        public override void PassiveEffect(Buff _buff, Unit _unit)
        {
        }

        public override void EndPassiveEffect(Buff _buff, Unit _unit)
        {
        }

        public override float GetBuffValue(Unit _sender)
        {
            return 10 ;
        }

        public override int GetBuffDuration(Unit _sender)
        {
            return 10 ;
        }

        public override Buff AddBuff(Buff _a, Buff _b)
        {
            if (_a.Effect != _b.Effect) return _a;
            Buff _ret = new Buff(_a);
            _ret.value += _b.value;
            return _ret;
        }

        public override string InfoEffect(Buff _buff)
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(Element.TextColour);
            return $"{Name}: - {_buff.value}% of <sprite name=HP> / Turn";
        }

        public override string InfoOnUnit(Buff _buff, Unit _unit)
        {
            return $"{Name}: - {_buff.value}% of <sprite name=HP> / Turn \n<sprite name=Duration>: {_buff.duration} Turn";
        }

        public override string InfoOnFloor(Cell _cell, Buff _buff)
        {
            string _str =
                $"{Name}: - {_buff.value / 2}% of <sprite name=HP> when Unit's pass By this Cell";
            if (_cell.CellSo.BasicBuff.Effect != this)
                _str += $"\n<sprite name=Duration>: {_buff.duration} Turn";
            return _str;
        }
    }
}