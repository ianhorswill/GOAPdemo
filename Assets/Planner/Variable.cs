using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Assets.Planner
{
    /// <summary>
    /// Base class for all variables
    /// A variable represents an aspect of the game state that is visible to the planner
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public abstract class Variable : UidBase
    {
        /// <summary>
        /// Name of the variable
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// All variables; they collectively form the planner-visible game state.
        /// </summary>
        public static readonly List<Variable> AllVariables = new();

        /// <summary>
        /// The ValueFor(agent) method is cached to keep from recalculating it.
        /// This is the agent for whom it is currently cached.
        /// </summary>
        protected Agent CachedFor;

        /// <summary>
        /// Clear the cached values of all variables.
        /// </summary>
        public static void InvalidateCachedValues()
        {
            foreach (var v in AllVariables)
                v.CachedFor = null;
        }

        protected Variable(string name)
        {
            Name = name;
            AllVariables.Add(this);
        }

        public override string ToString() => Name;
    }

#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
    /// <summary>
    /// A variable represents an aspect of the game state that is visible to the planner
    /// </summary>
    /// <typeparam name="T">Data type of the variable's value</typeparam>
    public class Variable<T> : Variable
#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
    {
        private readonly Func<Agent,T> valueInEngine;
        private T cachedValue;

        /// <summary>
        /// Returns the current value of the variable.  Since many variables are agent-relative (e.g. nearest enemy)
        /// we have to tell it what agent we're asking for.
        /// Values are cached for the most recent agent.  The cache is cleared using Variable.InvalidateCachedValues.
        /// </summary>
        public T ValueFor(Agent a)
        {
            if (CachedFor != a)
            {
                cachedValue = valueInEngine(a);
                CachedFor = a;
            }

            return cachedValue;
        }

        /// <summary>
        /// Make a new variable
        /// </summary>
        /// <param name="name">Name, for debugging purposes</param>
        /// <param name="valueInEngine">Code to get current value of the variable for a specified agent from the game logic.</param>
        public Variable(string name, Func<Agent,T> valueInEngine) : base(name)
        {
            this.valueInEngine = valueInEngine;
        }

        /// <summary>
        /// Returns the Goal object for the specified variable having the specified value
        /// </summary>
        public static Goal operator ==(Variable<T> v, T value) => Goal<T>.Find(v, value);

        public static Goal operator !=(Variable<T> v, T value) => throw new NotImplementedException();
    }
}
