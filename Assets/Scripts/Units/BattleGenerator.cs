using System;
using System.Collections.Generic;
using _Instances;
using EndConditions;
using Random = UnityEngine.Random;

namespace Units
{
    public static class BattleGenerator
    {
        public static List<MonsterSo> GenerateEnemies(EConditionType _battleType)
        {
            switch (_battleType)
            {
                case EConditionType.LootBox : return new List<MonsterSo>();
                case EConditionType.Death: return GenerateMinions();
                case EConditionType.Boss:
                {
                    List<MonsterSo> _ret = new List<MonsterSo>();
                    _ret.Add(DataBase.Monster.AllBosses[Random.Range(0, DataBase.Monster.AllBosses.Count)]);
                    _ret.AddRange(GenerateMinions());
                    return _ret;
                }
                case EConditionType.Last:
                {
                    List<MonsterSo> _ret = new List<MonsterSo>();
                    _ret.Add(DataBase.Monster.AllBosses[Random.Range(0, DataBase.Monster.AllBosses.Count)]);
                    _ret.AddRange(GenerateMinions());
                    return _ret;
                }
                default: return GenerateMinions();
            }
        }

        private static List<MonsterSo> GenerateMinions()
        {
            int _totalLvl = Random.Range(BattleStage.Stage, BattleStage.Stage * 2) + 1 + Math.Max(0, BattleStage.BattleNumber);
            List<MonsterSo> _ret = new List<MonsterSo>();
            int _actualLvl = 0;
            while (_actualLvl < _totalLvl)
            {
                MonsterSo _randomMonster = DataBase.Monster.AllMinions[Random.Range(0, DataBase.Monster.AllMinions.Count)];
                _ret.Add(_randomMonster);
                _actualLvl += _randomMonster.Level;
            }
            return _ret;
        }
    }
}
