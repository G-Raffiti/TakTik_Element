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

        public override void OnCellClicked(Cell _cell)
        {
            if (unit.IsMoving)
                return;
            if (_cell.IsTaken && _cell.CurrentGridObject != null)
            {
                if (_cell.CurrentGridObject.IsInteractable)
                {
                    _cell.CurrentGridObject.Interact(unit);
                    return;
                }
            }

            if (!pathsInRange.Contains(_cell))
                return;

            List<Cell> _path = unit.FindPath(StateManager.Cells, _cell);
            unit.Move(_cell, _path);
        }

        public override void OnCellDeselected(Cell _targetCell)
        {
            base.OnCellDeselected(_targetCell);
            
            MarkCellsBack();
        }
        
        public override void OnCellSelected(Cell _targetCell)
        {
            base.OnCellSelected(_targetCell);
            
            

            if (_targetCell.CurrentUnit is Monster _monster)
            {
                _monster.ShowRange();
            }
            
            if (!pathsInRange.Contains(_targetCell)) return;

            currentPath = unit.FindPath(StateManager.Cells, _targetCell);
            foreach (Cell _cell in currentPath)
            {
                _cell.MarkAsPath();
            }
            foreach (GridObject _gridObject in cellGrid.GridObjects)
            {
                if (_gridObject.Cell == _targetCell)
                {
                    _targetCell.MarkAsHighlighted();
                    continue;
                }

                if (_gridObject.IsInteractableFrom(_targetCell))
                {
                    _gridObject.Cell.MarkAsInteractable();
                }
                else _gridObject.Cell.UnMark();
            }
        }

        public override void OnStateEnter()
        {
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

            if (unit.battleStats.ap <= 0) return;

            foreach (GridObject _object in StateManager.GridObjects)
            {
                if(_object.IsInteractable)
                    _object.Cell.MarkAsInteractable();
            }

            foreach (Unit _currentUnit in StateManager.Units)
            {
                if (_currentUnit.playerType.Equals(unit.playerType))
                    continue;

                _currentUnit.Cell.MarkAsEnemyCell();
            }
            
            unitCell.MarkAsHighlighted();
        }
        public override void OnStateExit()
        {
            foreach (Cell _cell in StateManager.Cells)
            {
                _cell.UnMark();
            }
        }
    }
}