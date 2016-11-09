using Polarith.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Polarith.AI.Move
{
    /// <summary>
    /// This class extends the <see cref="AIMBehaviour"/> so that it is able to work with <see cref="IPercept{T}"/>
    /// instances which are relevant for the associated agent (front-end component). Two possibilities are added for
    /// obtaining percept data to be processed. One way is to use the environment/perceiver/filter pipeline through the
    /// corresponding components <see cref="AIMEnvironment"/>, <see cref="AIMPerceiver{T}"/> and <see
    /// cref="AIMFilter{T}"/>. The other possibility is given by the <see cref="GameObjects"/> list which takes custom
    /// targeted objects for extracting the appropriate percept data. The inputs of both gets combined and the result is
    /// handed over to <see cref="PerceptBehaviour{T}.Percepts"/> of the underlying derived back-end class. When the
    /// specified objects are needed by multiple behaviours and/or agents, let the agents perceive them via the
    /// environment/perceiver/filter pipeline to increase the overall performance.
    /// <para/>
    /// Derived <see cref="AIMBehaviour"/> front-end base component of every derived <see cref="PerceptBehaviour{T}"/>.
    /// </summary>
    /// <typeparam name="T">
    /// Type of the percept holding extracted data, needs to implement <see cref="IPercept{T}"/> and to provide a
    /// constructor.
    /// </typeparam>
    public abstract class AIMPerceptBehaviour<T> : AIMBehaviour
        where T : IPercept<GameObject>, new()
    {
        #region Fields =================================================================================================

        /// <summary>
        /// All environments to obtain the percepts for. A name correspond to an environment label.
        /// </summary>
        [Tooltip("All environments to obtain the percepts for. A name correspond to an environment label.")]
        public List<string> FilteredEnvironments = new List<string>();

        /// <summary>
        /// Allows to specify custom objects which should be processed by this behaviour. This is especially suitable
        /// for a few special targeted objects. When the specified objects are needed by multiple behaviours and/or
        /// agents, let the agents perceive them via the environment/perceiver/filter pipeline to increase the overall
        /// performance.
        /// </summary>
        [Tooltip("Allows to specify custom objects which should be processed by this behaviour. This is especially " +
            "suitable for a few special targeted objects. When the specified objects are needed by multiple " +
            "behaviours and/or agents, let the agents perceive them via the environment/perceiver/filter pipeline to " +
            "increase the overall performance.")]
        public List<GameObject> GameObjects = new List<GameObject>();

        //--------------------------------------------------------------------------------------------------------------

        private AIMFilter<T> filter;

        #endregion // Fields

        #region Properties =============================================================================================

        /// <summary>
        /// Polymorphic reference to the underlying back-end class (read only).
        /// <para/>
        /// Needs to be implemented by derived components.
        /// </summary>
        public abstract PerceptBehaviour<T> PerceptBehaviour { get; }

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Polymorphic reference to the underlying back-end class (read only).
        /// <para/>
        /// The returned reference is originally of type <see cref="PerceptBehaviour{T}"/>.
        /// </summary>
        public override MoveBehaviour Behaviour
        {
            get { return PerceptBehaviour; }
        }

        #endregion // Properties

        #region Methods ================================================================================================

        /// <summary>
        /// Fetches percept data from the filter which matches the percept type <typeparamref name="T"/> if there is one
        /// in the associated agent. In addition, it obtains percept data from <see cref="GameObjects"/> which has been
        /// directly specified within this component.
        /// <para/>
        /// Needs to be called from within the main thread.
        /// </summary>
        /// <exception cref="NullReferenceException">
        /// If <see cref="PerceptBehaviour"/> and/or its percepts are <c>null</c>.
        /// </exception>
        public override void PrepareEvaluation()
        {
            base.PrepareEvaluation();
            IList<T> percepts = PerceptBehaviour.Percepts;

            // Obtain data via a filter if it is necessary and there is one
            if (filter != null)
            {
                filter.GetPercepts(FilteredEnvironments, percepts);
                PerceptBehaviour.Self = filter.Self;
            }
            else // Obtain self data if no filter could be found
            {
                PerceptBehaviour.Self.Receive(aimContext.SelfObject);
            }

            // Obtain data for the specified game objects if there are any
            int filteredPerceptCount;
            if (FilteredEnvironments.Count > 0 && filter != null)
                filteredPerceptCount = percepts.Count;
            else
                filteredPerceptCount = 0;
            Collections.ResizeList(percepts, filteredPerceptCount + GameObjects.Count);
            for (int i = 0; i < GameObjects.Count; i++)
                percepts[i + filteredPerceptCount].Receive(GameObjects[i]);
        }

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// This method is called when the object becomes enabled and active.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            filter = GetComponent<AIMFilter<T>>();
        }

        #endregion // Methods
    } // class AIMPerceptBehaviour
} // namespace Polarith.AI.Move
