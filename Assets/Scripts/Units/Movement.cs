using System.Collections.Generic;
using System.Linq;
using Cells;

namespace Units
{
    public static class Movement
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

            if (movable.MovementAnimationSpeed > 0)
            {
                pathToDestination.Reverse();
                movable.StartCoroutine(movable.MovementAnimation(pathToDestination));
            }
            else
            {
                movable.TeleportTo(destination.transform.position);
            }

            if (destination.GetCurrentIMovable() != movable)
                destination.Take(movable);

            if (destination.IsUnderGround)
            {
                movable.StartCoroutine(movable.Fall(movable.Cell));
            }

            return pathToDestination;
        }
    }
}