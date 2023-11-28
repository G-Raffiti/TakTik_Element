using System.Collections.Generic;
using System.Linq;
using Cells;

namespace Units
{
    public static class Movement
    {
        public static List<Cell> Move(Movable _movable, Cell _destinationCell, List<Cell> _path)
        {
            if (_movable.Cell == _destinationCell)
                return new List<Cell>();

            _movable.IsMoving = true;

            Cell _destination = _destinationCell;
            _path.Sort((_cell, _cell1) => _cell.GetDistance(_movable.Cell).CompareTo(_cell1.GetDistance(_movable.Cell)));

            List<Cell> _deadEnd = _path.Where(_c => _c.IsWalkable != true).ToList();
            if (_deadEnd.Count > 0)
            {
                if (_deadEnd[0].IsUnderGround)
                    _destination = _deadEnd[0];
                else
                {
                    if (_movable.Cell.Neighbours.Contains(_deadEnd[0])) return new List<Cell>();

                    List<Cell> _destinations = _deadEnd[0].Neighbours;
                    _destinations.Sort((_c1, _c2) => _c1.GetDistance(_movable.Cell).CompareTo(_c2.GetDistance(_movable.Cell)));
                    _destination = _destinations[0];
                }
            }

            List<Cell> _pathToDestination = new List<Cell>();

            if (_destination != _destinationCell)
            {
                for (int _i = 0; _i < _path.Count; _i++)
                {
                    _pathToDestination.Add(_path[_i]);
                    if (_path[_i] == _destination)
                        break;
                }
            }
            else _pathToDestination = _path;

            if (_movable.MovementAnimationSpeed > 0)
            {
                _pathToDestination.Reverse();
                _movable.StartCoroutine(_movable.MovementAnimation(_pathToDestination));
            }
            else
            {
                _movable.TeleportTo(_destination.transform.position);
            }

            if (_destination.GetCurrentIMovable() != _movable)
                _destination.ForceTake(_movable);

            if (_destination.IsUnderGround)
            {
                _movable.StartCoroutine(_movable.Fall(_movable.Cell));
            }

            return _pathToDestination;
        }
    }
}