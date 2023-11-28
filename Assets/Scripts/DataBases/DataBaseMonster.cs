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
        [SerializeField] private List<MonsterSo> allMonsters;

        public List<MonsterSo> Monsters => allMonsters;
        public List<MonsterSo> AllMinions => allMonsters.Where(_monster => _monster.Type == EMonster.Minion).ToList();
        public List<MonsterSo> AllBosses => allMonsters.Where(_monster => _monster.Type == EMonster.Boss).ToList();
        public List<MonsterSo> AllInvocs => allMonsters.Where(_monster => _monster.Type == EMonster.Invoc).ToList();

        public void AddMonster(MonsterSo _newMonster)
        {
            if (Monsters.Contains(_newMonster)) return;
            #if (UNITY_EDITOR)
                allMonsters.Add(_newMonster);
                EditorUtility.SetDirty(this); 
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            #endif
        }

        public void ClearDataBase()
        {
            allMonsters = new List<MonsterSo>();
        }
    }
}