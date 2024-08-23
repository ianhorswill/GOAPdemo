using System;

namespace Assets.Planner
{
    /// <summary>
    /// Linked List
    /// </summary>
    public class LList<T>
    {
        /// <summary>
        /// First element of the list
        /// </summary>
        public readonly T First;
        /// <summary>
        /// Rest of the list
        /// </summary>
        public readonly LList<T> Rest;

        public LList(T first, LList<T> rest)
        {
            First = first;
            Rest = rest;
        }

        public static implicit operator LList<T>(T element) => new LList<T>(element, null);

        public static LList<T> operator +(T first, LList<T> rest) => new LList<T>(first, rest);
    }

    public static class LListUtilities
    {
        /// <summary>
        /// Return just the elements of list that satisfy predicate, followed by tail.
        /// Shares as much structure as possible with the original list, without mutating the original list.
        /// </summary>
        public static LList<T> Where<T>(this LList<T> list, Predicate<T> predicate, LList<T> tail = null)
        {
            if (list == null)
                return tail;
            var newTail = list.Rest.Where(predicate, tail);
            var keep = predicate(list.First);
            if (keep && newTail == tail)
                return list;
            return keep ? list.First+newTail : newTail;
        }

        /// <summary>
        /// An array containing the elements of list, in order.
        /// </summary>
        public static T[] ToArray<T>(this LList<T> list)
        {
            int length=0;
            for (var cell = list; cell != null; cell = cell.Rest)
                length++;
            var array = new T[length];
            int i=0;
            for (var cell = list; cell != null; cell = cell.Rest)
            {
                array[i++] = cell.First;
            }

            return array;
        }
    }
}
