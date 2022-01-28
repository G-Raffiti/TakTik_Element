using System;
using Cells;
using Units;
using UnityEngine;

namespace StatusEffect
{
    [CreateAssetMenu(fileName = "Status_Corruption", menuName = "Scriptable Object/Status Effects/Corruption")]
    public class Corruption : StatusSO
    {
        public override void ActiveEffect(Buff _buff, Unit _unit)
        {
            _unit.DefendHandler(_unit, 1+(int)((_buff.Value / 100) * _unit.BattleStats.HP), Element);
        }

        public override void OnUnitTakeCell(Buff _buff, Unit _unit)
        {
            _unit.DefendHandler(_unit, Math.Min(1, (int) ((_buff.Value / 100) * _unit.BattleStats.HP * 0.5)), Element);
        }

        public override void PassiveEffect(Buff _buff, Unit _unit)
        {
        }

        public override void EndPassiveEffect(Buff _buff, Unit _unit)
        {
        }

        public override float GetBuffValue(Unit sender)
        {
            return 10 ;
        }

        public override int GetBuffDuration(Unit sender)
        {
            return 10 ;
        }

        public override Buff AddBuff(Buff a, Buff b)
        {
            if (a.Effect != b.Effect) return a;
            Buff ret = new Buff(a);
            ret.Value += b.Value;
            return ret;
        }

        public override string InfoEffect(Buff _buff)
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(Element.TextColour);
            return $"{Name}: - {_buff.Value}% of <sprite name=HP> / Turn";
        }

        public override string InfoOnUnit(Buff _buff, Unit _unit)
        {
            return $"{Name}: - {_buff.Value}% of <sprite name=HP> / Turn \n<sprite name=Duration>: {_buff.Duration} Turn";
        }

        public override string InfoOnFloor(Cell _cell, Buff _buff)
        {
            string str =
                $"{Name}: - {_buff.Value / 2}% of <sprite name=HP> when Unit's pass By this Cell";
            if (_cell.CellSO.BasicBuff.Effect != this)
                str += $"\n<sprite name=Duration>: {_buff.Duration} Turn";
            return str;
        }
    }
}