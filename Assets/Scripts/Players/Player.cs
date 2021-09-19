using Grid;
using UnityEngine;

namespace Players
{
    /// <summary>
    /// Class represents a game participant.
    /// </summary>
    public abstract class Player : MonoBehaviour
    {
        public int playerNumber;
        /// <summary>
        /// Method is called every turn. Allows player to interact with his units.
        /// </summary>         
        public abstract void Play(BattleStateManager _cellGrid);
    }
}