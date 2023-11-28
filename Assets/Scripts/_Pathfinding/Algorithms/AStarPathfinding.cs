using System.Collections.Generic;
using _Pathfinding.DataStructs;

namespace _Pathfinding.Algorithms
{
    /// <summary>
    /// Implementation of A* pathfinding algorithm.
    /// </summary>
    class AStarPathfinding : Pathfinding
    {
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
                if (_current.Equals(_destinationNode)) break;

                List<T> _neighbours = GetNeigbours(_edges, _current);
                foreach (T _neighbour in _neighbours)
                {
                    float _newCost = _costSoFar[_current] + _edges[_current][_neighbour];
                    if (!_costSoFar.ContainsKey(_neighbour) || _newCost < _costSoFar[_neighbour])
                    {
                        _costSoFar[_neighbour] = _newCost;
                        _cameFrom[_neighbour] = _current;
                        float _priority = _newCost + Heuristic(_destinationNode, _neighbour);
                        _frontier.Enqueue(_neighbour, _priority);
                    }
                }
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
        private int Heuristic<T>(T _a, T _b) where T : IGraphNode
        {
            return _a.GetDistance(_b);
        }
    }
}



