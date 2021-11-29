using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cells;
using Gears;
using UnityEngine;

namespace Units
{
    public abstract class IMovable : MonoBehaviour
    {
        /// <summary>
        /// Cell that the unit is currently occupying.
        /// </summary>
        [SerializeField] protected Cell cell;
        public Cell Cell
        {
            get => cell;
            set => cell = value;
        }

        public abstract string getName();

        public virtual void Move(Cell destinationCell, List<Cell> path)
        {
            Movable.Move(this, destinationCell, path);
        }
        public abstract float MovementAnimationSpeed { get; }
        public bool IsMoving { get; set; }
        public abstract IEnumerator MovementAnimation(List<Cell> _path);

        public abstract void AutoSortOrder();
        
        /// <summary>
        /// Method is Called if Cell is Underground
        /// </summary>
        public IEnumerator Fall(Cell _destination)
        {
            Inventory = new Inventory();
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

        public Inventory Inventory;
    }

    public static class Movable
    {
        public static List<Cell> Move(IMovable movable, Cell destinationCell, List<Cell> path)
        {
            if (movable.Cell == destinationCell)
                return new List<Cell>();
            
            movable.IsMoving = true;
            
            Cell destination = destinationCell;
            path.Sort((_cell, _cell1) => _cell.GetDistance(movable.Cell).CompareTo(_cell1.GetDistance(movable.Cell)));
                        
            List<Cell> deadEnd = path.Where(c => c.IsWalkable != true).ToList();
            if (deadEnd.Count > 0)
            {
                if (deadEnd[0].IsUnderGround)
                    destination = deadEnd[0];
                else
                {
                    if (movable.Cell.Neighbours.Contains(deadEnd[0])) return new List<Cell>();
                    
                    List<Cell> destinations = deadEnd[0].Neighbours;
                    destinations.Sort((c1, c2) => c1.GetDistance(movable.Cell).CompareTo(c2.GetDistance(movable.Cell)));
                    destination = destinations[0];
                }
            }

            List<Cell> pathToDestination = new List<Cell>();
            
            if (destination != destinationCell)
            {
                for (int i = 0; i < path.Count; i++)
                {
                    pathToDestination.Add(path[i]);
                    if (path[i] == destination)
                        break;
                }
            }
            else pathToDestination = path;

            movable.Cell.FreeTheCell();
            destination.Take(movable);

            if (movable.MovementAnimationSpeed > 0)
            {
                pathToDestination.Reverse();
                movable.StartCoroutine(movable.MovementAnimation(pathToDestination));
            }
            else
            {
                movable.TeleportTo(movable.Cell.transform.position);
            }
            
            
            if (destination.IsUnderGround)
            {
                movable.StartCoroutine(movable.Fall(movable.Cell));
            }

            return pathToDestination;
        }
    }
}