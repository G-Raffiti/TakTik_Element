using System.Collections.Generic;
using Cells;

namespace StateMachine.GridStates
{
    public class BattleStateBlockInput : BattleState
    {
        public BattleStateBlockInput(BattleStateManager _stateManager) : base(_stateManager)
        {
            State = EBattleState.BlockInput;
        }
        
        public override void OnStateEnter()
        {
            if (StateManager.Cells == null) return; 
            foreach (Cell _cell in StateManager.Cells)
            {
                _cell.UnMark();
            }
        }

        public override void OnStateExit()
        {
        }

        public override void OnCellSelected(Cell _targetCell)
        {
        }

        public override void OnCellDeselected(Cell _targetCell)
        {
        }

        public override void OnCellClicked(Cell _cell)
        {
        }
    }
}
