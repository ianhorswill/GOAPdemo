using System.Collections.Immutable;

namespace Assets.Planner
{
    /// <summary>
    /// A partially worked-out plan for achieving the original goal
    /// These form the nodes of the tree searched by the planner.  Children of tree nodes are partial plans that
    /// are elaborations of the parent partial plan.  The root is a partial plan with no actions worked out yet, and the
    /// leaves are either completed plans or broken partial plans that can't be completed.
    /// </summary>
    internal class PartialPlan
    {
        /// <summary>
        /// Actions in the plan, in reverse order
        /// These are all actions that will occur last in the final plan, because the planner plans backward.
        /// </summary>
        public readonly LList<Action> Plan;
        /// <summary>
        /// Goals that must be established by new actions before the actions in Plan can be executed.
        /// </summary>
        public readonly LList<Goal> RemainingSubgoals;
        /// <summary>
        /// Goals that actions in Plan rely on that are already true in the current world state
        /// and so shouldn't be changed ("clobbered") by and actions we add at the beginning
        /// </summary>
        public readonly ImmutableSortedDictionary<Variable,Goal> Protected;
        /// <summary>
        /// Cumulative cost for actions in plan.
        /// This does not include the heuristic function and so is different from the priority in the planner's queue.
        /// </summary>
        public readonly float Cost;

        public PartialPlan(LList<Action> plan, LList<Goal> remainingSubgoals, ImmutableSortedDictionary<Variable, Goal> @protected, float cost)
        {
            Plan = plan;
            RemainingSubgoals = remainingSubgoals;
            Protected = @protected;
            Cost = cost;
        }
    }
}
