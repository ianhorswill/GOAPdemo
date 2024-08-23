using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Planner
{
    public static class Executive
    {
        
        public static IEnumerator ExecutiveCoroutine(Agent agent, Goal[] topLevelGoals)
        {
            while (true)
            {
                Variable.InvalidateCachedValues();
                foreach (var g in topLevelGoals)
                {
                    if (!g.IsValidFor(agent) || g.IsTrueFor(agent))
                        continue;
                    var plan = Planner.Plan(g, agent);
                    if (plan != null)
                    {
                        UnityEngine.Debug.Log($"{agent.gameObject.name}: Got plan {plan} for {g}");
                        yield return plan.Perform(agent);
                        agent.Stop();
                        break;
                    }
                }

                yield return null;
            }
        }
    }
}
