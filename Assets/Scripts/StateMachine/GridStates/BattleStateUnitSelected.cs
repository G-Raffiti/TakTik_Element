using System.Collections.Generic;
using System.Linq;
using Cells;
using GridObjects;
using Units;
using UnityEngine;
using UnityEngine.EventSystems;

namespace StateMachine.GridStates
{
    class BattleStateUnitSelected : BattleState
    {
        private Unit unit;
        private HashSet<Cell> pathsInRange;
        private BattleStateManager cellGrid;

        private Cell unitCell;

        private List<Cell> currentPath;

        public BattleStateUnitSelected(BattleStateManager _stateManager, Unit _unit) : base(_stateManager)
        {
            State = EBattleState.Unit;
            unit = _unit;
            pathsInRange = new HashSet<Cell>();
            currentPath = new List<Cell>();
            cellGrid = _stateManager;
        }

        public override void OnCellClicked(Cell cell)
        {
            if (unit.IsMoving)
                return;
            if (cell.IsTaken && cell.CurrentGridObject != null)
            {
                if (cell.CurrentGridObject.IsInteractable)
                {
                    cell.CurrentGridObject.Interact(unit);
                    return;
                }
            }

            if (!pathsInRange.Contains(cell))
                return;

            List<Cell> _path = unit.FindPath(StateManager.Cells, cell);
            unit.Move(cell, _path);
        }

        public override void OnCellDeselected(Cell cell)
        {
            base.OnCellDeselected(cell);
            if (!EventSystem.current.IsPointerOverGameObject())
                TooltipOff.Raise();
            
            MarkCellsBack();
        }
        
        public override void OnCellSelected(Cell cell)
        {
            base.OnCellSelected(cell);
            if (!EventSystem.current.IsPointerOverGameObject() &&
                cell.Buffs.Count > 0)
                TooltipOn.Raise((TileIsometric) cell);
            
            if (!EventSystem.current.IsPointerOverGameObject() && 
                Input.GetKey(KeyCode.LeftControl) || 
                Input.GetKey(KeyCode.RightControl))
            {
                if (cell.CurrentGridObject != null)
                    TooltipOn.Raise(cell.CurrentGridObject.GridObjectSO);
                else if (cell.CurrentUnit != null)
                    TooltipOn.Raise((cell.CurrentUnit));
                else TooltipOn.Raise((TileIsometric) cell);
            }
            
            

            if (cell.CurrentUnit is Monster _monster)
            {
                _monster.ShowRange();
            }
            
            if (!pathsInRange.Contains(cell)) return;

            currentPath = unit.FindPath(StateManager.Cells, cell);
            foreach (Cell _cell in currentPath)
            {
                _cell.MarkAsPath();
            }
            foreach (GridObject _gridObject in cellGrid.GridObjects)
            {
                if (_gridObject.Cell == cell)
                {
                    cell.MarkAsHighlighted();
                    continue;
                }

                if (_gridObject.IsInteractableFrom(cell))
                {
                    _gridObject.Cell.MarkAsInteractable();
                }
                else _gridObject.Cell.UnMark();
            }
        }

        public override void OnStateEnter()
        {
            unit.OnUnitSelected();
            unitCell = unit.Cell;

            MarkCellsBack();

        }

        private void MarkCellsBack()
        {
            pathsInRange = unit.GetAvailableDestinations(StateManager.Cells);
            IEnumerable<Cell> _cellsNotInRange = StateManager.Cells.Except(pathsInRange);

            foreach (Cell _cell in _cellsNotInRange)
            {
                _cell.UnMark();
            }
            foreach (Cell _cell in pathsInRange)
            {
                _cell.MarkAsReachable();
            }

            if (unit.BattleStats.AP <= 0) return;

            foreach (GridObject _object in StateManager.GridObjects)
            {
                if(_object.IsInteractable)
                    _object.Cell.MarkAsInteractable();
            }

            foreach (Unit _currentUnit in StateManager.Units)
            {
                if (_currentUnit.playerNumber.Equals(unit.playerNumber))
                    continue;

                _currentUnit.Cell.MarkAsEnemyCell();
            }
            
            unitCell.MarkAsHighlighted();
        }
        public override void OnStateExit()
        {
            unit.OnUnitDeselected();
            foreach (Cell _cell in StateManager.Cells)
            {
                _cell.UnMark();
            }
        }
    }
}