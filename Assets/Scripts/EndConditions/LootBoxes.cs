using System.Collections.Generic;
using System.Linq;
using _Instances;
using Players;
using StateMachine;
using Units;
using UnityEngine;

namespace EndConditions
{
    [CreateAssetMenu(fileName = "EndCondition_LootBoxes", menuName = "Scriptable Object/End Conditions/LootBoxes")]
    public class LootBoxes : EndConditionSO
    {
        public override bool battleIsOver(BattleStateManager StateManager)
        {
            List<Unit> playerHeroes = StateManager.Units.Where(unit => unit.playerType == EPlayerType.HUMAN).ToList();
            WinCondition = playerHeroes.Count > 0;
            
            return StateManager.GridObjects.Count == 0 || StateManager.Turn >= 3 || playerHeroes.Count == 0;
        }
    }
}