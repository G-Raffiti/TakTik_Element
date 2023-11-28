using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _EventSystem.CustomEvents;
using _Instances;
using Buffs;
using Cells;
using Skills;
using Skills._Zone;
using StateMachine;
using StateMachine.GridStates;
using Units;
using UnityEngine;
using UnityEngine.Serialization;
using Void = _EventSystem.CustomEvents.Void;

namespace Players
{
    public class AIPlayer : Player
    {
        private struct DestinationTarget
        {
            public Cell Destination;
            public Cell Target;
        }

        [Serializable]
        public struct EvaluationValues
        {
            [FormerlySerializedAs("MPCost")]
            public int mpCost;
            [FormerlySerializedAs("DirectTargetEnemy")]
            public int directTargetEnemy;
            [FormerlySerializedAs("DirectTargetAlly")]
            public int directTargetAlly;
            [FormerlySerializedAs("ZoneTargetEnemy")]
            public int zoneTargetEnemy;
            [FormerlySerializedAs("ZoneTargetAlly")]
            public int zoneTargetAlly;
            [FormerlySerializedAs("NearToEnemy")]
            public int nearToEnemy;
            [FormerlySerializedAs("NearToAlly")]
            public int nearToAlly;
            [FormerlySerializedAs("NearToObject")]
            public int nearToObject;
            [FormerlySerializedAs("CorruptedCell")]
            public int corruptedCell;
            [FormerlySerializedAs("DeBuffOnCell")]
            public int deBuffOnCell;
            [FormerlySerializedAs("BuffOnCell")]
            public int buffOnCell;
        }
        
        private Monster monster;
        private static SkillInfo _aiSkillInfo;
        private static Dictionary<Cell, int> _destinations = new Dictionary<Cell, int>();
        private static Dictionary<Cell, Dictionary<Cell, int>> _skillTargets = new Dictionary<Cell, Dictionary<Cell, int>>();

        private static EvaluationValues _evaluationValues;

        [Header("Event Listener")]
        [SerializeField] private VoidEvent onSkillUsed;
        private static bool _skillUsed;
        
        [Header("Event Sender")]
        [SerializeField] private VoidEvent onEndTurn;
        [SerializeField] private VoidEvent onMonsterPlay;


        public override void Play(BattleStateManager _stateManager)
        {
            // Check if it is the turn of IA Player
            if (_stateManager.PlayingUnit.playerType == EPlayerType.Human)
            {
                _stateManager.BattleState = new BattleStateUnitSelected(_stateManager, _stateManager.PlayingUnit);
                return;
            }
            monster = (Monster) _stateManager.PlayingUnit;
            _aiSkillInfo = monster.GetComponentInChildren<SkillInfo>();
            _evaluationValues = monster.Archetype.Evaluations;
            
            monster.StartTurn();

            // Play
            onMonsterPlay.Raise();
            Debug.LogWarning("Unit :" + _stateManager.PlayingUnit.unitName + "Skill :" + monster.MonsterSkill.BaseSkill.Name + "start");
            Execute(_stateManager);
        }

        private void Execute(BattleStateManager _stateManager)
        {
            // Play
            StartCoroutine(Routine(_stateManager));
        }

