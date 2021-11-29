using Units;
using UnityEngine;

namespace StatusEffect
{
    [CreateAssetMenu(fileName = "Status_Burning", menuName = "Scriptable Object/Status Effects/Burned")]
    public class Burned : StatusSO
    {
        [SerializeField] private int duration;
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

        public override int GetBuffDuration(Unit sender)
        {
            return duration;
        }

        public override string InfoEffect(Buff _buff)
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(Element.TextColour);
            return $"Burning Damage: <color=#{_hexColor}>{_buff.Value}</color> at the End of the Unit Turn \n Duration: {_buff.Duration} Turn";
        }

        public override string InfoOnUnit(Buff _buff, Unit _unit)
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(Element.TextColour);
            return $"Burned: -<color=#{_hexColor}>{_buff.Value}</color> HP / Turn \n Duration: last {_buff.Duration} Turn";
        }

        public override string InfoOnFloor(Buff _buff)
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(Element.TextColour);
            return $"Burned: -<color=#{_hexColor}>{_buff.Value}</color> HP / Turn \n Duration: {_buff.Duration} Turn";
        }
    }
}