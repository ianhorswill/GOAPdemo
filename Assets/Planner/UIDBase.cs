using System;

namespace Assets.Planner
{
    /// <summary>
    /// This just assigns a UID to every instance, so we can use sorted dictionaries and lists for them.
    /// </summary>
    public class UidBase : IComparable, IComparable<UidBase>
    {
        private static uint nextUid;
        public readonly uint Uid = nextUid++;
        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(UidBase other) => Uid.CompareTo(other.Uid);
    }
}
