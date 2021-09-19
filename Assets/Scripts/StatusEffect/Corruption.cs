using Units;
using UnityEngine;

namespace StatusEffect
{
    [CreateAssetMenu(fileName = "Status_Corruption", menuName = "Scriptable Object/Status Effects/Corruption")]
    public class Corruption : StatusSO
    {
        public override void ActiveEffect(Buff _buff, Unit _unit)
        {
            _unit.DefendHandler(_unit, 1+(int)(0.1f * _unit.BattleStats.HP), Element);
        }

        public override void PassiveEffect(Buff _buff, Unit _unit)
        {
        }

        public override void EndPassiveEffect(Buff _buff, Unit _unit)
        {
        }

        public override int GetPower(Unit sender)
        {
            return 10 ;
        }

        public override int GetDuration(Unit sender)
        {
            return 10 ;
        }

        public override string InfoEffect(Buff _buff)
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(Element.TextColour);
            return $"Corruption Damage: <color=#{_hexColor}>{_buff.Power}</color>% of the Unit's HP at the End of the Unit Turn";
        }

        public override string InfoApply()
        {
            return
                $"Corruption Damage are always 10% of the remaining Life +1";
        }

        public override string InfoOnUnit(Buff _buff, Unit _unit)
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(Element.TextColour);
            return $"Corrupted: -<color=#{_hexColor}>10%</color> of HP / Turn \n Duration: last {_buff.Duration} Turn";
        }
    }
}