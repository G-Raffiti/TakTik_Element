using System.Collections.Generic;

namespace _Pathfinding.DataStructs
{
    /// <summary>
    /// Implementation of priority queue based on heap. Should be fast.
    /// </summary>
    class HeapPriorityQueue<T> : IPriorityQueue<T>
    {
        private List<PriorityQueueNode<T>> queue;

        public HeapPriorityQueue()
        {
            queue = new List<PriorityQueueNode<T>>();
        }

        public int Count
        {
            get { return queue.Count; }
        }

        public void Enqueue(T _item, float _priority)
        {
            queue.Add(new PriorityQueueNode<T>(_item, _priority));
            int _ci = queue.Count - 1;
            while (_ci > 0)
            {
                int _pi = (_ci - 1) / 2;
                if (queue[_ci].CompareTo(queue[_pi]) >= 0)
                    break;
                PriorityQueueNode<T> _tmp = queue[_ci];
                queue[_ci] = queue[_pi];
                queue[_pi] = _tmp;
                _ci = _pi;
            }
        }
        public T Dequeue()
        {
            int _li = queue.Count - 1;
            PriorityQueueNode<T> _frontItem = queue[0];
            queue[0] = queue[_li];
            queue.RemoveAt(_li);

            --_li;
            int _pi = 0;
            while (true)
            {
                int _ci = _pi * 2 + 1;
                if (_ci > _li) break;
                int _rc = _ci + 1;
                if (_rc <= _li && queue[_rc].CompareTo(queue[_ci]) < 0)
                    _ci = _rc;
                if (queue[_pi].CompareTo(queue[_ci]) <= 0) break;
                PriorityQueueNode<T> _tmp = queue[_pi]; queue[_pi] = queue[_ci]; queue[_ci] = _tmp;
                _pi = _ci;
            }
            return _frontItem.Item;
        }
    }
}