using Cells;
using Units;
using UnityEngine;

namespace Buffs
{
    [CreateAssetMenu(fileName = "Status_Burning", menuName = "Scriptable Object/Status Effects/Burned")]
    public class Burned : StatusSo
    {
        public override void ActiveEffect(Buff _buff, Unit _unit)
        {
            _unit.DefendHandler(_unit, _buff.value, Element);
        }

        public override void PassiveEffect(Buff _buff, Unit _unit)
        {
        }

        public override void EndPassiveEffect(Buff _buff, Unit _unit)
        {
        }

        public override float GetBuffValue(Unit _sender)
        {
            return _sender.battleStats.GetPower(Element.Type);
        }

        public override Buff AddBuff(Buff _a, Buff _b)
        {
            if (_a.Effect != _b.Effect) return _a;
            Buff _ret = new Buff(_a);
            _ret.value += _b.value;
            _ret.duration += 1;
            return _ret;
        }

        public override void OnUnitTakeCell(Buff _buff, Unit _unit)
        {
            _unit.DefendHandler(_unit, _buff.value/2f, Element);
        }

        public override string InfoEffect(Buff _buff)
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(Element.TextColour);
            return $"{Name}: -<color=#{_hexColor}>{_buff.value}</color> <sprite name=HP> on this Unit's End Turn \n<sprite name=Duration>: {_buff.duration} Turn";
        }

        public override string InfoOnUnit(Buff _buff, Unit _unit)
        {
            return InfoEffect(_buff);
        }

        public override string InfoOnFloor(Cell _cell, Buff _buff)
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(Element.TextColour);
            string _str =
                $"{Name}: -<color=#{_hexColor}>{(int) (_buff.value / 2f)}</color> <sprite name=HP> when Unit's pass By this Cell";
            if (_cell.CellSo.BasicBuff.Effect != this)
                _str += $"\n<sprite name=Duration>: {_buff.duration} Turn";
            return _str;
        }
    }
}