using System;
using Units;
using UnityEngine;

namespace StatusEffect
{
    [CreateAssetMenu(fileName = "Status_Poison", menuName = "Scriptable Object/Status Effects/Poisoned")]
    public class Poison : StatusSO
    {
        [SerializeField] private int duration = 10;
        public override void ActiveEffect(Buff _buff, Unit _unit)
        {
            _unit.DefendHandler(_unit, _buff.Power, Element);
        }

        public override void PassiveEffect(Buff _buff, Unit _unit)
        {
        }

        public override void EndPassiveEffect(Buff _buff, Unit _unit)
        {
        }

        public override float GetPower(Unit sender)
        {
            return sender.BattleStats.GetPower(Element.Type);
        }

        public override int GetDuration(Unit sender)
        {
            return duration;
        }

        public override string InfoEffect(Buff _buff)
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(Element.TextColour);
            return $"Poison Damage: <color=#{_hexColor}>{_buff.Power}</color> each time an Unit play their Turn \n Duration: {_buff.Duration} Turn";
        }

        public override string InfoOnUnit(Buff _buff, Unit _unit)
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(Element.TextColour);
            return
                $"Poisoned: -<color=#{_hexColor}>{(int)_buff.Power}</color> HP / Unit's Turn\nDuration: last for {_buff.Duration} Turn";
        }

        public override string InfoOnFloor(Buff _buff)
        {
            return InfoEffect(_buff);
        }
    }
}