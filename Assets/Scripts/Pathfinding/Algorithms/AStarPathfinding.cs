using System.Collections.Generic;
using Pathfinding.DataStructs;

namespace Pathfinding.Algorithms
{
    /// <summary>
    /// Implementation of A* pathfinding algorithm.
    /// </summary>
    class AStarPathfinding : Pathfinding
    {
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
                if (_current.Equals(destinationNode)) break;

                List<T> _neighbours = GetNeigbours(edges, _current);
                foreach (T _neighbour in _neighbours)
                {
                    float _newCost = _costSoFar[_current] + edges[_current][_neighbour];
                    if (!_costSoFar.ContainsKey(_neighbour) || _newCost < _costSoFar[_neighbour])
                    {
                        _costSoFar[_neighbour] = _newCost;
                        _cameFrom[_neighbour] = _current;
                        float _priority = _newCost + Heuristic(destinationNode, _neighbour);
                        _frontier.Enqueue(_neighbour, _priority);
                    }
                }
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
        private int Heuristic<T>(T a, T b) where T : IGraphNode
        {
            return a.GetDistance(b);
        }
    }
}



