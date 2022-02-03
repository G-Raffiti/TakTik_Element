using StateMachine;
using StateMachine.GridStates;

namespace Players
{
    /// <summary>
    /// Class representing a human player.
    /// </summary>
    public class HumanPlayer : Player
    {
        public override void Play(BattleStateManager _cellGrid)
        {
            _cellGrid.PlayingUnit.StartTurn();
            _cellGrid.BattleState = new BattleStateUnitSelected(_cellGrid, _cellGrid.PlayingUnit);
        }
    }
}