        private string TestPrint(DestinationTarget _test)
        {
            string _dest = _test.Destination != null ? _test.Destination.OffsetCoord.ToString() : "None";
            string _targ = _test.Target != null ? _test.Target.OffsetCoord.ToString() : "None";
            return $"Destination: {_dest}, Target: {_targ}";
        }
        private IEnumerator Routine(BattleStateManager _stateManager)
        {
            monster.isPlaying = true;

            bool _canPlay = monster.battleStats.ap > 0;
            bool _canMove = monster.battleStats.mp > 0;

            Debug.LogWarning("1rst Evaluation :");
            DestinationTarget _best = Evaluate(monster, _stateManager);

            yield return new WaitForSeconds(1);

            int _loop = 0;
            // Loop where the Unit Use all his AP.
            while (_canPlay && _best.Target != null)
            {
                _loop += 1;
                Debug.LogWarning($"Loop Start\nLoop number : {_loop}\n{monster.unitName} AP = {monster.battleStats.ap} MP = {monster.battleStats.mp}\n{TestPrint(_best)}");
                
                // Move
                if (_best.Destination != monster.Cell)
                {
                    List<Cell> _path = monster.FindPath(_stateManager.Cells, _best.Destination);
                    _path.Sort((_cell, _cell1) => _cell.GetDistance(monster.Cell).CompareTo(_cell1.GetDistance(monster.Cell)));
                    _path.Reverse();
                    monster.Move(_best.Destination, _path);
                }

                Debug.LogWarning($"Loop number : {_loop}\nMonster is Moving = {monster.IsMoving}");
                yield return new WaitUntil(() => !monster.IsMoving);
                Debug.LogWarning($"Loop number : {_loop}\nMonster is Moving = {monster.IsMoving}");
                yield return new WaitForSeconds(0.2f);
            
                // Use Skill
                onSkillUsed.EventListeners += SkillUsed;
                _aiSkillInfo.UseSkill(_best.Target);
                
                yield return new WaitUntil(() => _skillUsed);
                Debug.LogWarning($"Loop number : {_loop}\nMonster used a Skill");
                yield return new WaitForSeconds(0.2f);
                
                // Check Loop Conditions
                Debug.LogWarning($"Check Condition\nLoop number : {_loop}\n{monster.unitName} AP = {monster.battleStats.ap} MP = {monster.battleStats.mp}\n{TestPrint(_best)}");
                
                _canPlay = (int) monster.battleStats.ap > 0;
                if (monster.MonsterSkill.Cost == 0)
                    _canPlay = _loop <= BattleStage.Stage + 1;
                _canMove = monster.battleStats.mp > 0;
                
                _skillUsed = false;
                onSkillUsed.EventListeners -= SkillUsed;

                _best = Evaluate(monster, _stateManager);
                
                yield return new WaitForSeconds(1);
                Debug.LogWarning($"Check Done\nLoop number : {_loop}\n{monster.unitName} AP = {monster.battleStats.ap} MP = {monster.battleStats.mp}\n{TestPrint(_best)}\nCan Play = {_canPlay}, Can Move = {_canMove}");
            }

            // Move one Last Time at the best Place
            if (_canMove && _best.Destination != monster.Cell)
            {
                // Move
                List<Cell> _path = monster.FindPath(_stateManager.Cells, _best.Destination);
                _path.Sort((_cell, _cell1) => _cell.GetDistance(monster.Cell).CompareTo(_cell1.GetDistance(monster.Cell)));
                monster.Move(_best.Destination, _path);

                yield return new WaitUntil(() => !monster.IsMoving);
            }

            
            // End Turn
            monster.isPlaying = false;
            
            yield return new WaitForSeconds(1);
            
            onEndTurn.Raise();
        }

        private void SkillUsed(Void _empty)
        {
            _skillUsed = true;
        }

        #region Evaluation

        private static DestinationTarget Evaluate(Monster _monster, BattleStateManager _stateManager)
        {
            // Reset Dictionaries
            _destinations = new Dictionary<Cell, int>();
            _skillTargets = new Dictionary<Cell, Dictionary<Cell, int>>();
            
            // Get all the Cell to evaluate
            _destinations.Add(_monster.Cell, 0);
            _skillTargets.Add(_monster.Cell, new Dictionary<Cell, int>());
            foreach (Cell _availableCell in _monster.GetAvailableDestinations(_stateManager.Cells))
            {
                if (_destinations.Keys.Contains(_availableCell)) continue;
                _destinations.Add(_availableCell, 0);
                _skillTargets.Add(_availableCell, new Dictionary<Cell, int>());
            }
            
            // Set the Skill to Evaluate
            _aiSkillInfo.skill = _monster.MonsterSkill;
            
            // If the MonsterSkill have Zone Get all TargetCell to evaluate
            if (_aiSkillInfo.skill.GridRange.zoneType != EZone.Self || _aiSkillInfo.skill.GridRange.radius > 0)
            {
                foreach (Cell _destination in _destinations.Keys)
                {
                    List<Cell> _inRange = 
                        _aiSkillInfo.skill.GridRange.needView ? 
                            Zone.CellsInView(_aiSkillInfo.skill, _destination) : 
                            Zone.CellsInRange(_aiSkillInfo.skill, _destination);
                    foreach (Cell _cellInRange in _inRange)
                    {
                        _skillTargets[_destination].Add(_cellInRange, 0);
                    }
                }
            }
            
            // Evaluate The Destinations
            EvaluateCells(_monster, _stateManager);
            
            // Evaluate the SkillUse Potential
            if (_aiSkillInfo.skill.GridRange.zoneType == EZone.Self || _aiSkillInfo.skill.GridRange.radius < 1)
                EvaluateDirectTarget(_stateManager, _aiSkillInfo);
            else EvaluateZoneTarget(_aiSkillInfo);
            
            // Get the Best Move
            DestinationTarget _best = GetBestDestinationTarget(_monster);

            // If the Best Target is Null Evaluate the distance to the nearest enemy
            if (_best.Target == null)
            {
                EvaluateDistanceToAlly(_monster, _stateManager);
                EvaluateDistanceToEnemy(_monster, _stateManager);
            }
            
            // Get the Best Move After Check
            _best = GetBestDestinationTarget(_monster);
            
            return _best;
        }
        
