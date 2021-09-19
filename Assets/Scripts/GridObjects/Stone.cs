using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Instances;
using _ScriptableObject;
using Cells;
using Grid;
using Pathfinding.Algorithms;
using Skills._Zone;
using Stats;
using Units;
using UnityEngine;

namespace GridObjects
{
    [CreateAssetMenu(fileName = "GridObject_Stone_", menuName = "Scriptable Object/Grid Objects/Stone")]
    public class Stone : GridObjectSO
    {
        [SerializeField] private float weight;

        public override void Interact(Unit actor, Cell location)
        {
            base.Interact(actor, location);
            DataBase.RunCoroutine(Push(actor, location.CurrentGridObject, (int)(actor.BattleStats.Power.Physic(EElement.None)/weight)));
        }
        
        
        private static IEnumerator Push(Unit actor, IMovable target, int strength)
        {
            // find the target
            if (target == null) yield break;
            Cell _targetedCell = target.Cell;
            target.IsMoving = true;

            // find the destination
            Cell destination = target.Cell;
            
            for (int i = 1; i <= strength; i++)
            {
                Cell arrival = BattleStateManager.instance.Cells.Find(c =>
                    c.OffsetCoord == target.Cell.OffsetCoord + Zone.Direction(actor.Cell, _targetedCell) * i);
                if (arrival == null) break;
                destination = arrival;
            }
            
            
            // find the shortest path between target and destination
            Dictionary<Cell, Dictionary<Cell, float>> _edges = new Dictionary<Cell, Dictionary<Cell, float>>();
            foreach (Cell _cell in BattleStateManager.instance.Cells)
            {
                _edges[_cell] = new Dictionary<Cell, float>();
                foreach (Cell _neighbour in _cell.Neighbours)
                {
                    _edges[_cell][_neighbour] = 1;
                }
            }
            DijkstraPathfinding pathfinder = new DijkstraPathfinding();
            Dictionary<Cell, List<Cell>> _paths = pathfinder.FindAllPaths(_edges, _targetedCell);
            List<Cell> _path = _paths[destination];
            _path = _path.OrderBy(c => _targetedCell.GetDistance(c)).Reverse().ToList();

            // Move
            int _distance = Units.Movable.Move(target, destination, _path).Count;
            if(_distance != 0)
                while (target.IsMoving) yield return null;
            
            // If an Unit hit an other object, both take damage
            if (_distance < strength)
            {
                Cell obstacleCell = BattleStateManager.instance.Cells.Find(c =>
                    c.OffsetCoord == target.Cell.OffsetCoord + Zone.Direction(actor.Cell, _targetedCell));
                if (obstacleCell == null) yield break;
                Unit obstacle = obstacleCell.CurrentUnit;
                if (obstacle != null)
                    obstacle.DefendHandler(actor,
                        (strength - _distance) * actor.BattleStats.Power.Physic(EElement.None), Element.None());
            }
        }

    }
}