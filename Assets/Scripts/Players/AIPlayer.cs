using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using _EventSystem.CustomEvents;
using _Instances;
using Cells;
using Grid;
using Grid.GridStates;
using Skills;
using Skills._Zone;
using Units;
using UnityEngine;
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
            public int MPCost;
            public int DirectTargetEnemy;
            public int DirectTargetAlly;
            public int ZoneTargetEnemy;
            public int ZoneTargetAlly;
            public int NearToEnemy;
            public int NearToAlly;
            public int NearToObject;
            public int CorruptedCell;
            public int DeBuffOnCell;
        }
        
        private Monster unit;
        private SkillInfo monsterSkill;
        private Dictionary<Cell, int> destinations = new Dictionary<Cell, int>();
        private Dictionary<Cell, Dictionary<Cell, int>> skillTargets = new Dictionary<Cell, Dictionary<Cell, int>>();

        [Header("Evaluation Value")] 
        [SerializeField] private EvaluationValues evaluationValues;

        [Header("Event Listener")]
        [SerializeField] private VoidEvent onSkillUsed;
        private bool skillUsed;
        
        [Header("Event Sender")]
        [SerializeField] private VoidEvent onEndTurn;
        [SerializeField] private VoidEvent onMonsterPlay;

        [SerializeField] private Gradient gradient;


        public override void Play(BattleStateManager stateManager)
        {
            // Check if it is the turn of IA Player
            if (stateManager.PlayingUnit.playerNumber == 0)
            {
                stateManager.BattleState = new BattleStateUnitSelected(stateManager, stateManager.PlayingUnit);
                return;
            }
            unit = (Monster) stateManager.PlayingUnit;
            monsterSkill = unit.GetComponentInChildren<SkillInfo>();
            
            unit.OnTurnStart();

            // Play
            onMonsterPlay.Raise();
            Execute(stateManager);
        }

        private void Execute(BattleStateManager _stateManager)
        {
            // Play
            StartCoroutine(Routine(_stateManager));
        }

        private IEnumerator Routine(BattleStateManager stateManager)
        {
            unit.isPlaying = true;

            bool canPlay = unit.BattleStats.AP > 0;
            bool canMove = unit.BattleStats.MP > 0;

            DestinationTarget best;
            if (unit.BattleStats.AP >= unit.Skill.Cost)
                best = Evaluate(unit.Skill, stateManager);
            else best = Evaluate(DataBase.Skill.MonsterAttack, stateManager);
            
            ColorTheFloor();
            yield return new WaitForSeconds(1);
            
            int loop = 0;
            // Loop where the Unit Use all his AP.
            while (canPlay && best.Target != null)
            {
                loop += 1;
                Debug.Log(loop);
                
                // Move
                List<Cell> path = unit.FindPath(stateManager.Cells, best.Destination);
                path.Sort((_cell, _cell1) => _cell.GetDistance(unit.Cell).CompareTo(_cell1.GetDistance(unit.Cell)));
                path.Reverse();
                unit.Move(best.Destination, path);

                yield return new WaitUntil(() => !unit.IsMoving);
                yield return new WaitForSeconds(0.2f);
            
                // Use Skill
                onSkillUsed.EventListeners += SkillUsed;
                monsterSkill.UseSkill(best.Target);

                yield return new WaitUntil(() => skillUsed);
                yield return new WaitForSeconds(0.2f);
                
                UnColor(stateManager);
                
                // Check Loop Conditions
                canPlay = (int) unit.BattleStats.AP > 0;
                canMove = unit.BattleStats.MP > 0;
                
                skillUsed = false;
                onSkillUsed.EventListeners -= SkillUsed;

                if (unit.BattleStats.AP >= unit.Skill.Cost)
                    best = Evaluate(unit.Skill, stateManager);
                else best = Evaluate(DataBase.Skill.MonsterAttack, stateManager);
                ColorTheFloor();
                yield return new WaitForSeconds(1);
            }

            // Move one Last Time at the best Place
            if (canMove)
            {
                // Move
                List<Cell> path = unit.FindPath(stateManager.Cells, best.Destination);
                path.Sort((_cell, _cell1) => _cell.GetDistance(unit.Cell).CompareTo(_cell1.GetDistance(unit.Cell)));
                unit.Move(best.Destination, path);

                yield return new WaitUntil(() => !unit.IsMoving);
            }

            
            // End Turn
            monsterSkill.UpdateSkill(unit.Skill, unit);
            unit.isPlaying = false;
            
            yield return new WaitForSeconds(1);
            
            onEndTurn.Raise();
        }

        private void ColorTheFloor()
        {
            // Color the Floor !!
            int minValue = destinations.Values.Min();
            int maxValue = destinations.Values.Max();
            foreach (Cell _cell in destinations.Keys)
            {
                _cell.MarkAsValue(gradient, destinations[_cell] - minValue, maxValue - minValue);
            }
        }

        private void UnColor(BattleStateManager stateManager)
        {
            foreach (Cell _cell in stateManager.Cells)
            {
                _cell.UnMark();
            }
        }

        private void SkillUsed(Void empty)
        {
            skillUsed = true;
        }

        #region Evaluation

        private DestinationTarget Evaluate(SkillSO _skill, BattleStateManager stateManager)
        {
            // Reset Dictionaries
            destinations = new Dictionary<Cell, int>();
            skillTargets = new Dictionary<Cell, Dictionary<Cell, int>>();
            
            // Get all the Cell to evaluate
            destinations.Add(unit.Cell, 0);
            skillTargets.Add(unit.Cell, new Dictionary<Cell, int>());
            foreach (Cell availableCell in unit.GetAvailableDestinations(stateManager.Cells))
            {
                destinations.Add(availableCell, 0);
                skillTargets.Add(availableCell, new Dictionary<Cell, int>());
            }
            
            // Set the Skill to Evaluate
            monsterSkill.UpdateSkill(_skill, unit);
            
            // If the MonsterSkill have Zone Get all TargetCell to evaluate
            if (monsterSkill.Range.ZoneType != EZone.Self || monsterSkill.Range.Radius > 0)
            {
                foreach (Cell destination in destinations.Keys)
                {
                    List<Cell> inRange = new List<Cell>();
                    inRange.AddRange(monsterSkill.Range.NeedView ? Zone.CellsInView(monsterSkill, destination) : Zone.CellsInRange(monsterSkill, destination));
                    foreach (Cell _cellInRange in inRange)
                    {
                        skillTargets[destination].Add(_cellInRange, 0);
                    }
                }
            }
            
            // Evaluate The Destinations
            EvaluateCells(stateManager);
            
            // Evaluate the SkillUse Potential
            if (monsterSkill.Range.ZoneType == EZone.Self || monsterSkill.Range.Radius < 1)
                EvaluateDirectTarget(stateManager, monsterSkill);
            else EvaluateZoneTarget(monsterSkill);
            
            // Get the Best Move
            DestinationTarget Best = GetBestDestinationTarget();
            
            // If the Best Target is Null Evaluate with the Monster Basic Attack
            if (Best.Target == null)
            {
                monsterSkill.UpdateSkill(DataBase.Skill.MonsterAttack, unit);
                if (monsterSkill.Range.ZoneType == EZone.Self || monsterSkill.Range.Radius < 1)
                    EvaluateDirectTarget(stateManager, monsterSkill);
                else EvaluateZoneTarget(monsterSkill);
            }
            
            // If the Best Target is Still Null Evaluate the distance to the nearest enemy
            if (Best.Target == null)
                EvaluateDistanceToEnemy(stateManager);
            
            // Get the Best Move After Check
            Best = GetBestDestinationTarget();
            
            return Best;
        }
        
        private void EvaluateDistanceToEnemy(BattleStateManager stateManager)
        {
            // Find the nearest Enemy
            List<Unit> enemies = new List<Unit>(stateManager.Units.Where(u => u.playerNumber != unit.playerNumber));
            enemies.Sort((u, u2) => u.Cell.GetDistance(unit.Cell).CompareTo(u2.Cell.GetDistance(unit.Cell)));

            List<Cell> destinationsKeys = new List<Cell>(destinations.Keys);
            foreach (Cell _cell in destinationsKeys)
            {
                int point = (int)(evaluationValues.NearToEnemy * 10f / _cell.GetDistance(enemies[0].Cell));
                destinations[_cell] += point;
            }
            
        }
        
        private void EvaluateCells(BattleStateManager stateManager)
        {
            // Evaluate Distance from playing Unit
            int MP = (int)unit.BattleStats.MP * evaluationValues.MPCost;
            List<Cell> destinationsKeys = new List<Cell>(destinations.Keys);
            foreach (Cell _cell in destinationsKeys)
            {
                int Point = MP - unit.FindPath(stateManager.Cells, _cell).Count * evaluationValues.MPCost;
                destinations[_cell] += Point;
            }
            
            // Evaluate Neighbours
            foreach (Cell _cell in destinationsKeys)
            {
                int Neighbours = 0;
                foreach (Cell c in _cell.GetNeighbours(stateManager.Cells))
                {
                    if (c.CurrentGridObject != null) Neighbours += evaluationValues.NearToObject;
                    if (c.CurrentUnit == null) continue;
                    if (c.CurrentUnit.playerNumber != unit.playerNumber) Neighbours += evaluationValues.NearToEnemy;
                    if (c.CurrentUnit.playerNumber == unit.playerNumber) Neighbours += evaluationValues.NearToAlly;
                }
                
                destinations[_cell] += Neighbours;
            }
            
            // Evaluate Corruption
            foreach (Cell _cell in destinationsKeys)
            {
                if (_cell.isCorrupted) destinations[_cell] += evaluationValues.CorruptedCell;
                foreach (Cell c in _cell.GetNeighbours(stateManager.Cells))
                {
                    if (c.isCorrupted) destinations[_cell] += evaluationValues.CorruptedCell / 2;
                }
            }
            
            // Evaluate Buffs on Floor
            foreach (Cell _cell in destinationsKeys)
            {
                if (_cell.Buffs.Count >= 0) destinations[_cell] += evaluationValues.DeBuffOnCell;
            }
        }

        private void EvaluateDirectTarget(BattleStateManager stateManager, SkillInfo skill)
        {
            List<Unit> Enemies = stateManager.Units.Where(u => u.playerNumber != unit.playerNumber).ToList();
            List<Unit> Allies = stateManager.Units.Except(Enemies).ToList();
            foreach (Unit _unit in stateManager.Units)
            {
                List<Cell> inRange = new List<Cell>();
                inRange.AddRange(skill.Range.NeedView ? Zone.CellsInView(skill, _unit.Cell) : Zone.CellsInRange(skill, _unit.Cell));
                
                foreach (Cell _cell in inRange.Where(_cell => destinations.ContainsKey(_cell)))
                {
                    if(Enemies.Contains(_unit))
                        destinations[_cell] += evaluationValues.DirectTargetEnemy;
                    if (Allies.Contains(_unit))
                        destinations[_cell] += evaluationValues.DirectTargetAlly;
                }
            }
        }

        private void EvaluateZoneTarget(SkillInfo skill)
        {
            List<Cell> destinationsKeys = new List<Cell>(destinations.Keys);
            foreach (Cell destination in destinationsKeys)
            {
                List<Cell> skillTargetsKeys = new List<Cell>(skillTargets[destination].Keys);
                foreach (Cell _cellInRange in skillTargetsKeys)
                {
                    List<Unit> affected = new List<Unit>(Zone.GetUnitsAffected(skill, _cellInRange));
                    if (affected.Count != 0)
                    {
                        foreach (Unit _unit in affected)
                        {
                            if (_unit.playerNumber == unit.playerNumber)
                                skillTargets[destination][_cellInRange] += evaluationValues.ZoneTargetAlly;
                            else
                                skillTargets[destination][_cellInRange] += evaluationValues.ZoneTargetEnemy;
                        }
                    }
                }

                if (skillTargets[destination].Count != 0)
                {
                    int maxValue = skillTargets[destination].Values.Max();
                    destinations[destination] += maxValue;
                }
            }
        }

        private DestinationTarget GetBestDestinationTarget()
        {
            Cell destinationOfMaxValue = unit.Cell;
            
            if (destinations.Count != 0)
                destinationOfMaxValue = destinations.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            
            Cell targetOfMaxValue = null;
            if (monsterSkill.Range.ZoneType != EZone.Self || monsterSkill.Range.Radius > 0)
            {
                if (skillTargets[destinationOfMaxValue].Count != 0)
                {
                    if (skillTargets[destinationOfMaxValue].Values.Max() > 0)
                        targetOfMaxValue = skillTargets[destinationOfMaxValue]
                            .Aggregate((x, y) => x.Value > y.Value ? x : y)
                            .Key;
                }
            }
            else
            {
                List<Unit> Targets = new List<Unit>();
                foreach (Cell _cell in monsterSkill.GetRangeFrom(destinationOfMaxValue))
                {
                    if(Zone.GetUnitAffected(_cell, monsterSkill) != null && Zone.GetUnitAffected(_cell, monsterSkill).playerNumber != unit.playerNumber)
                        Targets.Add(Zone.GetUnitAffected(_cell, monsterSkill));
                }

                Targets.Sort((u, u2) => (u.BattleStats.HP+u.BattleStats.Shield).CompareTo(u2.BattleStats.HP+u2.BattleStats.Shield));
                if (Targets.Count != 0)
                    targetOfMaxValue = Targets[0].Cell;
            }

            return new DestinationTarget {Destination = destinationOfMaxValue, Target = targetOfMaxValue};
        }

        #endregion
    }
}