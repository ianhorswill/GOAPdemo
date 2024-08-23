using System;
using System.Collections.Generic;

namespace Assets.Planner
{
    /// <summary>
    /// Priority queue implemented as a binary heap.
    /// Generic in element type, but priority is always a float.
    /// Implemented as a standard array-embedded binary heap.
    /// </summary>
    internal class MinQueue<T>
    {
        private readonly List<(float priority, T element)> data = new List<(float, T)> ();
        public bool IsEmpty => data.Count == 0;

        public int Parent(int child) => (child - 1) / 2;
        public int LeftChild(int parent) => 2 * parent + 1;
        public int RightChild(int parent) => 2 * parent + 2;

        /// <summary>
        /// Add element to queue with specified priority.
        /// Despite the term "priority", smaller numbers for priority are actually more important
        /// and are removed from the queue first.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="priority"></param>
        public void Add(T element, float priority)
        {
            var position = data.Count;
            data.Add((priority, element));
            MoveUp(position);
        }

        /// <summary>
        /// Remove and return the element from the queue with the smallest "priority".
        /// </summary>
        public (float Priority, T Element) RemoveMin()
        {
            if (data.Count == 0) throw new InvalidOperationException("Attempt to call RemoveMin() on an empty queue");
            var result = data[0];
            var lastElement = data.Count - 1;
            data[0] = data[lastElement];
            data.RemoveAt(lastElement);
            MoveDown(0);
            return result;
        }

        private void MoveUp(int position)
        {
            while (position > 0)
            {
                var parentPosition = Parent(position);
                if (data[parentPosition].priority > data[position].priority)
                    SwapData(position,parentPosition);
                position = parentPosition;
            }
        }

        private void MoveDown(int position)
        {
            again:
            var left = LeftChild(position);
            var right = RightChild(position);
            var smallest = position;
            if (left < data.Count && data[left].priority < data[smallest].priority) smallest = left;
            if (right < data.Count && data[right].priority < data[smallest].priority) smallest = right;
            if (position != smallest)
            {
                SwapData(position, smallest);
                goto again;
            }
        }

        private void SwapData(int position, int parentPosition) 
            => (data[position], data[parentPosition]) = (data[parentPosition], data[position]);
    }
}
