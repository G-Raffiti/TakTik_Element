using System.Collections.Generic;
using System.Linq;
using _Instances;
using Grid;
using Units;
using UnityEngine;

namespace BattleOver
{
    [CreateAssetMenu(fileName = "EndCondition_BossBattle", menuName = "Scriptable Object/End Conditions/Boss Battle")]
    public class BossBattle : EndConditionSO
    {
        public override bool battleIsOver(BattleStateManager StateManager)
        {
            List<int> _totalPlayersAlive = StateManager.Units.Select(u => u.playerNumber).Distinct().ToList();
            List<Unit> playerHeroes = StateManager.Units.Where(unit => unit.playerNumber == 0).ToList();
            List<Monster> bosses = new List<Monster>();
            foreach (Unit monsterUnit in StateManager.Units.Except(playerHeroes).ToList())
            {
                if (!(monsterUnit is Monster _monster)) continue;
                if (_monster.Type != EMonster.Boss) continue;
                bosses.Add(_monster);
            }

            WinCondition = playerHeroes.Count > 0;

            if (WinCondition)
                KeepBetweenScene.Stage += 1;
            
            return _totalPlayersAlive.Count == 1 || bosses.Count == 0;
        }
    }
}