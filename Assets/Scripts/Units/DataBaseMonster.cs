using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Units
{
    [CreateAssetMenu(fileName = "DataBase_Monster", menuName = "Scriptable Object/DataBase/Monster")]
    public class DataBaseMonster : ScriptableObject
    {
        [SerializeField] private List<MonsterSO> allMonster;
        public List<MonsterSO> AllMinions => allMonster.Where(monster => monster.Type == EMonster.Minion).ToList();
        public List<MonsterSO> AllBosses => allMonster.Where(monster => monster.Type == EMonster.Boss).ToList();
        public List<MonsterSO> AllInvocs => allMonster.Where(monster => monster.Type == EMonster.Invoc).ToList();
    }
}