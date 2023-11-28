using System.Collections.Generic;
using System.Linq;
using EndConditions;
using UnityEngine;

namespace Cells
{
    [CreateAssetMenu(fileName = "DataBase_Board", menuName = "Scriptable Object/DataBase/Board")]
    public class DataBaseBoard : ScriptableObject
    {
        [SerializeField] private List<BoardSo> allBoards;
        public List<BoardSo> LastBattleBoards => allBoards.Where(_b => _b.EndCondition.Type == EConditionType.Last).ToList();
        public List<BoardSo> DeathBattleBoards => allBoards.Where(_b => _b.EndCondition.Type == EConditionType.Death).ToList();
        public List<BoardSo> BossBattleBoards => allBoards.Where(_b => _b.EndCondition.Type == EConditionType.Boss).ToList();
        public List<BoardSo> LootBoxBoards => allBoards.Where(_b => _b.EndCondition.Type == EConditionType.LootBox).ToList();
    }
}