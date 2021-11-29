using System.Collections.Generic;
using _Instances;
using EndConditions;
using Random = UnityEngine.Random;

namespace Units
{
    public static class BattleGenerator
    {
        public static List<MonsterSO> GenerateEnemies(EConditionType BattleType)
        {
            switch (BattleType)
            {
                case EConditionType.LootBox : return new List<MonsterSO>();
                case EConditionType.Death: return GenerateMinions();
                case EConditionType.Boss:
                {
                    List<MonsterSO> _ret = new List<MonsterSO>();
                    _ret.Add(DataBase.Monster.AllBosses[Random.Range(0, DataBase.Monster.AllBosses.Count)]);
                    _ret.AddRange(GenerateMinions());
                    return _ret;
                }
                default: return GenerateMinions();
            }
        }

        private static List<MonsterSO> GenerateMinions()
        {
            int _totalLvl = Random.Range(KeepBetweenScene.Stage + KeepBetweenScene.BattleNumber, KeepBetweenScene.Stage + KeepBetweenScene.BattleNumber * 2 + 1);
            List<MonsterSO> _ret = new List<MonsterSO>();
            int _actualLvl = 0;
            while (_actualLvl < _totalLvl)
            {
                MonsterSO _randomMonster = DataBase.Monster.AllMinions[Random.Range(0, DataBase.Monster.AllMinions.Count)];
                _ret.Add(_randomMonster);
                _actualLvl += _randomMonster.Level;
            }
            return _ret;
        }
    }
}
