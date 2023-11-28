using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Pathfinding.Algorithms;
using Cells;
using Gears;
using StateMachine;
using UnityEngine;
using UnityEngine.Serialization;

namespace Units
{
    public abstract class Movable : MonoBehaviour
    {
        protected static DijkstraPathfinding pathfinder = new DijkstraPathfinding();
        protected static Pathfinding fallbackPathfinder = new AStarPathfinding();
        /// <summary>
        /// Determines speed of movement animation.
        /// </summary>
        [Header("IMovable")]
        [SerializeField] protected float movementAnimationSpeed;
        [FormerlySerializedAs("Inventory")]
        public Inventory inventory;
        public float MovementAnimationSpeed => movementAnimationSpeed;
        public abstract SpriteRenderer MovableSprite { get; }
        
        /// <summary>
        /// Cell that the unit is currently occupying.
        /// </summary>
        public Cell Cell
        {
            get
            {
                if (BattleStateManager.instance.ObjectCells.Keys.Contains(this))
                    return BattleStateManager.instance.ObjectCells[this];
                return null;
            }
        }
        public abstract string GetName { get; }
        public bool IsMoving { get; set; }
        
        public virtual List<Cell> Move(Cell _destinationCell, List<Cell> _completePath)
        {
            return Movement.Move(this, _destinationCell, _completePath);
        }

        public virtual IEnumerator MovementAnimation(List<Cell> _path)
        {
            IsMoving = true;
            _path.Reverse();
            foreach (Cell _cell in _path)
            {
                _cell.Take(this);
                Vector3 _destinationPos = new Vector3(_cell.transform.localPosition.x, _cell.transform.localPosition.y,
                    transform.localPosition.z);
                while (transform.localPosition != _destinationPos)
                {
                    transform.localPosition = Vector3.MoveTowards(transform.localPosition, _destinationPos,
                        Time.deltaTime * MovementAnimationSpeed);
                    yield return 0;
                }
                AutoSortOrder();
            }

            IsMoving = false;
        }

        public virtual void AutoSortOrder()
        {
            MovableSprite.sortingOrder = 500 - (int)(transform.position.y/0.577f);
        }
        
        /// <summary>
        /// Method is Called if Cell is Underground
        /// </summary>
        public virtual IEnumerator Fall(Cell _destination)
        {
            inventory = new Inventory();
            while (IsMoving)
                yield return null;
            _destination.FallIn();
            StartCoroutine(OnDestroyed());
        }
        
        public abstract IEnumerator OnDestroyed();
        public void TeleportTo(Vector3 _transformPosition)
        {
            transform.position = _transformPosition;
        }

    }
}