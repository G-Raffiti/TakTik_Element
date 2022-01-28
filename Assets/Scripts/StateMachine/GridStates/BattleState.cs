using _EventSystem.CustomEvents;
using Cells;

namespace StateMachine.GridStates
{
    public enum EBattleState {Beginning, BlockInput, Skill, Unit}
    public abstract class BattleState
    {
        protected BattleStateManager StateManager;
        protected InfoEvent TooltipOn;
        protected VoidEvent TooltipOff;
        public EBattleState State;

        protected BattleState(BattleStateManager _stateManager)
        {
            StateManager = _stateManager;
            TooltipOn = StateManager.TooltipOn;
            TooltipOff = StateManager.TooltipOff;
        }

        /// <summary>
        /// Method is called when mouse exits cell's collider.
        /// </summary>
        /// <param name="cell">Cell that was deselected.</param>
        public virtual void OnCellDeselected(Cell cell)
        {
            if(cell == null) return;
            cell.UnMark();
        }

        /// <summary>
        /// Method is called when mouse enters cell's collider.
        /// </summary>
        /// <param name="cell">Cell that was selected.</param>
        public virtual void OnCellSelected(Cell cell)
        {
            if(cell == null) return;
            cell.MarkAsHighlighted();
        }

        /// <summary>
        /// Method is called when a cell is clicked.
        /// </summary>
        /// <param name="cell">Cell that was clicked.</param>
        public virtual void OnCellClicked(Cell cell)
        {
            if(cell == null) return;
        }

        /// <summary>
        /// Method is called on transitioning into a state.
        /// </summary>
        public virtual void OnStateEnter()
        {
        }

        /// <summary>
        /// Method is called on transitioning out of a state.
        /// </summary>
        public virtual void OnStateExit()
        {
        }
    }
}