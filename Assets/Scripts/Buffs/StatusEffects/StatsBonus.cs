using System;
using System.Collections.Generic;
using Cells;
using Stats;
using Units;
using UnityEngine;
using UnityEngine.Serialization;

namespace Buffs
{
    public enum EDependency{ None, Power, Focus, Affinity }
    [CreateAssetMenu(fileName = "Status_StatsBonus_", menuName = "Scriptable Object/Status Effects/Stats Bonus")]
    public class StatsBonus : StatusSo
    {
        [FormerlySerializedAs("Bonus")]
        [SerializeField] private List<Affix> bonus;
        [FormerlySerializedAs("Malus")]
        [SerializeField] private List<Affix> malus;
        [SerializeField] private EDependency dependOn;
        [SerializeField] private int fixValue;

        public override void ActiveEffect(Buff _buff, Unit _unit)
        {
        }

        public override void PassiveEffect(Buff _buff, Unit _unit)
        {
            BattleStats _bonus = new BattleStats(0);
            this.bonus.ForEach(_affix => _bonus += _affix.affix.GenerateBs(_affix.value));
            malus.ForEach(_affix => _bonus -= _affix.affix.GenerateBs(_affix.value));
            _bonus = _bonus + _bonus * _buff.value;

            _unit.battleStats += _bonus;
            if (_unit.battleStats.speed <= 0)
                _unit.battleStats.speed = 2;
        }

        public override void EndPassiveEffect(Buff _buff, Unit _unit)
        {
            if (isDefinitive) return;
            
            BattleStats _bonus = new BattleStats();
            this.bonus.ForEach(_affix => _bonus += _affix.affix.GenerateBs(_affix.value));
            malus.ForEach(_affix => _bonus -= _affix.affix.GenerateBs(_affix.value));
            _bonus = _bonus + _bonus * _buff.value;

            _unit.battleStats -= _bonus;
        }
        
        public override float GetBuffValue(Unit _sender)
        {
            int _factor = fixValue;
            switch (dependOn)
            {
                case EDependency.Power:
                    _factor += _sender.battleStats.power;
                    break;
                case EDependency.Affinity:
                    _factor += _sender.battleStats.GetPower(Element.Type);
                    break;
            }

            return _factor;
        }

        public override Buff AddBuff(Buff _a, Buff _b)
        {
            if (_a.Effect != _b.Effect) return _a;
            Buff _ret = new Buff(_a);
            if (isDefinitive)
            {
                return _b;
            }
            _ret.value = Math.Max(_a.value, _b.value);
            _ret.duration += _b.duration;
            return _ret;
        }

        public override string InfoEffect(Buff _buff)
        {
            string _str = "";
            if (bonus.Count > 0)
            {
                _str += $"Bonus: ";
                bonus.ForEach(_affix => _str += _affix.ValueToString((int)(_affix.value + _affix.value * _buff.value)));
            }
            if (malus.Count > 0)
            {
                _str += $"\nMalus: ";
                malus.ForEach(_affix => _str += $"-{_affix.ValueToString((int)(_affix.value + _affix.value * _buff.value))}");
            }
            if (_buff.duration != 0)
                _str += $"\n<sprite name=Duration>: {_buff.duration} Turn";
            return _str;
        }

        public override string InfoOnUnit(Buff _buff, Unit _unit)
        {
            return InfoEffect(_buff);
        }

        public override string InfoOnFloor(Cell _cell, Buff _buff)
        {
            string _str = "When any Unit enter this Cell it will be Buffed: \n";
            return _str + InfoEffect(_buff);
        }

        public override void OnUnitTakeCell(Buff _buff, Unit _unit)
        {
            _unit.ApplyBuff(_buff);
        }
    }
}