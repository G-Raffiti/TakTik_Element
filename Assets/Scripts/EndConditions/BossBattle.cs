﻿using System.Collections.Generic;
using System.Linq;
using _Instances;
using Players;
using StateMachine;
using Units;
using UnityEngine;

namespace EndConditions
{
    [CreateAssetMenu(fileName = "EndCondition_BossBattle", menuName = "Scriptable Object/End Conditions/Boss Battle")]
    public class BossBattle : EndConditionSO
    {
        public override bool battleIsOver(BattleStateManager StateManager)
        {
            List<EPlayerType> _totalPlayersAlive = StateManager.Units.Select(u => u.playerType).Distinct().ToList();
            List<Unit> playerHeroes = StateManager.Units.Where(unit => unit.playerType == EPlayerType.HUMAN).ToList();
            List<Monster> bosses = new List<Monster>();
            foreach (Unit monsterUnit in StateManager.Units.Except(playerHeroes).ToList())
            {
                if (!(monsterUnit is Monster _monster)) continue;
                if (_monster.Type != EMonster.Boss) continue;
                bosses.Add(_monster);
            }

            WinCondition = (playerHeroes.Count > 0) && (bosses.Count == 0);

            return _totalPlayersAlive.Count == 1 || bosses.Count == 0;
        }
    }
}