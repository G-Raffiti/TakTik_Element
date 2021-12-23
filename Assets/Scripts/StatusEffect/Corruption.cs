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
            return $"Corruption Damage: -<color=#{_hexColor}>1 HP \n-{_buff.Value}%</color> of HP / Turn";
        }

        public override string InfoOnUnit(Buff _buff, Unit _unit)
        {
            string _hexColor = ColorUtility.ToHtmlStringRGB(Element.TextColour);
            return $"Corrupted: -<color=#{_hexColor}>1 HP \n-{_buff.Value}%</color> of HP / Turn \n Duration: last {_buff.Duration} Turn";
        }

        public override string InfoOnFloor(Buff _buff)
        {
            return InfoEffect(_buff);
        }
    }
}