using System;
using Cells;
using Grid;
using Units;

namespace StatusEffect
{
    [Serializable]
    public class Buff
    {
        public int Duration { get; set; }
        public float Power { get; set; }

        private StatusSO StatusEffect;
        public StatusSO Effect => StatusEffect;
        public Buff(Buff _buff)
        {
            Duration = _buff.Duration;
            Power = _buff.Power;
            StatusEffect = _buff.StatusEffect;
        }

        public static Buff operator +(Buff a, Buff b)
        {
            if (a.Effect != b.Effect) return a;
            Buff ret = new Buff(a)
            {
                Duration = a.Duration + b.Duration,
                Power = a.Power + b.Power,
                StatusEffect = a.StatusEffect,
            };
            return ret;
        }
        
        public Buff (Unit sender, StatusSO _status)
        {
            StatusEffect = _status;
            Duration = StatusEffect.GetDuration(sender);
            Power = StatusEffect.GetPower(sender);
        }
        
        public Buff (Cell tile, StatusSO _status)
        {
            StatusEffect = _status;
            Duration = 1000;
            Power = tile.Power;
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
    }
}