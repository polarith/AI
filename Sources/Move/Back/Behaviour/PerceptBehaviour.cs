using System.Collections.Generic;
using UnityEngine;

namespace Polarith.AI.Move
{
    /// <summary>
    /// This class extends the <see cref="MoveBehaviour"/> so that it is able to work with <see cref="IPercept{T}"/>
    /// instances which are relevant for the associated agent (back-end class). In addition, for derived classes, it
    /// requires the implementation of the <see cref="Self"/> property.
    /// <para/>
    /// The percept data allows this behaviour to interact with the environment of the associated agent to fulfil its
    /// purpose. These data might come from an <see cref="AIMFilter{T}"/> instance (which obtains its data from an <see
    /// cref="AIMPerceiver{T}"/> and its associated <see cref="AIMEnvironment"/>).
    /// <para/>
    /// A <see cref="PerceptBehaviour{T}"/> can be marked as thread-safe in its corresponding front-end class through
    /// setting <see cref="AIMBehaviour.ThreadSafe"/> to <c>true</c>. If you do this, be sure not to use reference types
    /// of the Unity scripting API and not to make any possibly unsafe variable writes. If at least one behaviour is not
    /// marked <see cref="AIMBehaviour.ThreadSafe"/>, the whole agent will not be part of the multithreading in <see
    /// cref="AIMThreading"/>.
    /// <para/>
    /// Base back-end class of every derived <see cref="AIMPerceptBehaviour{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of the processed percepts.</typeparam>
    public abstract class PerceptBehaviour<T> : MoveBehaviour
        where T : IPercept<GameObject>, new()
    {
        #region Fields =================================================================================================

        /// <summary>
        /// All percepts which are relevant for an agent.
        /// </summary>
        public readonly IList<T> Percepts = new List<T>();

        #endregion // Fields

        #region Properties =============================================================================================

        /// <summary>
        /// The data of the associated agent itself.
        /// </summary>
        public abstract T Self { get; set; }

        #endregion // Properties
    } // class PerceptBehaviour
} // namespace Polarith.AI.Move
