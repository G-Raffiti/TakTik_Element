using System.Collections.Generic;
using System.Linq;
using _Pathfinding.DataStructs;

namespace _Pathfinding.Algorithms
{
    public abstract class Pathfinding
    {
        /// <summary>
        /// Method finds path between two nodes in a graph.
        /// </summary>
        /// <param name="edges">
        /// Graph edges represented as nested dictionaries. Outer dictionary contains all nodes in the graph, inner dictionary contains 
        /// its neighbouring nodes with edge weight.
        /// </param>
        /// <returns>
        /// If a path exist, method returns list of nodes that the path consists of. Otherwise, empty list is returned.
        /// </returns>
        public abstract List<T> FindPath<T>(Dictionary<T, Dictionary<T, float>> _edges, T _originNode, T _destinationNode) where T : IGraphNode;

        protected List<T> GetNeigbours<T>(Dictionary<T, Dictionary<T, float>> _edges, T _node) where T : IGraphNode
        {
            if (!_edges.ContainsKey(_node))
            {
                return new List<T>();
            }
            return _edges[_node].Keys.ToList();
        }
    }
}