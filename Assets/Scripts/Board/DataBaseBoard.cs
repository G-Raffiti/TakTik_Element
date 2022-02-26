using System.Collections.Generic;
using System.Linq;
using EndConditions;
using UnityEngine;

namespace Cells
{
    [CreateAssetMenu(fileName = "DataBase_Board", menuName = "Scriptable Object/DataBase/Board")]
    public class DataBaseBoard : ScriptableObject
    {
        [SerializeField] private List<BoardSO> allBoards;
        public List<BoardSO> LastBattleBoards => allBoards.Where(b => b.EndCondition.Type == EConditionType.Last).ToList();
        public List<BoardSO> DeathBattleBoards => allBoards.Where(b => b.EndCondition.Type == EConditionType.Death).ToList();
        public List<BoardSO> BossBattleBoards => allBoards.Where(b => b.EndCondition.Type == EConditionType.Boss).ToList();
        public List<BoardSO> LootBoxBoards => allBoards.Where(b => b.EndCondition.Type == EConditionType.LootBox).ToList();
    }
}