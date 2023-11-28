using _EventSystem.CustomEvents;
using Cells;

namespace StateMachine.GridStates
{
    public enum EBattleState {Beginning, BlockInput, Skill, Unit}
    public abstract class BattleState
    {
        protected BattleStateManager StateManager;
        public EBattleState State;

        protected BattleState(BattleStateManager _stateManager)
        {
            StateManager = _stateManager;
            //TooltipOn = StateManager.TooltipOn;
            //TooltipOff = StateManager.TooltipOff;
            //todo fix it
        }

        /// <summary>
        /// Method is called when mouse exits cell's collider.
        /// </summary>
        /// <param name="cell">Cell that was deselected.</param>
        public virtual void OnCellDeselected(Cell _targetCell)
        {
            if(_targetCell == null) return;
            _targetCell.UnMark();
        }

        /// <summary>
        /// Method is called when mouse enters cell's collider.
        /// </summary>
        /// <param name="cell">Cell that was selected.</param>
        public virtual void OnCellSelected(Cell _targetCell)
        {
            if(_targetCell == null) return;
            _targetCell.MarkAsHighlighted();
        }

        /// <summary>
        /// Method is called when a cell is clicked.
        /// </summary>
        /// <param name="cell">Cell that was clicked.</param>
        public virtual void OnCellClicked(Cell _cell)
        {
            if(_cell == null) return;
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