using System.Collections.Generic;
using System.Linq;
using StateMachine;
using Units;
using UnityEngine;

namespace EndConditions
{
    [CreateAssetMenu(fileName = "EndCondition_DeathBattle", menuName = "Scriptable Object/End Conditions/Normal Battle")]
    public class DeathBattle : EndConditionSO
    {
        public override bool battleIsOver(BattleStateManager StateManager)
        {
            List<int> _totalPlayersAlive = StateManager.Units.Select(u => u.playerNumber).Distinct().ToList();
            List<Unit> playerHeroes = StateManager.Units.Where(unit => unit.playerNumber == 0).ToList();
            WinCondition = playerHeroes.Count > 0;
            return _totalPlayersAlive.Count == 1;
        }
    }
}