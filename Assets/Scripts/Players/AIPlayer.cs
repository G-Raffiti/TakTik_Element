﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _EventSystem.CustomEvents;
using _Instances;
using Cells;
using Skills;
using Skills._Zone;
using StateMachine;
using StateMachine.GridStates;
using StatusEffect;
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
            public int BuffOnCell;
        }
        
        private Monster monster;
        private static SkillInfo skillInfo;
        private static Dictionary<Cell, int> destinations = new Dictionary<Cell, int>();
        private static Dictionary<Cell, Dictionary<Cell, int>> skillTargets = new Dictionary<Cell, Dictionary<Cell, int>>();

        private static EvaluationValues evaluationValues;

        [Header("Event Listener")]
        [SerializeField] private VoidEvent onSkillUsed;
        private static bool skillUsed;
        
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
            monster = (Monster) stateManager.PlayingUnit;
            skillInfo = monster.GetComponentInChildren<SkillInfo>();
            evaluationValues = monster.Archetype.Evaluations;
            
            monster.OnTurnStart();

            // Play
            onMonsterPlay.Raise();
            Debug.LogWarning("Unit :" + stateManager.PlayingUnit.UnitName + "Skill :" + monster.monsterSkill.BaseSkill.Name + "start");
            Execute(stateManager);
        }

        private void Execute(BattleStateManager _stateManager)
        {
            // Play
            StartCoroutine(Routine(_stateManager));
        }

        private string TestPrint(DestinationTarget test)
        {
            string dest = test.Destination != null ? test.Destination.OffsetCoord.ToString() : "None";
            string targ = test.Target != null ? test.Target.OffsetCoord.ToString() : "None";
            return $"Destination: {dest}, Target: {targ}";
        }
        private IEnumerator Routine(BattleStateManager stateManager)
        {
            monster.isPlaying = true;

            bool canPlay = monster.BattleStats.AP > 0;
            bool canMove = monster.BattleStats.MP > 0;

            Debug.LogWarning("1rst Evaluation :");
            DestinationTarget best;
            if (monster.BattleStats.AP >= monster.monsterSkill.Cost)
                best = Evaluate(monster, monster.monsterSkill.BaseSkill, stateManager);
            else best = Evaluate(monster, DataBase.Skill.MonsterAttack, stateManager);
            
            //TODO : possibilité d'activé les couleur de des cells de l'IA via un menu Setting
            //ColorTheFloor();
            
            yield return new WaitForSeconds(1);

            int loop = 0;
            // Loop where the Unit Use all his AP.
            while (canPlay && best.Target != null)
            {
                loop += 1;
                Debug.LogWarning($"Loop Start\nLoop number : {loop}\n{monster.UnitName} AP = {monster.BattleStats.AP} MP = {monster.BattleStats.MP}\n{TestPrint(best)}");
                
                // Move
                if (best.Destination != monster.Cell)
                {
                    List<Cell> path = monster.FindPath(stateManager.Cells, best.Destination);
                    path.Sort((_cell, _cell1) => _cell.GetDistance(monster.Cell).CompareTo(_cell1.GetDistance(monster.Cell)));
                    path.Reverse();
                    monster.Move(best.Destination, path);
                }

                Debug.LogWarning($"Loop number : {loop}\nMonster is Moving = {monster.IsMoving}");
                yield return new WaitUntil(() => !monster.IsMoving);
                Debug.LogWarning($"Loop number : {loop}\nMonster is Moving = {monster.IsMoving}");
                yield return new WaitForSeconds(0.2f);
            
                // Use Skill
                onSkillUsed.EventListeners += SkillUsed;
                skillInfo.UseSkill(best.Target);
                
                yield return new WaitUntil(() => skillUsed);
                Debug.LogWarning($"Loop number : {loop}\nMonster used a Skill");
                yield return new WaitForSeconds(0.2f);
                
                UnColor(stateManager);
                
                // Check Loop Conditions
                Debug.LogWarning($"Check Condition\nLoop number : {loop}\n{monster.UnitName} AP = {monster.BattleStats.AP} MP = {monster.BattleStats.MP}\n{TestPrint(best)}");
                
                canPlay = (int) monster.BattleStats.AP > 0;
                if (monster.monsterSkill.Cost == 0)
                    canPlay = loop >= KeepBetweenScene.Stage + 1;
                canMove = monster.BattleStats.MP > 0;
                
                skillUsed = false;
                onSkillUsed.EventListeners -= SkillUsed;

                if (monster.BattleStats.AP >= monster.monsterSkill.Cost)
                    best = Evaluate(monster, monster.monsterSkill.BaseSkill, stateManager);
                else best = Evaluate(monster, DataBase.Skill.MonsterAttack, stateManager);
                
                //TODO : possibilité d'activé les couleur de des cells de l'IA via un menu Setting
                //ColorTheFloor();
                yield return new WaitForSeconds(1);
                Debug.LogWarning($"Check Done\nLoop number : {loop}\n{monster.UnitName} AP = {monster.BattleStats.AP} MP = {monster.BattleStats.MP}\n{TestPrint(best)}\nCan Play = {canPlay}, Can Move = {canMove}");
            }

            // Move one Last Time at the best Place
            if (canMove && best.Destination != monster.Cell)
            {
                // Move
                List<Cell> path = monster.FindPath(stateManager.Cells, best.Destination);
                path.Sort((_cell, _cell1) => _cell.GetDistance(monster.Cell).CompareTo(_cell1.GetDistance(monster.Cell)));
                monster.Move(best.Destination, path);

                yield return new WaitUntil(() => !monster.IsMoving);
            }

            
            // End Turn
            skillInfo.skill = monster.monsterSkill;
            monster.isPlaying = false;
            
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

        private static DestinationTarget Evaluate(Monster _monster, SkillSO _skill, BattleStateManager stateManager)
        {
            // Reset Dictionaries
            destinations = new Dictionary<Cell, int>();
            skillTargets = new Dictionary<Cell, Dictionary<Cell, int>>();
            
            // Get all the Cell to evaluate
            destinations.Add(_monster.Cell, 0);
            skillTargets.Add(_monster.Cell, new Dictionary<Cell, int>());
            foreach (Cell availableCell in _monster.GetAvailableDestinations(stateManager.Cells))
            {
                if (destinations.Keys.Contains(availableCell)) continue;
                destinations.Add(availableCell, 0);
                skillTargets.Add(availableCell, new Dictionary<Cell, int>());
            }
            
            // Set the Skill to Evaluate
            skillInfo.skill = Skill.CreateSkill(_skill, _monster.monsterSkill.Deck, _monster);
            
            // If the MonsterSkill have Zone Get all TargetCell to evaluate
            if (skillInfo.skill.Range.ZoneType != EZone.Self || skillInfo.skill.Range.Radius > 0)
            {
                foreach (Cell destination in destinations.Keys)
                {
                    List<Cell> inRange = new List<Cell>();
                    inRange.AddRange(skillInfo.skill.Range.NeedView ? Zone.CellsInView(skillInfo.skill, destination) : Zone.CellsInRange(skillInfo.skill, destination));
                    foreach (Cell _cellInRange in inRange)
                    {
                        skillTargets[destination].Add(_cellInRange, 0);
                    }
                }
            }
            
            // Evaluate The Destinations
            EvaluateCells(_monster, stateManager);
            
            // Evaluate the SkillUse Potential
            if (skillInfo.skill.Range.ZoneType == EZone.Self || skillInfo.skill.Range.Radius < 1)
                EvaluateDirectTarget(stateManager, skillInfo);
            else EvaluateZoneTarget(skillInfo);
            
            // Get the Best Move
            DestinationTarget Best = GetBestDestinationTarget(_monster);
            
            // If the Best Target is Null Evaluate with the Monster Basic Attack
            if (Best.Target == null)
            {
                skillInfo.skill = Skill.CreateSkill(DataBase.Skill.MonsterAttack, skillInfo.skill.Deck, _monster);
                if (skillInfo.skill.Range.ZoneType == EZone.Self || skillInfo.skill.Range.Radius < 1)
                    EvaluateDirectTarget(stateManager, skillInfo);
                else EvaluateZoneTarget(skillInfo);
                
                // Check Again for the Best Target
                Best = GetBestDestinationTarget(_monster);
            }

            // If the Best Target is Still Null Evaluate the distance to the nearest enemy
            if (Best.Target == null)
            {
                EvaluateDistanceToAlly(_monster, stateManager);
                EvaluateDistanceToEnemy(_monster, stateManager);
            }
            
            // Get the Best Move After Check
            Best = GetBestDestinationTarget(_monster);
            
            return Best;
        }
        
        private static void EvaluateDistanceToEnemy(Unit _unit, BattleStateManager stateManager)
        {
            // Find the nearest Enemy
            List<Unit> enemies = new List<Unit>(stateManager.Units.Where(u => u.playerNumber != _unit.playerNumber));
            if (enemies.Count == 0) return;
            enemies.Sort((u, u2) => u.Cell.GetDistance(_unit.Cell).CompareTo(u2.Cell.GetDistance(_unit.Cell)));

            List<Cell> destinationsKeys = new List<Cell>(destinations.Keys);
            foreach (Cell _cell in destinationsKeys)
            {
                int point = (int)(evaluationValues.NearToEnemy * 10f / _cell.GetDistance(enemies[0].Cell));
                destinations[_cell] += point;
            }
        }
        
        private static void EvaluateDistanceToAlly(Unit _unit, BattleStateManager stateManager)
        {
            // Find the nearest Ally
            List<Unit> allies = new List<Unit>(stateManager.Units.Where(u => u.playerNumber == _unit.playerNumber));
            allies.Sort((u, u2) => u.Cell.GetDistance(_unit.Cell).CompareTo(u2.Cell.GetDistance(_unit.Cell)));

            List<Cell> destinationsKeys = new List<Cell>(destinations.Keys);
            foreach (Cell _cell in destinationsKeys)
            {
                int point = (int)(evaluationValues.NearToAlly * 10f / _cell.GetDistance(allies[0].Cell));
                destinations[_cell] += point;
            }
        }
        
        private static void EvaluateCells(Unit _unit, BattleStateManager stateManager)
        {
            // Evaluate Distance from playing Unit
            int MP = (int)_unit.BattleStats.MP * evaluationValues.MPCost;
            List<Cell> destinationsKeys = new List<Cell>(destinations.Keys);
            foreach (Cell _cell in destinationsKeys)
            {
                int Point = MP - _unit.FindPath(stateManager.Cells, _cell).Count * evaluationValues.MPCost;
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
                    if (c.CurrentUnit.playerNumber != _unit.playerNumber) Neighbours += evaluationValues.NearToEnemy;
                    if (c.CurrentUnit.playerNumber == _unit.playerNumber) Neighbours += evaluationValues.NearToAlly;
                }
                
                destinations[_cell] += Neighbours;
            }
            
            // Evaluate Corruption
            foreach (Cell _cell in destinationsKeys)
            {
                if (_cell.isCorrupted)
                {
                    destinations[_cell] += evaluationValues.CorruptedCell;
                    
                    foreach (Cell neighbour in _cell.GetNeighbours(stateManager.Cells))
                    {
                        if(destinationsKeys.Contains(neighbour) && !neighbour.isCorrupted)
                            destinations[neighbour] += evaluationValues.CorruptedCell / 2;
                    }
                }
            }
            
            // Evaluate Buffs on Floor
            foreach (Cell _cell in destinationsKeys)
            {
                int debuffs = _cell.Buffs.Where(b => b.Effect.Type == EBuff.Debuff).ToList().Count;
                int buffs = _cell.Buffs.Where(b => b.Effect.Type == EBuff.Buff).ToList().Count;
                
                if (debuffs <= 0 && buffs <= 0) continue;
                
                destinations[_cell] += debuffs * evaluationValues.DeBuffOnCell;
                destinations[_cell] += buffs * evaluationValues.BuffOnCell;
                
                foreach (Cell c in _cell.GetNeighbours(stateManager.Cells))
                {
                    if (destinationsKeys.Contains(c))
                        destinations[c] += evaluationValues.DeBuffOnCell / 2;
                }
            }
        }

        private static void EvaluateDirectTarget(BattleStateManager stateManager, SkillInfo skill)
        {
            List<Unit> Enemies = stateManager.Units.Where(u => u.playerNumber != skill.Unit.playerNumber).ToList();
            List<Unit> Allies = stateManager.Units.Except(Enemies).ToList();
            foreach (Unit _unit in stateManager.Units)
            {
                List<Cell> inRange = new List<Cell>();
                inRange.AddRange(skill.skill.Range.NeedView ? Zone.CellsInView(skill.skill, _unit.Cell) : Zone.CellsInRange(skill.skill, _unit.Cell));
                
                foreach (Cell _cell in inRange.Where(_cell => destinations.ContainsKey(_cell)))
                {
                    if(Enemies.Contains(_unit))
                        destinations[_cell] += evaluationValues.DirectTargetEnemy;
                    if (Allies.Contains(_unit))
                        destinations[_cell] += evaluationValues.DirectTargetAlly;
                }
            }
        }

        private static void EvaluateZoneTarget(SkillInfo skill)
        {
            List<Cell> destinationsKeys = new List<Cell>(destinations.Keys);
            foreach (Cell destination in destinationsKeys)
            {
                List<Cell> skillTargetsKeys = new List<Cell>(skillTargets[destination].Keys);
                foreach (Cell _cellInRange in skillTargetsKeys)
                {
                    List<Unit> affected = new List<Unit>(Zone.GetUnitsAffected(skill, _cellInRange));
                    
                    if (skill.GetZoneOfEffect(_cellInRange).Contains(destination))
                    {
                        if (skill.skill.Affect == EAffect.All || skill.skill.Affect == EAffect.OnlyAlly || skill.skill.Affect == EAffect.OnlySelf || skill.skill.Affect == EAffect.OnlyUnits)
                            affected.Add(skill.Unit);
                    }
                    
                    if (affected.Count != 0)
                    {
                        foreach (Unit _unit in affected)
                        {
                            if (_unit.playerNumber == skill.Unit.playerNumber)
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

        private static DestinationTarget GetBestDestinationTarget(Unit _unit)
        {
            Cell destinationOfMaxValue = _unit.Cell;
            
            if (destinations.Count != 0)
                destinationOfMaxValue = destinations.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            
            Cell targetOfMaxValue = null;
            if (skillInfo.skill.Range.ZoneType != EZone.Self || skillInfo.skill.Range.Radius > 0)
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
                foreach (Cell _cell in skillInfo.GetRangeFrom(destinationOfMaxValue))
                {
                    if(Zone.GetUnitAffected(_cell, skillInfo) != null && Zone.GetUnitAffected(_cell, skillInfo).playerNumber != _unit.playerNumber)
                        Targets.Add(Zone.GetUnitAffected(_cell, skillInfo));
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