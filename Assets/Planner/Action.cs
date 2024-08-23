using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Assets.Planner
{
    /// <summary>
    /// An action that can be used in a plan.
    /// Actions in GOAP don't take arguments.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public class Action
    {
        /// <summary>
        /// Name of action for debugging purposes
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// Goals that automatically become true if this action is successfully executed.
        /// This is only ever used to check whether a given goal is an effect of the action,
        /// so we store it in a hash set.
        /// </summary>
        public readonly HashSet<Goal> Effects = new();
        /// <summary>
        /// Things that have to be true to be able to execute this action at all.
        /// </summary>
        public Goal[] Preconditions = Array.Empty<Goal>();
        /// <summary>
        /// Returns a coroutine that will execute the action for the specified agent.
        /// </summary>
        public readonly Func<Agent, IEnumerator> Implementation;

        public Action(string name, Func<Agent, IEnumerator> implementation)
        {
            Name = name;
            Implementation = implementation;
        }

        public float Cost = 1;

        public Action Achieves(params Goal[] effects)
        {
            foreach (var effect in effects)
            {
                Effects.Add(effect);
                effect.Achievers.Add(this);
            }
            return this;
        }

        public Action Needs(params Goal[] preconditions)
        {
            Preconditions = preconditions;
            return this;
        }
    }
}
