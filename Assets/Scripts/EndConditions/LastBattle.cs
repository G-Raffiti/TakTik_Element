using System.Collections.Generic;
using System.Linq;
using _Instances;
using Players;
using StateMachine;
using Units;
using UnityEngine;

namespace EndConditions
{
    [CreateAssetMenu(fileName = "EndCondition_LastBattle", menuName = "Scriptable Object/End Conditions/Last Battle")]
    public class LastBattle : EndConditionSo
    {
        public override bool BattleIsOver(BattleStateManager _stateManager)
        {
            List<EPlayerType> _totalPlayersAlive = _stateManager.Units.Select(_u => _u.playerType).Distinct().ToList();
            List<Unit> _playerHeroes = _stateManager.Units.Where(_unit => _unit.playerType == EPlayerType.Human).ToList();
            List<Monster> _bosses = new List<Monster>();
            foreach (Unit _monsterUnit in _stateManager.Units.Except(_playerHeroes).ToList())
            {
                if (!(_monsterUnit is Monster _monster)) continue;
                if (_monster.Type != EMonster.Boss) continue;
                _bosses.Add(_monster);
            }

            WinCondition = (_playerHeroes.Count > 0) && (_bosses.Count == 0);

            return _totalPlayersAlive.Count == 1 || _bosses.Count == 0;
        }
    }
}