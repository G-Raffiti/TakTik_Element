using System.Collections.Generic;
using Cells;
using Stats;
using Units;
using UnityEngine;


namespace StatusEffect
{
    public enum EDependency{ None, Power, Focus, Affinity }
    [CreateAssetMenu(fileName = "Status_StatsBonus_", menuName = "Scriptable Object/Status Effects/Stats Bonus")]
    public class StatsBonus : StatusSO
    {
        [SerializeField] private List<Affix> Bonus;
        [SerializeField] private List<Affix> Malus;
        [SerializeField] private bool isDefinitive;
        [SerializeField] private EDependency dependOn;
        [SerializeField] private int fixValue;

        public override void ActiveEffect(Buff _buff, Unit _unit)
        {
        }

        public override void PassiveEffect(Buff _buff, Unit _unit)
        {
            BattleStats bonus = new BattleStats(0);
            Bonus.ForEach(affix => bonus += affix.affix.GenerateBS(affix.value));
            Malus.ForEach(affix => bonus -= affix.affix.GenerateBS(affix.value));
            bonus = bonus + bonus * _buff.Value;

            _unit.BattleStats += bonus;
            if (_unit.BattleStats.Speed <= 0)
                _unit.BattleStats.Speed = 2;
        }

        public override void EndPassiveEffect(Buff _buff, Unit _unit)
        {
            if (isDefinitive) return;
            
            BattleStats bonus = new BattleStats();
            Bonus.ForEach(affix => bonus += affix.affix.GenerateBS(affix.value));
            Malus.ForEach(affix => bonus -= affix.affix.GenerateBS(affix.value));
            bonus = bonus + bonus * _buff.Value;

            _unit.BattleStats -= bonus;
        }
        
        public override float GetBuffValue(Unit sender)
        {
            int factor = fixValue;
            switch (dependOn)
            {
                case EDependency.Power:
                    factor += sender.BattleStats.Power;
                    break;
                case EDependency.Affinity:
                    factor += sender.BattleStats.GetPower(Element.Type);
                    break;
            }

            return factor;
        }

        public override string InfoEffect(Buff _buff)
        {
            string str = "";
            if (Bonus.Count > 0)
            {
                str += $"Bonus: ";
                Bonus.ForEach(affix => str += affix.Value((int)(affix.value + affix.value * _buff.Value)));
            }
            if (Malus.Count > 0)
            {
                str += $"\nMalus: ";
                Malus.ForEach(affix => str += $"-{affix.Value((int)(affix.value + affix.value * _buff.Value))}");
            }
            if (_buff.Duration != 0)
                str += $"\n<sprite name=Duration>: {_buff.Duration} Turn";
            return str;
        }

        public override string InfoOnUnit(Buff _buff, Unit _unit)
        {
            return InfoEffect(_buff);
        }

        public override string InfoOnFloor(Cell _cell, Buff _buff)
        {
            string str = "When any Unit enter this Cell it will be Buffed: \n";
            return str + InfoEffect(_buff);
        }

        public override void OnUnitTakeCell(Buff _buff, Unit _unit)
        {
            _unit.ApplyBuff(_buff);
        }
    }
}