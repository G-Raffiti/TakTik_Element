using System;

namespace _Pathfinding.DataStructs
{
    /// <summary>
    /// Represents a prioritized queue.
    /// </summary>
    public interface IPriorityQueue<T>
    {
        /// <summary>
        /// Number of items in the queue.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Method adds item to the queue.
        /// </summary>
        void Enqueue(T _item, float _priority);
        /// <summary>
        /// Method returns item with the LOWEST priority value.
        /// </summary>
        T Dequeue();
    }

    /// <summary>
    /// Represents a node in a priority queue.
    /// </summary>
    class PriorityQueueNode<T> : IComparable
    {
        public T Item { get; private set; }
        public float Priority { get; private set; }

        public PriorityQueueNode(T _item, float _priority)
        {
            Item = _item;
            Priority = _priority;
        }

        public int CompareTo(object _obj)
        {
            return Priority.CompareTo((_obj as PriorityQueueNode<T>).Priority);
        }
    }
}
