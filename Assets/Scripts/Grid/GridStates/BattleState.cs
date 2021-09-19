using _EventSystem.CustomEvents;
using Cells;
using Units;

namespace Grid.GridStates
{
    public abstract class BattleState
    {
        protected BattleStateManager StateManager;
        protected InfoEvent TooltipOn;
        protected VoidEvent TooltipOff;

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
            cell.UnMark();
        }

        /// <summary>
        /// Method is called when mouse enters cell's collider.
        /// </summary>
        /// <param name="cell">Cell that was selected.</param>
        public virtual void OnCellSelected(Cell cell)
        {
            cell.MarkAsHighlighted();
        }

        /// <summary>
        /// Method is called when a cell is clicked.
        /// </summary>
        /// <param name="cell">Cell that was clicked.</param>
        public virtual void OnCellClicked(Cell cell)
        {
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