using System.Collections.Generic;
using System.Linq;
using Players;
using StateMachine;
using Units;
using UnityEngine;

namespace EndConditions
{
    [CreateAssetMenu(fileName = "EndCondition_DeathBattle", menuName = "Scriptable Object/End Conditions/Normal Battle")]
    public class DeathBattle : EndConditionSo
    {
        public override bool BattleIsOver(BattleStateManager _stateManager)
        {
            List<EPlayerType> _totalPlayersAlive = _stateManager.Units.Select(_u => _u.playerType).Distinct().ToList();
            List<Unit> _playerHeroes = _stateManager.Units.Where(_unit => _unit.playerType == EPlayerType.Human).ToList();
            WinCondition = _playerHeroes.Count > 0;
            return _totalPlayersAlive.Count == 1;
        }
    }
}