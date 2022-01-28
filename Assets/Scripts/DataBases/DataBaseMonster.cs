using System.Collections.Generic;
using System.Linq;
using Units;
using UnityEditor;
using UnityEngine;

namespace DataBases
{
    [CreateAssetMenu(fileName = "DataBase_Monster", menuName = "Scriptable Object/DataBase/Monster")]
    public class DataBaseMonster : ScriptableObject
    {
        [SerializeField] private List<MonsterSO> allMonsters;

        public List<MonsterSO> Monsters => allMonsters;
        public List<MonsterSO> AllMinions => allMonsters.Where(monster => monster.Type == EMonster.Minion).ToList();
        public List<MonsterSO> AllBosses => allMonsters.Where(monster => monster.Type == EMonster.Boss).ToList();
        public List<MonsterSO> AllInvocs => allMonsters.Where(monster => monster.Type == EMonster.Invoc).ToList();

        public void AddMonster(MonsterSO newMonster)
        {
            if (Monsters.Contains(newMonster)) return;
            #if (UNITY_EDITOR)
                allMonsters.Add(newMonster);
                EditorUtility.SetDirty(this); 
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            #endif
        }

        public void ClearDataBase()
        {
            allMonsters = new List<MonsterSO>();
        }
    }
}