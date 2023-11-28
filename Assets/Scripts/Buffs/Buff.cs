using System;
using Cells;
using StateMachine;
using Units;
using UnityEngine;
using UnityEngine.Serialization;

namespace Buffs
{
    [Serializable]
    public class Buff
    {
        [FormerlySerializedAs("Duration")]
        public int duration;
        [FormerlySerializedAs("Value")]
        public float value;
        public bool onFloor;

        [FormerlySerializedAs("StatusEffect")]
        [SerializeField] private StatusSo statusEffect;
        public StatusSo Effect => statusEffect;
        public Buff(Buff _buff)
        {
            duration = _buff.duration;
            value = _buff.value;
            statusEffect = _buff.statusEffect;
        }

        public static Buff operator +(Buff _a, Buff _b)
        {
            if (_a.Effect != _b.Effect) return _a;
            Buff _ret = _a.Effect.AddBuff(_a, _b);
            return _ret;
        }
        
        public Buff (Unit _sender, StatusSo _status)
        {
            statusEffect = _status;
            duration = statusEffect.GetBuffDuration(_sender);
            value = statusEffect.GetBuffValue(_sender);
        }
        
        public Buff (Cell _tile, StatusSo _status)
        {
            statusEffect = _status;
            duration = 0;
            value = 10;
        }

        /// <summary>
        /// Methode Called on the End of the Playing Unit's Turn
        /// </summary>
        public void OnEndTurn(Unit _unit)
        {
            if (_unit == null)
            {
                duration -= 1;
                return;
            }
            
            if (statusEffect.BetweenTurn)
            {
                statusEffect.ActiveEffect(this, _unit);
                duration -= 1;
            }

            else if (_unit == BattleStateManager.instance.PlayingUnit)
            {
                statusEffect.ActiveEffect(this, _unit);
                duration -= 1;
            }
        }

        /// <summary>
        /// Add all the Stats Bonus or Malus from the Buff to the affected Unit
        /// </summary>
        /// <param name="unit"></param>
        public void Apply(Unit _unit)
        {
            statusEffect.PassiveEffect(this, _unit);
        }

        /// <summary>
        /// Remove all the Stats Bonus or Malus from the Buff from the affected Unit
        /// </summary>
        /// <param name="unit"></param>
        public void Undo(Unit _unit)
        {
            statusEffect.EndPassiveEffect(this, _unit);
        }

        /// <summary>
        /// Infos given on the Skill
        /// </summary>
        public string InfoBuff()
        {
            return statusEffect.InfoEffect(this);
        }

        /// <summary>
        /// Info given on the Affected Unit
        /// </summary>
        public string InfoOnUnit(Buff _buff, Unit _unit)
        {
            return statusEffect.InfoOnUnit(_buff, _unit);
        }

        /// <summary>
        /// Info Given on CtRl holded while Hovering a Cell
        /// </summary>
        /// <returns></returns>
        public string InfoBuffOnCell(Cell _cell)
        {
            return statusEffect.InfoOnFloor(_cell, this);
        }
    }
}