using System.Collections.Generic;
using _Pathfinding.DataStructs;
using Cells;

namespace _Pathfinding.Algorithms
{
    /// <summary>
    /// Implementation of Dijkstra pathfinding algorithm.
    /// </summary>
    public class DijkstraPathfinding : Pathfinding
    {
        public Dictionary<Cell, List<Cell>> FindAllPaths(Dictionary<Cell, Dictionary<Cell, float>> _edges, Cell _originNode)
        {
            IPriorityQueue<Cell> _frontier = new HeapPriorityQueue<Cell>();
            _frontier.Enqueue(_originNode, 0);

            Dictionary<Cell, Cell> _cameFrom = new Dictionary<Cell, Cell>();
            _cameFrom.Add(_originNode, default(Cell));
            Dictionary<Cell, float> _costSoFar = new Dictionary<Cell, float>();
            _costSoFar.Add(_originNode, 0);

            while (_frontier.Count != 0)
            {
                Cell _current = _frontier.Dequeue();
                List<Cell> _neighbours = GetNeigbours(_edges, _current);
                foreach (Cell _neighbour in _neighbours)
                {
                    float _newCost = _costSoFar[_current] + _edges[_current][_neighbour];
                    if (!_costSoFar.ContainsKey(_neighbour) || _newCost < _costSoFar[_neighbour])
                    {
                        _costSoFar[_neighbour] = _newCost;
                        _cameFrom[_neighbour] = _current;
                        _frontier.Enqueue(_neighbour, _newCost);
                    }
                }
            }

            Dictionary<Cell, List<Cell>> _paths = new Dictionary<Cell, List<Cell>>();
            foreach (Cell _destination in _cameFrom.Keys)
            {
                List<Cell> _path = new List<Cell>();
                Cell _current = _destination;
                while (!_current.Equals(_originNode))
                {
                    _path.Add(_current);
                    _current = _cameFrom[_current];
                }
                _paths.Add(_destination, _path);
            }
            return _paths;
        }
        public override List<T> FindPath<T>(Dictionary<T, Dictionary<T, float>> _edges, T _originNode, T _destinationNode)
        {
            IPriorityQueue<T> _frontier = new HeapPriorityQueue<T>();
            _frontier.Enqueue(_originNode, 0);

            Dictionary<T, T> _cameFrom = new Dictionary<T, T>();
            _cameFrom.Add(_originNode, default(T));
            Dictionary<T, float> _costSoFar = new Dictionary<T, float>();
            _costSoFar.Add(_originNode, 0);

            while (_frontier.Count != 0)
            {
                T _current = _frontier.Dequeue();
                List<T> _neighbours = GetNeigbours(_edges, _current);
                foreach (T _neighbour in _neighbours)
                {
                    float _newCost = _costSoFar[_current] + _edges[_current][_neighbour];
                    if (!_costSoFar.ContainsKey(_neighbour) || _newCost < _costSoFar[_neighbour])
                    {
                        _costSoFar[_neighbour] = _newCost;
                        _cameFrom[_neighbour] = _current;
                        _frontier.Enqueue(_neighbour, _newCost);
                    }
                }
                if (_current.Equals(_destinationNode)) break;
            }
            List<T> _path = new List<T>();
            if (!_cameFrom.ContainsKey(_destinationNode))
                return _path;

            _path.Add(_destinationNode);
            T _temp = _destinationNode;

            while (!_cameFrom[_temp].Equals(_originNode))
            {
                T _currentPathElement = _cameFrom[_temp];
                _path.Add(_currentPathElement);

                _temp = _currentPathElement;
            }

            return _path;
        }
    }
}