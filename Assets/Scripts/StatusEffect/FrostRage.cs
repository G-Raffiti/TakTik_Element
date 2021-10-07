using System;
using Units;
using UnityEngine;

namespace StatusEffect
{
    [CreateAssetMenu(fileName = "Status_FrostRage", menuName = "Scriptable Object/Status Effects/FrostRage")]
    public class FrostRage : StatusSO
    {
        public override void ActiveEffect(Buff _buff, Unit _unit)
        {
            
        }

        public override void PassiveEffect(Buff _buff, Unit _unit)
        {
            _unit.BattleStats.Power.Affinity.Water += _buff.Power * 5;
            _unit.BattleStats.Speed -= (int)_buff.Power;
            if (_unit.BattleStats.Speed < 1)
                _unit.BattleStats.Speed = 1;
        }

        public override void EndPassiveEffect(Buff _buff, Unit _unit)
        {
            _unit.BattleStats.Power.Affinity.Water -= _buff.Power * 5;
            _unit.BattleStats.Speed += (int)_buff.Power;
        }

        public override float GetPower(Unit sender)
        {
            return sender.BattleStats.GetFocus(Element.Type);
        }

        public override int GetDuration(Unit sender)
        {
            return 1 + (int) Math.Max(1, sender.BattleStats.GetFocus(Element.Type) / 10f);
        }

        public override string InfoEffect(Buff _buff)
        {
            return $"this curse boost the {Element.Name} Affinity but reduce the Speed of the target";
        }

        public override string InfoApply()
        {
            return "";
        }

        public override string InfoOnUnit(Buff _buff, Unit _unit)
        {
            return $"this curse boost your {Element.Name}Affinity by {_buff.Power * 5}% but reduce your Speed by {_buff.Power}\nLast for {_buff.Duration}";
        }
    }
}