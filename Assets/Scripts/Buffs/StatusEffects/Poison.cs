using System;
using Cells;
using Units;
using UnityEngine;

namespace Buffs
{
    [CreateAssetMenu(fileName = "Status_Poison", menuName = "Scriptable Object/Status Effects/Poisoned")]
    public class Poison : StatusSo
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
            _ret.value = Math.Max(_a.value, _b.value);
            _ret.duration += _b.duration;
            return _ret;
        }

        public override void OnUnitTakeCell(Buff _buff, Unit _unit)
        {
            Buff _apply = new Buff(_buff);
            _apply.duration = 1;
            _apply.value /= 2;
            
            _unit.ApplyBuff(_apply);
        }

        public override string InfoEffect(Buff _buff)
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(Element.TextColour);
            return $"{Name}: -<color=#{_hexColor}>{_buff.value}</color> <sprite name=HP> on every Unit's Turn \n<sprite name=Duration>: {_buff.duration} Turn";
        }

        public override string InfoOnUnit(Buff _buff, Unit _unit)
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(Element.TextColour);
            return $"{Name}: -<color=#{_hexColor}>{_buff.value}</color> <sprite name=HP> on every Unit's Turn \n<sprite name=Duration>: {_buff.duration} Turn";
        }

        public override string InfoOnFloor(Cell _cell, Buff _buff)
        {
            string _str = $"{Name}: a new Stack of {Name} will be applied when Unit's pass By this Cell (-{_buff.value / 2} <sprite name=HP> / 1 <sprite name=Duration>)";
            if (_cell.CellSo.BasicBuff.Effect != this)
                _str += $"\n<sprite name=Duration>: {_buff.duration} Turn";
            return _str;
        }
    }
}