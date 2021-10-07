using System;
using Cells;
using Units;
using UnityEngine;

namespace StatusEffect
{
    [CreateAssetMenu(fileName = "Status_Drowned", menuName = "Scriptable Object/Status Effects/Drowned")]
    public class Drowned : StatusSO
    {
        [SerializeField] private int percent;
        public override void ActiveEffect(Buff _buff, Unit _unit)
        {
            if (((TileIsometric) _unit.Cell).CellSO.Type == "Water")
            {
                _unit.DefendHandler(_unit, Math.Min(_unit.BattleStats.HP * percent/100f , _unit.BattleStats.HP-1), Element);
                _buff.Duration++;
            }
        }

        public override void PassiveEffect(Buff _buff, Unit _unit)
        {
        }

        public override void EndPassiveEffect(Buff _buff, Unit _unit)
        {
        }

        public override float GetPower(Unit sender)
        {
            return 0;
        }

        public override int GetDuration(Unit sender)
        {
            return 1;
        }

        public override string InfoEffect(Buff _buff)
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(Element.TextColour);
            return $"At the end of your turn if you stay on Water, Loose {percent}% of your <sprite name=HP>";
        }

        public override string InfoOnUnit(Buff _buff, Unit _unit)
        {
            return InfoEffect(_buff);
        }

        public override string InfoOnFloor(Buff _buff)
        {
            return "if any Units Move in Water it Loose {percent}% of their <sprite name=HP>";
        }
    }
}