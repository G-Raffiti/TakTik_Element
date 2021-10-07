using System;
using System.Collections.Generic;
using Stats;
using Units;
using UnityEngine;


namespace StatusEffect
{
    public enum EDependency{ None, Magic, Physic, Focus, Affinity }
    [CreateAssetMenu(fileName = "Status_StatsBonus_", menuName = "Scriptable Object/Status Effects/Stats Bonus")]
    public class StatsBonus : StatusSO
    {
        [SerializeField] private List<Affix> Bonus;
        [SerializeField] private List<Affix> Malus;
        [SerializeField] private bool isDefinitive;
        [SerializeField] private float durationPerFocusPercent;
        [SerializeField] private EDependency powerDependency;
        [SerializeField] private float powerPercent;
        [SerializeField] private string description;


        public override void ActiveEffect(Buff _buff, Unit _unit)
        {
        }

        public override void PassiveEffect(Buff _buff, Unit _unit)
        {
            BattleStats bonus = new BattleStats(0);
            Debug.Log(Bonus.Count);
            Bonus.ForEach(affix => bonus += affix.affix.GenerateBS(affix.value));
            Malus.ForEach(affix => bonus -= affix.affix.GenerateBS(affix.value));
            bonus *= _buff.Power;

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
            bonus *= _buff.Power;

            _unit.BattleStats -= bonus;
        }
        
        

        public override float GetPower(Unit sender)
        {
            switch (powerDependency)
            {
                case EDependency.Magic:
                    return sender.BattleStats.Power.Magic(Element.Type) * powerPercent /100f;
                case EDependency.Physic:
                    return sender.BattleStats.Power.Physic(Element.Type) * powerPercent /100f;
                case EDependency.Focus:
                    return sender.BattleStats.GetFocus(Element.Type) * powerPercent /100f;
                case EDependency.Affinity:
                    return sender.BattleStats.Power.Affinity.GetAffinity(Element.Type) * powerPercent /100f;
                default:
                    return 1;
            }
        }

        public override int GetDuration(Unit sender)
        {
            return (int)(sender.BattleStats.GetFocus(Element.Type) * (durationPerFocusPercent / 100f));
        }

        public override string InfoEffect(Buff _buff)
        {
            string str = "";
            if (Bonus.Count > 0)
            {
                str += $"Stats Bonus:";
                Bonus.ForEach(affix => str += affix.Value((int)(affix.value * _buff.Power)));
            }
            if (Malus.Count > 0)
            {
                str += $"\nStats Malus:";
                Malus.ForEach(affix => str += $"-{affix.Value((int)(affix.value * _buff.Power))}");
            }
            if (_buff.Duration != 0)
                str += $"\n Duration: {_buff.Duration} Turn";
            return str;
        }

        public override string InfoOnUnit(Buff _buff, Unit _unit)
        {
            return InfoEffect(_buff);
        }

        public override string InfoOnFloor(Buff _buff)
        {
            string str = "When any Unit enter this Cell it will be Buffed: \n";
            return str + InfoEffect(_buff);
        }
    }
}