        private static void EvaluateDistanceToEnemy(Unit _unit, BattleStateManager _stateManager)
        {
            // Find the nearest Enemy
            List<Unit> _enemies = new List<Unit>(_stateManager.Units.Where(_u => _u.playerType != _unit.playerType));
            if (_enemies.Count == 0) return;
            _enemies.Sort((_u, _u2) => _u.Cell.GetDistance(_unit.Cell).CompareTo(_u2.Cell.GetDistance(_unit.Cell)));

            List<Cell> _destinationsKeys = new List<Cell>(_destinations.Keys);
            foreach (Cell _cell in _destinationsKeys)
            {
                int _point = (int)(_evaluationValues.nearToEnemy * 10f / _cell.GetDistance(_enemies[0].Cell));
                _destinations[_cell] += _point;
            }
        }
        
        private static void EvaluateDistanceToAlly(Unit _unit, BattleStateManager _stateManager)
        {
            // Find the nearest Ally
            List<Unit> _allies = new List<Unit>(_stateManager.Units.Where(_u => _u.playerType == _unit.playerType));
            _allies.Sort((_u, _u2) => _u.Cell.GetDistance(_unit.Cell).CompareTo(_u2.Cell.GetDistance(_unit.Cell)));

            List<Cell> _destinationsKeys = new List<Cell>(_destinations.Keys);
            foreach (Cell _cell in _destinationsKeys)
            {
                int _point = (int)(_evaluationValues.nearToAlly * 10f / _cell.GetDistance(_allies[0].Cell));
                _destinations[_cell] += _point;
            }
        }
        
        private static void EvaluateCells(Unit _unit, BattleStateManager _stateManager)
        {
            
            List<Cell> _destinationsKeys = new List<Cell>(_destinations.Keys);
            /*
            // Evaluate Distance from playing Unit
            int MP = (int)_unit.BattleStats.MP * evaluationValues.MPCost;
            foreach (Cell _cell in destinationsKeys)
            {
                int Point = MP - _unit.FindPath(stateManager.Cells, _cell).Count * evaluationValues.MPCost;
                destinations[_cell] += Point;
            }*/
            
            // Evaluate Neighbours
            foreach (Cell _cell in _destinationsKeys)
            {
                int _neighbours = 0;
                foreach (Cell _c in _cell.GetNeighbours(_stateManager.Cells))
                {
                    if (_c.CurrentGridObject != null) _neighbours += _evaluationValues.nearToObject;
                    if (_c.CurrentUnit == null) continue;
                    if (_c.CurrentUnit.playerType != _unit.playerType) _neighbours += _evaluationValues.nearToEnemy;
                    if (_c.CurrentUnit.playerType == _unit.playerType) _neighbours += _evaluationValues.nearToAlly;
                }
                
                _destinations[_cell] += _neighbours;
            }
            
            // Evaluate Corruption
            foreach (Cell _cell in _destinationsKeys)
            {
                if (_cell.IsCorrupted)
                {
                    _destinations[_cell] += _evaluationValues.corruptedCell;
                    
                    foreach (Cell _neighbour in _cell.GetNeighbours(_stateManager.Cells))
                    {
                        if(_destinationsKeys.Contains(_neighbour) && !_neighbour.IsCorrupted)
                            _destinations[_neighbour] += _evaluationValues.corruptedCell / 2;
                    }
                }
            }
            
            // Evaluate Buffs on Floor
            foreach (Cell _cell in _destinationsKeys)
            {
                int _debuffs = _cell.Buffs.Where(_b => _b.Effect.Type == EBuff.Debuff).ToList().Count;
                int _buffs = _cell.Buffs.Where(_b => _b.Effect.Type == EBuff.Buff).ToList().Count;
                
                if (_debuffs <= 0 && _buffs <= 0) continue;
                
                _destinations[_cell] += _debuffs * _evaluationValues.deBuffOnCell;
                _destinations[_cell] += _buffs * _evaluationValues.buffOnCell;
                
                foreach (Cell _c in _cell.GetNeighbours(_stateManager.Cells))
                {
                    if (_destinationsKeys.Contains(_c))
                        _destinations[_c] += _evaluationValues.deBuffOnCell / 2;
                }
            }
        }

