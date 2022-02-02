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
        public Dictionary<Cell, List<Cell>> FindAllPaths(Dictionary<Cell, Dictionary<Cell, float>> edges, Cell originNode)
        {
            IPriorityQueue<Cell> _frontier = new HeapPriorityQueue<Cell>();
            _frontier.Enqueue(originNode, 0);

            Dictionary<Cell, Cell> _cameFrom = new Dictionary<Cell, Cell>();
            _cameFrom.Add(originNode, default(Cell));
            Dictionary<Cell, float> _costSoFar = new Dictionary<Cell, float>();
            _costSoFar.Add(originNode, 0);

            while (_frontier.Count != 0)
            {
                Cell _current = _frontier.Dequeue();
                List<Cell> _neighbours = GetNeigbours(edges, _current);
                foreach (Cell _neighbour in _neighbours)
                {
                    float _newCost = _costSoFar[_current] + edges[_current][_neighbour];
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
                while (!_current.Equals(originNode))
                {
                    _path.Add(_current);
                    _current = _cameFrom[_current];
                }
                _paths.Add(_destination, _path);
            }
            return _paths;
        }
        public override List<T> FindPath<T>(Dictionary<T, Dictionary<T, float>> edges, T originNode, T destinationNode)
        {
            IPriorityQueue<T> _frontier = new HeapPriorityQueue<T>();
            _frontier.Enqueue(originNode, 0);

            Dictionary<T, T> _cameFrom = new Dictionary<T, T>();
            _cameFrom.Add(originNode, default(T));
            Dictionary<T, float> _costSoFar = new Dictionary<T, float>();
            _costSoFar.Add(originNode, 0);

            while (_frontier.Count != 0)
            {
                T _current = _frontier.Dequeue();
                List<T> _neighbours = GetNeigbours(edges, _current);
                foreach (T _neighbour in _neighbours)
                {
                    float _newCost = _costSoFar[_current] + edges[_current][_neighbour];
                    if (!_costSoFar.ContainsKey(_neighbour) || _newCost < _costSoFar[_neighbour])
                    {
                        _costSoFar[_neighbour] = _newCost;
                        _cameFrom[_neighbour] = _current;
                        _frontier.Enqueue(_neighbour, _newCost);
                    }
                }
                if (_current.Equals(destinationNode)) break;
            }
            List<T> _path = new List<T>();
            if (!_cameFrom.ContainsKey(destinationNode))
                return _path;

            _path.Add(destinationNode);
            T _temp = destinationNode;

            while (!_cameFrom[_temp].Equals(originNode))
            {
                T _currentPathElement = _cameFrom[_temp];
                _path.Add(_currentPathElement);

                _temp = _currentPathElement;
            }

            return _path;
        }
    }
}