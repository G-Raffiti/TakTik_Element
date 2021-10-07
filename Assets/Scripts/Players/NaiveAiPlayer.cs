using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Extension;
using _Instances;
using Cells;
using Grid;
using Grid.GridStates;
using Units;
using UnityEngine;

namespace Players
{
    /// <summary>
    /// Simple implementation of AI for the game.
    /// </summary>
    public class NaiveAiPlayer : Player
    {
        private BattleStateManager cellGrid;
        private System.Random rnd;
        private Monster unit;
        bool canAttack = true;

        public NaiveAiPlayer()
        {
            rnd = new System.Random();
        }

        public override void Play(BattleStateManager _cellGrid)
        {
            canAttack = true;
            _cellGrid.BattleState = new BattleStateBlockInput(_cellGrid);
            cellGrid = _cellGrid;

            if (cellGrid.PlayingUnit.playerNumber == 0)
            {
                cellGrid.BattleState = new BattleStateUnitSelected(cellGrid, cellGrid.PlayingUnit);
                return;
            }
            
            unit = (Monster)_cellGrid.PlayingUnit;
            unit.OnTurnStart();
            unit.isPlaying = true;
            StartCoroutine(Play(unit));

            //Coroutine is necessary to allow Unity to run updates on other objects (like UI).
            //Implementing this with threads would require a lot of modifications in other classes, as Unity API is not thread safe.
        }

        private IEnumerator Attack(List<Unit> _enemyUnits, List<Unit> _myUnits,Monster _unit)
        {
            List<Unit> _unitsInRange = new List<Unit>();

            while (canAttack && unit.BattleStats.AP > 0)
            {
                _unitsInRange = new List<Unit>();
                foreach (Unit _enemyUnit in _enemyUnits)
                {
                    if (_unit.IsUnitTargetable(_enemyUnit, _unit.Cell))
                    {
                        _unitsInRange.Add(_enemyUnit);
                    }
                }//Looking for enemies that are in skill range.
                
                yield return new WaitForSeconds(2f);
                
                if (_unitsInRange.Count != 0)
                {
                    int _index = rnd.Next(0, _unitsInRange.Count);
                    _unit.UseSkill(_unitsInRange[_index], _myUnits);
                    _unit.BattleStats.AP--;
                    yield return new WaitForSeconds(2f);
                }//If there is an enemy in range, use the Skill to hit him.
                
                else
                {
                    foreach (Unit _enemyUnit in _enemyUnits)
                    {
                        if (_unit.IsUnitAttackable(_enemyUnit, _unit.Cell))
                        {
                            _unitsInRange.Add(_enemyUnit);
                        }
                    }
                }//Looking for enemies that are in attack range.
                
                if (_unitsInRange.Count != 0)
                {
                    int _index = rnd.Next(0, _unitsInRange.Count);
                    _unit.Attack(_unitsInRange[_index]);
                    _unit.BattleStats.AP--;
                    yield return new WaitForSeconds(2f);
                }//If there is an enemy in range, attack it.

                canAttack = _unitsInRange.Count != 0;
            }

            canAttack = false;
        }
        private IEnumerator Play(Monster _unit)
        {
            BattleStateManager.instance.BlockInputs();
            
            List<Unit> _myUnits = cellGrid.Units.FindAll(u => u.playerNumber.Equals(playerNumber)).ToList();
            List<Unit> _enemyUnits = cellGrid.Units.Except(_myUnits).ToList();

            DataBase.RunCoroutine(Attack(_enemyUnits, _myUnits, _unit));
            while (canAttack)
            {
                yield return null;
            }
            
            List<Cell> _potentialDestinations = new List<Cell>();

            foreach (Unit _enemyUnit in _enemyUnits)
            {
                _potentialDestinations.AddRange(cellGrid.Cells.FindAll(c => _unit.IsCellMovableTo(c) && _unit.IsUnitTargetable(_enemyUnit, c)));
            }//Making a list of cells that the unit can attack from.

            List<Cell> _notInRange = _potentialDestinations.FindAll(c => c.GetDistance(_unit.Cell) > _unit.BattleStats.MP);
            _potentialDestinations = _potentialDestinations.Except(_notInRange).ToList();

            if (_potentialDestinations.Count == 0 && _notInRange.Count != 0)
            {
                _notInRange.Sort((_cell, _cell1) =>
                    _cell.GetDistance(_unit.Cell).CompareTo(_cell1.GetDistance(_unit.Cell)));
                _potentialDestinations.Add(_notInRange[0]);
            }

            _potentialDestinations = _potentialDestinations.OrderBy(h => rnd.Next()).ToList();
            List<Cell> _shortestPath = null;
            foreach (Cell _potentialDestination in _potentialDestinations)
            {
                List<Cell> _path = _unit.FindPath(cellGrid.Cells, _potentialDestination);
                if ((_shortestPath == null && _path.Sum(h => h.MovementCost) > 0) || _shortestPath != null && (_path.Sum(h => h.MovementCost) < _shortestPath.Sum(h => h.MovementCost) && _path.Sum(h => h.MovementCost) > 0))
                    _shortestPath = _path;

                float _pathCost = _path.Sum(h => h.MovementCost);
                if (_pathCost > 0 && _pathCost <= _unit.BattleStats.MP)
                {
                    _unit.Move(_potentialDestination, _path);
                    while (_unit.IsMoving)
                        yield return 0;
                    _shortestPath = null;
                    break;
                }
                yield return new WaitForSeconds(2f);
            }//If there is a path to any cell that the unit can attack from, move there.

            if (_shortestPath != null)
            {
                foreach (Cell _potentialDestination in _shortestPath.Intersect(_unit.GetAvailableDestinations(cellGrid.Cells)).OrderByDescending(h => h.GetDistance(_unit.Cell)))
                {
                    List<Cell> _path = _unit.FindPath(cellGrid.Cells, _potentialDestination);
                    float _pathCost = _path.Sum(h => h.MovementCost);
                    if (_pathCost > 0 && _pathCost <= _unit.BattleStats.MP)
                    {
                        _unit.Move(_potentialDestination, _path);
                        while (_unit.IsMoving)
                            yield return 0;
                        break;
                    }
                    yield return new WaitForSeconds(2f);
                }
            }//If the path cost is greater than unit movement points, move as far as possible.

            DataBase.RunCoroutine(Attack(_enemyUnits, _myUnits, _unit));
            while (canAttack)
            {
                yield return null;
            }// Try to attack or Use the Skill on enemy

            if (_unit.BattleStats.MP > 0)
            {
                List<Cell> randomDestination = new List<Cell>(_unit.GetAvailableDestinations(cellGrid.Cells));
                randomDestination.Shuffle();
                if (randomDestination.Count > 0)
                    _unit.Move(randomDestination[0], _unit.cachedPaths[randomDestination[0]]);
            }

            if (_unit.BattleStats.AP > 0)
            {
                DataBase.RunCoroutine(Attack(_enemyUnits, _myUnits, _unit));
                while (canAttack)
                {
                    yield return null;
                }// Try to attack or Use the Skill on enemy
            }
            
            cellGrid.BlockInputs();

            unit.isPlaying = false;
            cellGrid.EndTurn();
                
        }
    }
}