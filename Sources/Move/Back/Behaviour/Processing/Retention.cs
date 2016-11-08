using Polarith.AI.Criteria;
using Polarith.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Polarith.AI.Move
{
    /// <summary>
    /// <see cref="Retention"/> is used to remember objective values for multiple frames (back-end class). How long
    /// values are being remembered can be controlled through the <see cref="Memory"/> parameter. When an agent
    /// remembers the data of its last decisions, the time-dependent coherence increases, and thus, the transition
    /// between consecutive decisions appears to be smoother. Note, too high values for the <see cref="Memory"/>
    /// parameter will lead to sluggish agents which cannot react properly to certain situations any longer.
    /// <para/>
    /// Back-end class of <see cref="AIMRetention"/>. This behaviour is thread-safe.
    /// </summary>
    [Serializable]
    public class Retention : MoveBehaviour
    {
        #region Fields =================================================================================================

        /// <summary>
        /// Specifies all the objectives which are influenced by this behaviour.
        /// </summary>
        [Tooltip("Specifies all the objectives which are influenced by this behaviour.")]
        public List<int> TargetObjectives = new List<int>();

        /// <summary>
        /// Determines how long old objectives values are being remembered in future AI updates. The higher this value,
        /// the longer it takes for new sampled values to occur within the problem objectives.
        /// </summary>
        [Tooltip("Determines how long old objectives values are being remembered in future AI updates. The higher " +
            "this value, the longer it takes for new sampled values to occur within the problem objectives.")]
        [Range(0f, 1f)]
        public float Memory = 0.5f;

        //--------------------------------------------------------------------------------------------------------------

        private List<List<float>> oldObjectives = new List<List<float>>();

        #endregion // Fields

        #region Methods ================================================================================================

        /// <summary>
        /// Processes the algorithms of this class writing/modifying objective values.
        /// </summary>
        public override void Behave()
        {
            IProblem<float> problem = Context.Problem;

            // Cancel the behaviour if the setup is invalid
            if (TargetObjectives.Count == 0 || problem.ObjectiveCount == 0 || problem.ValueCount == 0)
                return;

            // Maintain consistency of old objectives and its corresponding values
            if (TargetObjectives.Count != oldObjectives.Count ||
                oldObjectives.Count == 0 ||
                oldObjectives[0].Count == 0 ||
                problem.ValueCount != oldObjectives[0].Count)
            {
                Collections.ResizeList(oldObjectives, TargetObjectives.Count);
                for (int i = 0; i < TargetObjectives.Count; i++)
                {
                    Collections.ResizeList(oldObjectives[i], problem.ValueCount);

                    if (TargetObjectives[i] < 0 || TargetObjectives[i] >= problem.ObjectiveCount)
                        continue;

                    for (int j = 0; j < problem.ValueCount; j++)
                        oldObjectives[i][j] = problem[TargetObjectives[i]][j];
                }
            }

            // Retention algorithm
            ReadOnlyCollection<float> values;
            List<float> oldValues;
            for (int i = 0; i < TargetObjectives.Count; i++)
            {
                if (TargetObjectives[i] < 0 || TargetObjectives[i] >= problem.ObjectiveCount)
                    continue;

                values = problem[TargetObjectives[i]];
                oldValues = oldObjectives[i];

                for (int j = 0; j < values.Count; j++)
                {
                    problem.SetValue(TargetObjectives[i], j, Mathf.Lerp(values[j], oldValues[j], Memory));
                    oldValues[j] = values[j];
                }
            }
        }

        #endregion // Methods
    } // class Retention
} // namespace Polarith.AI.Move
