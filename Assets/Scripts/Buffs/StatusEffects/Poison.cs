using System;
using Cells;
using Units;
using UnityEngine;

namespace Buffs
{
    [CreateAssetMenu(fileName = "Status_Poison", menuName = "Scriptable Object/Status Effects/Poisoned")]
    public class Poison : StatusSO
    {
        public override void ActiveEffect(Buff _buff, Unit _unit)
        {
            _unit.DefendHandler(_unit, _buff.Value, Element);
        }

        public override void PassiveEffect(Buff _buff, Unit _unit)
        {
        }

        public override void EndPassiveEffect(Buff _buff, Unit _unit)
        {
        }

        public override float GetBuffValue(Unit sender)
        {
            return sender.BattleStats.GetPower(Element.Type);
        }

        public override Buff AddBuff(Buff a, Buff b)
        {
            if (a.Effect != b.Effect) return a;
            Buff ret = new Buff(a);
            ret.Value = Math.Max(a.Value, b.Value);
            ret.Duration += b.Duration;
            return ret;
        }

        public override void OnUnitTakeCell(Buff _buff, Unit _unit)
        {
            Buff apply = new Buff(_buff);
            apply.Duration = 1;
            apply.Value /= 2;
            
            _unit.ApplyBuff(apply);
        }

        public override string InfoEffect(Buff _buff)
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(Element.TextColour);
            return $"{Name}: -<color=#{_hexColor}>{_buff.Value}</color> <sprite name=HP> on every Unit's Turn \n<sprite name=Duration>: {_buff.Duration} Turn";
        }

        public override string InfoOnUnit(Buff _buff, Unit _unit)
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(Element.TextColour);
            return $"{Name}: -<color=#{_hexColor}>{_buff.Value}</color> <sprite name=HP> on every Unit's Turn \n<sprite name=Duration>: {_buff.Duration} Turn";
        }

        public override string InfoOnFloor(Cell _cell, Buff _buff)
        {
            string str = $"{Name}: a new Stack of {Name} will be applied when Unit's pass By this Cell (-{_buff.Value / 2} <sprite name=HP> / 1 <sprite name=Duration>)";
            if (_cell.CellSO.BasicBuff.Effect != this)
                str += $"\n<sprite name=Duration>: {_buff.Duration} Turn";
            return str;
        }
    }
}