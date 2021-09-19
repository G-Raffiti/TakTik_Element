using System;
using Units;
using UnityEngine;

namespace StatusEffect
{
    [CreateAssetMenu(fileName = "Status_Poison", menuName = "Scriptable Object/Status Effects/Poisoned")]
    public class Poison : StatusSO
    {
        [SerializeField] private float damagePerSpeedPercent = 10;
        [SerializeField] private float durationPerFocusPercent = 50;
        public override void ActiveEffect(Buff _buff, Unit _unit)
        {
            _unit.DefendHandler(_unit, _buff.Power * (_unit.BattleStats.Speed * damagePerSpeedPercent / 100f), Element);
        }

        public override void PassiveEffect(Buff _buff, Unit _unit)
        {
        }

        public override void EndPassiveEffect(Buff _buff, Unit _unit)
        {
        }

        public override int GetPower(Unit sender)
        {
            return sender.BattleStats.GetFocus(Element.Type);
        }

        public override int GetDuration(Unit sender)
        {
            return Math.Max(1, (int)(sender.BattleStats.Focus * (durationPerFocusPercent / 100f)));
        }

        public override string InfoEffect(Buff _buff)
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(Element.TextColour);
            return $"Poison Damage: <color=#{_hexColor}>{_buff.Power}</color> each time an Unit play their Turn \n Duration: {_buff.Duration} Turn";
        }

        public override string InfoApply()
        {
            return
                $"Damage depend of Focus of {Element.Name} and {damagePerSpeedPercent}% of the Targeted Unit's Speed \n Duration depend on Focus of {Element.Name} ({durationPerFocusPercent}%)";
        }

        public override string InfoOnUnit(Buff _buff, Unit _unit)
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(Element.TextColour);
            return
                $"Poisoned: -<color=#{_hexColor}>{(int)(_buff.Power * (_unit.BattleStats.Speed * damagePerSpeedPercent / 100f))}</color> HP / Unit's Turn\nDuration: last for {_buff.Duration} Turn";
        }
    }
}