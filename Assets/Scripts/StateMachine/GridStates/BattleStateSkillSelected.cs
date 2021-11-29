using System.Collections.Generic;
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
            
            usable.AddRange(skill.skill.Range.NeedView ? Zone.CellsInView(skill.skill, skill.Unit.Cell) : Zone.CellsInRange(skill.skill, skill.Unit.Cell));

            if (skill.skill.Range.NeedTarget || skill.skill.Range.NeedView)
            {
                inRange.AddRange(Zone.GetRange(skill.skill.Range, currentUnit.Cell));
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
            currentUnit.OnUnitSelected();
        }
        
        
        public override void OnCellClicked(Cell _cell)
        {
            if (usable.Contains(_cell))
            {
                skill.UseSkill(_cell);
            }
            else StateManager.BattleState = new BattleStateUnitSelected(StateManager, currentUnit);
        }

        public override void OnCellSelected(Cell cell)
        {
            if (!EventSystem.current.IsPointerOverGameObject() &&
                (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
            {
                if (cell.CurrentGridObject == null)
                    TooltipOn.Raise((TileIsometric)cell);
                else TooltipOn.Raise(cell.CurrentGridObject.GridObjectSO);
            }
            if (usable.Contains(cell))
            {
                foreach (Cell _cellInRadius in skill.GetZoneOfEffect(cell))
                {
                    _cellInRadius.MarkAsPath();
                }
            }
            else
            {
                base.OnCellSelected(cell);
            }
        }

        public override void OnCellDeselected(Cell cell)
        {
            if (!EventSystem.current.IsPointerOverGameObject())
                TooltipOff.Raise();
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