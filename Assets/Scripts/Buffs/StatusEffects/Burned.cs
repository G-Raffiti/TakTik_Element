using Cells;
using Units;
using UnityEngine;

namespace Buffs
{
    [CreateAssetMenu(fileName = "Status_Burning", menuName = "Scriptable Object/Status Effects/Burned")]
    public class Burned : StatusSO
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
            ret.Value += b.Value;
            ret.Duration += 1;
            return ret;
        }

        public override void OnUnitTakeCell(Buff _buff, Unit _unit)
        {
            _unit.DefendHandler(_unit, _buff.Value/2f, Element);
        }

        public override string InfoEffect(Buff _buff)
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(Element.TextColour);
            return $"{Name}: -<color=#{_hexColor}>{_buff.Value}</color> <sprite name=HP> on this Unit's End Turn \n<sprite name=Duration>: {_buff.Duration} Turn";
        }

        public override string InfoOnUnit(Buff _buff, Unit _unit)
        {
            return InfoEffect(_buff);
        }

        public override string InfoOnFloor(Cell _cell, Buff _buff)
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(Element.TextColour);
            string str =
                $"{Name}: -<color=#{_hexColor}>{(int) (_buff.Value / 2f)}</color> <sprite name=HP> when Unit's pass By this Cell";
            if (_cell.CellSO.BasicBuff.Effect != this)
                str += $"\n<sprite name=Duration>: {_buff.Duration} Turn";
            return str;
        }
    }
}