using System.Collections.Generic;
using System.Linq;
using Players;
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
            List<EPlayerType> _totalPlayersAlive = StateManager.Units.Select(u => u.playerType).Distinct().ToList();
            List<Unit> playerHeroes = StateManager.Units.Where(unit => unit.playerType == EPlayerType.HUMAN).ToList();
            WinCondition = playerHeroes.Count > 0;
            return _totalPlayersAlive.Count == 1;
        }
    }
}