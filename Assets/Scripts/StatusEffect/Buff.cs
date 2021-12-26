using System;
using Cells;
using StateMachine;
using Units;
using UnityEngine;

namespace StatusEffect
{
    [Serializable]
    public class Buff
    {
        public int Duration;
        public float Value;

        [SerializeField] private StatusSO StatusEffect;
        public StatusSO Effect => StatusEffect;
        public Buff(Buff _buff)
        {
            Duration = _buff.Duration;
            Value = _buff.Value;
            StatusEffect = _buff.StatusEffect;
        }

        public static Buff operator +(Buff a, Buff b)
        {
            if (a.Effect != b.Effect) return a;
            Buff ret = a.Effect.AddBuff(a, b);
            return ret;
        }
        
        public Buff (Unit sender, StatusSO _status)
        {
            StatusEffect = _status;
            Duration = StatusEffect.GetBuffDuration(sender);
            Value = StatusEffect.GetBuffValue(sender);
        }
        
        public Buff (Cell tile, StatusSO _status)
        {
            StatusEffect = _status;
            Duration = 0;
            Value = 10;
        }

        /// <summary>
        /// Methode Called on the End of the Playing Unit's Turn
        /// </summary>
        public void OnEndTurn(Unit _unit)
        {
            if (_unit != BattleStateManager.instance.PlayingUnit) return;
            if (!StatusEffect.BetweenTurn) 
                StatusEffect.ActiveEffect(this, _unit);
        }

        /// <summary>
        /// Methode Called on the Start of any Unit's Turn
        /// </summary>
        public void OnStartTurn(Unit _unit)
        {
            if (StatusEffect.BetweenTurn)
                StatusEffect.ActiveEffect(this, _unit);
        }
        
        /// <summary>
        /// Add all the Stats Bonus or Malus from the Buff to the affected Unit
        /// </summary>
        /// <param name="unit"></param>
        public void Apply(Unit unit)
        {
            StatusEffect.PassiveEffect(this, unit);
        }

        /// <summary>
        /// Remove all the Stats Bonus or Malus from the Buff from the affected Unit
        /// </summary>
        /// <param name="unit"></param>
        public void Undo(Unit unit)
        {
            StatusEffect.EndPassiveEffect(this, unit);
        }

        /// <summary>
        /// Infos given on the Skill
        /// </summary>
        public string InfoBuff()
        {
            return StatusEffect.InfoEffect(this);
        }

        /// <summary>
        /// Info given on the Affected Unit
        /// </summary>
        public string InfoOnUnit(Buff _buff, Unit _unit)
        {
            return StatusEffect.InfoOnUnit(_buff, _unit);
        }

        /// <summary>
        /// Info Given on CtRl holded while Hovering a Cell
        /// </summary>
        /// <returns></returns>
        public string InfoBuffOnCell(Cell _cell)
        {
            return StatusEffect.InfoOnFloor(_cell, this);
        }
    }
}