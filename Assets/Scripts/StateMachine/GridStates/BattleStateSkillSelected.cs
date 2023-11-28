﻿using System.Collections.Generic;
using System.Linq;
using Cells;
using Skills;
using Skills._Zone;
using Units;
using UnityEngine;
using UnityEngine.EventSystems;

namespace StateMachine.GridStates
{
    internal class BattleStateSkillSelected : BattleState
    {
        private SkillInfo skill;
        private List<Cell> usable;
        private List<Cell> inRange;
        private Unit currentUnit;

        private List<Cell> currentPath;

        public BattleStateSkillSelected(BattleStateManager _stateManager, SkillInfo _skill) : base(_stateManager)
        {
            State = EBattleState.Skill;
            StateManager = _stateManager;
            skill = _skill;
            currentUnit = BattleStateManager.instance.PlayingUnit;
            inRange = new List<Cell>();
            usable = new List<Cell>();
            currentPath = new List<Cell>();
        }

        public override void OnStateEnter()
        {
            
            foreach (Cell _cell in StateManager.Cells)
            {
                _cell.UnMark();
            }
            
            usable.AddRange(skill.skill.GridRange.needView ? Zone.CellsInView(skill.skill, skill.unit.Cell) : Zone.CellsInRange(skill.skill, skill.unit.Cell));

            if (skill.skill.GridRange.needTarget || skill.skill.GridRange.needView)
            {
                inRange.AddRange(Zone.GetRange(skill.skill.GridRange, currentUnit.Cell));
                foreach (Cell _cell in inRange)
                {
                    _cell.MarkAsUnReachable();
                }
            }
            
            foreach (Cell _cell in usable)
            {
                _cell.MarkAsReachable();
            }

        }
        
        public override void OnStateExit()
        {
            foreach (Cell _cell in StateManager.Cells)
            {
                _cell.UnMark();
            }
            //currentUnit.OnUnitSelected();
        }
        
        
        public override void OnCellClicked(Cell _cell)
        {
            if (usable.Contains(_cell))
            {
                skill.UseSkill(_cell);
            }
            else StateManager.BattleState = new BattleStateUnitSelected(StateManager, currentUnit);
        }

        public override void OnCellSelected(Cell _targetCell)
        {
            if (usable.Contains(_targetCell))
            {
                foreach (Cell _cellInRadius in skill.GetZoneOfEffect(_targetCell))
                {
                    _cellInRadius.MarkAsPath();
                }
            }
            else
            {
                base.OnCellSelected(_targetCell);
            }
        }

        public override void OnCellDeselected(Cell _targetCell)
        {
            IEnumerable<Cell> _cellsNotInRange = StateManager.Cells.Except(usable);
            
            foreach (Cell _cell in _cellsNotInRange)
            {
                _cell.UnMark();
            }
            
            foreach (Cell _cell in inRange)
            {
                _cell.MarkAsUnReachable();
            }
            
            foreach (Cell _cell in usable)
            {
                _cell.MarkAsReachable();
            }
        }
    }
}