using System;
using System.Collections.Generic;

namespace Assets.Planner
{
    /// <summary>
    /// Base class for all goals in the system.
    /// Goals are things that can be true or not true and involve making a Variable have a specified value.
    /// </summary>
    public abstract class Goal : UidBase
    {
        /// <summary>
        /// Ask the game logic whether this goal is true.
        /// Takes the Agent as an argument because a lot of goals and variables are agent-relative,
        /// e.g. "nearest enemy".
        /// </summary>
        public abstract bool IsTrueFor(Agent a);
        /// <summary>
        /// True if the executive should consider this goal
        /// This isn't used by the planner.  It's used by the executive that decides what goal to plan for.
        /// This is a cheap way to disable a goal temporarily.
        /// </summary>
        public Predicate<Agent> IsValidFor = _ => true;
        /// <summary>
        /// The variable the goal wants to have some value.
        /// The values are type-dependent, so the type field is declared in the generic class, below.
        /// </summary>
        public abstract Variable VariableUntyped { get; }
        /// <summary>
        /// Actions that have this goal as an effect.
        /// </summary>
        public readonly List<Action> Achievers = new List<Action>();
        /// <summary>
        /// Cost of performing the action.
        /// Should probably be a function, so an action can have different costs in different situations.
        /// </summary>
        public float Cost => 1;

        /// <summary>
        /// Return a goal that makes the specified Boolean variable be true.
        /// </summary>
        public static implicit operator Goal(Variable<bool> b) => Goal<bool>.Find(b, true);
    }

    /// <summary>
    /// A goal whose Variable and value have type T
    /// </summary>
    public class Goal<T> : Goal
    {
        /// <summary>
        /// The variable the goal wants to control the value of.
        /// </summary>
        public readonly Variable<T> Variable;

        /// <inheritdoc />
        public override Variable VariableUntyped => Variable;

        /// <summary>
        /// The value the goal wants Variable to have
        /// </summary>
        public readonly T Value;

        /// <summary>
        /// Ask the game logic if this is currently to for the specified agent.
        /// </summary>
        public override bool IsTrueFor(Agent a) => EqualityComparer<T>.Default.Equals(Variable.ValueFor(a),Value);

        /// <summary>
        /// Make a new goal.  This is private because we use a factory method (Find) to ensure there is never more than
        /// one copy of a give variable
        /// </summary>
        private Goal(Variable<T> variable, T value)
        {
            Variable = variable;
            Value = value;
        }

        /// <summary>
        /// Find the goal that sets the specified variable to the specified value.  If none exists, make one.
        /// </summary>
        public static Goal<T> Find(Variable<T> v, T value)
        {
            if (GoalTable.TryGetValue((v, value), out var goal))
                return goal;
            GoalTable[(v, value)] = goal = new Goal<T>(v, value);
            return goal;
        }

        /// <summary>
        /// Table of all existing goals of type T
        /// </summary>
        private static readonly Dictionary<(Variable<T>, T), Goal<T>> GoalTable = new();

        /// <summary>
        /// Specifies when the goal is "valid".  The planner doesn't care whether a goal is valid.
        /// This is only used by the executive, which has to decide which goal to pass to the planner.
        /// Setting this gives the executive a way to selectively disable top-level goals.
        /// </summary>
        public Goal<T> ValidWhen(Predicate<Agent> predicate)
        {
            IsValidFor = predicate;
            return this;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Variable} == {StringifyValue(Value)}";
        }

        private static string StringifyValue(object o) => o == null ? "null" : o.ToString();
    }
}
