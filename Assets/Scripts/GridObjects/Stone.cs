using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Extension;
using _Instances;
using _Pathfinding.Algorithms;
using _ScriptableObject;
using Cells;
using Skills._Zone;
using StateMachine;
using Units;
using UnityEngine;

namespace GridObjects
{
    [CreateAssetMenu(fileName = "GridObject_Stone_", menuName = "Scriptable Object/Grid Objects/Stone")]
    public class Stone : GridObjectSo
    {
        [SerializeField] private int pushDistance;
        [SerializeField] private List<Sprite> stonesSprites;
        public override Sprite Image => stonesSprites.GetRandom();

        public override void Interact(Unit _actor, Cell _location)
        {
            base.Interact(_actor, _location);
            Utility.RunCoroutine(Push(_actor, _location.CurrentGridObject, pushDistance));
            //TODO : créer une variable de distance à la place de strength
        }

        private static Cell GetDestination(Movable _target, int _strength, Cell _targetedCell, Unit _actor)
        {
            // find the destination
            Cell _destination = _target.Cell;
            
            for (int _i = 1; _i <= _strength; _i++)
            {
                Cell _arrival = BattleStateManager.instance.Cells.Find(_c =>
                    _c.OffsetCoord == _target.Cell.OffsetCoord + Zone.Direction(_actor.Cell, _targetedCell) * _i);
                if (_arrival == null) break;
                _destination = _arrival;
            }

            return _destination;
        }
        private static IEnumerator Push(Unit _actor, Movable _target, int _strength)
        {
            // find the target
            if (_target == null) yield break;
            Cell _targetedCell = _target.Cell;
            _target.IsMoving = true;

            // find destination 
            Cell _destination = GetDestination(_target, _strength, _targetedCell, _actor);
            
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
            DijkstraPathfinding _pathfinder = new DijkstraPathfinding();
            Dictionary<Cell, List<Cell>> _paths = _pathfinder.FindAllPaths(_edges, _targetedCell);
            List<Cell> _path = _paths[_destination];
            _path = _path.OrderBy(_c => _targetedCell.GetDistance(_c)).Reverse().ToList();

            // Move
            int _distance = Units.Movement.Move(_target, _destination, _path).Count;
            if(_distance != 0)
                while (_target.IsMoving) yield return null;
            
            // If an Unit hit an other object, both take damage
            if (_distance < _strength)
            {
                Cell _obstacleCell = BattleStateManager.instance.Cells.Find(_c =>
                    _c.OffsetCoord == _target.Cell.OffsetCoord + Zone.Direction(_actor.Cell, _targetedCell));
                if (_obstacleCell == null) yield break;
                Unit _obstacle = _obstacleCell.CurrentUnit;
                if (_obstacle != null)
                    _obstacle.DefendHandler(_actor,
                        (_strength - _distance) * _actor.battleStats.power, Element.None());
            }
        }

        private Dictionary<Cell, CellState> savedMark;
        public override void ShowAction(Unit _actor, Cell _location)
        {
            base.ShowAction(_actor, _location);

            GetDestination(_location.CurrentGridObject, pushDistance, _location, _actor).MarkAsHighlighted();
        }

        //public override void UnShowAction(Unit actor, Cell location)
        
    }
}