        private static void EvaluateDirectTarget(BattleStateManager _stateManager, SkillInfo _skill)
        {
            List<Unit> _enemies = _stateManager.Units.Where(_u => _u.playerType != _skill.unit.playerType).ToList();
            List<Unit> _allies = _stateManager.Units.Except(_enemies).ToList();
            foreach (Unit _unit in _stateManager.Units)
            {
                List<Cell> _inRange = new List<Cell>();
                _inRange.AddRange(_skill.skill.GridRange.needView ? Zone.CellsInView(_skill.skill, _unit.Cell) : Zone.CellsInRange(_skill.skill, _unit.Cell));
                
                foreach (Cell _cell in _inRange.Where(_cell => _destinations.ContainsKey(_cell)))
                {
                    if(_enemies.Contains(_unit))
                        _destinations[_cell] += _evaluationValues.directTargetEnemy;
                    if (_allies.Contains(_unit))
                        _destinations[_cell] += _evaluationValues.directTargetAlly;
                }
            }
        }

        private static void EvaluateZoneTarget(SkillInfo _skill)
        {
            List<Cell> _destinationsKeys = new List<Cell>(_destinations.Keys);
            foreach (Cell _destination in _destinationsKeys)
            {
                List<Cell> _skillTargetsKeys = new List<Cell>(_skillTargets[_destination].Keys);
                foreach (Cell _cellInRange in _skillTargetsKeys)
                {
                    List<Unit> _affected = new List<Unit>(Zone.GetUnitsAffected(_skill, _cellInRange));
                    
                    if (_skill.GetZoneOfEffect(_cellInRange).Contains(_destination))
                    {
                        if (_skill.skill.Affect == EAffect.All || _skill.skill.Affect == EAffect.OnlyAlly || _skill.skill.Affect == EAffect.OnlySelf || _skill.skill.Affect == EAffect.OnlyUnits)
                            _affected.Add(_skill.unit);
                    }
                    
                    if (_affected.Count != 0)
                    {
                        foreach (Unit _unit in _affected)
                        {
                            if (_unit.playerType == _skill.unit.playerType)
                                _skillTargets[_destination][_cellInRange] += _evaluationValues.zoneTargetAlly;
                            else
                                _skillTargets[_destination][_cellInRange] += _evaluationValues.zoneTargetEnemy;
                        }
                    }
                }

                if (_skillTargets[_destination].Count != 0)
                {
                    int _maxValue = _skillTargets[_destination].Values.Max();
                    _destinations[_destination] += _maxValue;
                }
            }
        }

        private static DestinationTarget GetBestDestinationTarget(Unit _unit)
        {
            Cell _destinationOfMaxValue = _unit.Cell;
            
            if (_destinations.Count != 0)
                _destinationOfMaxValue = _destinations.Aggregate((_x, _y) => _x.Value > _y.Value ? _x : _y).Key;
            
            Cell _targetOfMaxValue = null;
            if (_aiSkillInfo.skill.GridRange.zoneType != EZone.Self || _aiSkillInfo.skill.GridRange.radius > 0)
            {
                if (_skillTargets[_destinationOfMaxValue].Count != 0)
                {
                    if (_skillTargets[_destinationOfMaxValue].Values.Max() > 0)
                        _targetOfMaxValue = _skillTargets[_destinationOfMaxValue]
                            .Aggregate((_x, _y) => _x.Value > _y.Value ? _x : _y)
                            .Key;
                }
            }
            else
            {
                List<Unit> _targets = new List<Unit>();
                foreach (Cell _cell in _aiSkillInfo.GetRangeFrom(_destinationOfMaxValue))
                {
                    if(Zone.GetUnitAffected(_cell, _aiSkillInfo) != null && Zone.GetUnitAffected(_cell, _aiSkillInfo).playerType != _unit.playerType)
                        _targets.Add(Zone.GetUnitAffected(_cell, _aiSkillInfo));
                }

                _targets.Sort((_u, _u2) => (_u.battleStats.hp+_u.battleStats.shield).CompareTo(_u2.battleStats.hp+_u2.battleStats.shield));
                if (_targets.Count != 0)
                    _targetOfMaxValue = _targets[0].Cell;
            }

            return new DestinationTarget {Destination = _destinationOfMaxValue, Target = _targetOfMaxValue};
        }

        #endregion
    }
}