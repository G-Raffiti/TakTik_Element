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
    public class LootBoxes : EndConditionSo
    {
        public override bool BattleIsOver(BattleStateManager _stateManager)
        {
            List<Unit> _playerHeroes = _stateManager.Units.Where(_unit => _unit.playerType == EPlayerType.Human).ToList();
            WinCondition = _playerHeroes.Count > 0;
            
            return _stateManager.GridObjects.Count == 0 || _stateManager.Turn >= 3 || _playerHeroes.Count == 0;
        }
    }
}