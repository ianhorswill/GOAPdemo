using System.Collections.Immutable;
using System.Diagnostics;

namespace Assets.Planner
{
    /// <summary>
    /// GOAP planner implementation
    /// </summary>
    public static class Planner
    {
        /// <summary>
        /// Maximum number of nodes to search before giving up
        /// </summary>
        public static int Timeout = 100;

        /// <summary>
        /// Find a plan to achieve goal from the current game state, on behalf of agent.
        /// </summary>
        /// <param name="goal">Goal to achieve</param>
        /// <param name="agent">Agent (NPC) trying to achieve it.</param>
        /// <returns>Plan to achieve goal or null if planning failed.</returns>
        public static Plan Plan(Goal goal, Agent agent)
        {
            var queue = new MinQueue<PartialPlan>();

            // Add initial partial plan, which has no actions and goal as its subgoal.
            queue.Add(new PartialPlan(null, goal, ImmutableSortedDictionary<Variable, Goal>.Empty, 1), 0);

            // Loop, elaborating the lowest cost partial plan and queuing the results elaborations
            for (var step = 0; step < Timeout; step++)
            {
                if (queue.IsEmpty)
                    // Goal is unachievable
                    return null;

                // Get the best partial plan
                var (_, partialPlan) = queue.RemoveMin();
                if (partialPlan.RemainingSubgoals == null)
                    // Success
                    return new Plan(partialPlan.Plan.ToArray());

                // Pick the next subgoal
                var nextSubgoal = partialPlan.RemainingSubgoals.First;
                var restSubgoals = partialPlan.RemainingSubgoals.Rest;

                // If the next subgoal is incompatible with a protected goal, abandon this partial plan
                if (partialPlan.Protected.TryGetValue(nextSubgoal.VariableUntyped, out var protectedGoal))
                {
                    Debug.Assert(!ReferenceEquals(protectedGoal, nextSubgoal), "Trying to achieve a protected goal");
                    continue;
                }

                // Extend partial plan with each action that can achieve the subgoal
                foreach (var a in nextSubgoal.Achievers)
                {
                    var newProtected = partialPlan.Protected;
                    var newSubgoals = restSubgoals.Where(sub => !a.Effects.Contains(sub));

                    // Find the estimated cost of newGoals
                    var heuristic = TotalGoalCost(newSubgoals);

                    // Sort preconditions into protected goals (already true) and subgoals
                    (newProtected, newSubgoals, heuristic) =
                        SortPreconditions(agent, a.Preconditions, newProtected, newSubgoals, heuristic);

                    // Make the elaborated partial plan
                    var newPartialPlan =
                        new PartialPlan(
                            new LList<Action>(a, partialPlan.Plan),
                            newSubgoals,
                            newProtected,
                            partialPlan.Cost + a.Cost);

                    // Queue it
                    queue.Add(newPartialPlan, newPartialPlan.Cost + heuristic);
                }
            }

            // Failure - timeout
            return null;
        }

        /// <summary>
        /// Divide preconditions into ones that are already true, and not already true.
        /// Add already true ones to protected goals and the not already true ones to subgoals.
        /// Also update cost of heuristic function on subgoals.
        /// </summary>
        /// <param name="agent">Agent we're evaluating this for</param>
        /// <param name="preconditions">Preconditions of the action we're considering</param>
        /// <param name="protectedGoals">Protected goals from the parent plan</param>
        /// <param name="subgoals">Subgoals form the parent plan</param>
        /// <param name="heuristic">Heuristic value on the subgoals</param>
        /// <returns>New protected goals, new subgoals, new heuristic value</returns>
        private static (ImmutableSortedDictionary<Variable, Goal>, LList<Goal>, float)
            SortPreconditions(Agent agent, Goal[] preconditions,
                ImmutableSortedDictionary<Variable, Goal> protectedGoals,
                LList<Goal> subgoals,
                float heuristic)
        {
            foreach (var p in preconditions)
            {
                if (p.IsTrueFor(agent))
                    protectedGoals = protectedGoals.Add(p.VariableUntyped, p);
                else
                {
                    subgoals = p + subgoals;
                    heuristic += p.Cost;
                }
            }
            return (protectedGoals, subgoals, heuristic);
        }

        /// <summary>
        /// Find the total heuristic cost of a list of subgoals.
        /// </summary>
        private static float TotalGoalCost(LList<Goal> newSubgoals)
        {
            var heuristic = 0f;
            for (var cell = newSubgoals; cell != null; cell = cell.Rest)
                heuristic += cell.First.Cost;
            return heuristic;
        }
    }
}
