using UnityEngine;

namespace Polarith.AI.Move
{
    /// <summary>
    /// By using <see cref="AIMFollow"/> the agent follows one target independent on its distance to the agent
    /// (front-end component). Note, changes to the inherited <see cref="AIMPerceptBehaviour{T}.FilteredEnvironments"/>
    /// and <see cref="AIMPerceptBehaviour{T}.GameObjects"/> fields have no effect, since they are reset <see
    /// cref="PrepareEvaluation"/>.
    /// <para/>
    /// Front-end component of the underlying <see cref="Move.Follow"/> class. This behaviour is thread-safe.
    /// </summary>
    [AddComponentMenu("Polarith AI » Move/Behaviours/Steering/AIM Follow")]
    public sealed class AIMFollow : AIMSteeringBehaviour
    {
        #region Fields =================================================================================================

        /// <summary>
        /// The target game object used by the agent to move towards.
        /// </summary>
        [Tooltip("The target game object used by the agent to move towards.")]
        public GameObject Target;

        /// <summary>
        /// The target position used by the agent to move towards, therefore, the <see cref="Target"/> must be
        /// <c>null</c>.
        /// </summary>
        [Tooltip("The target position used by the agent to move towards, therefore, the 'Target' must be 'null'.")]
        public Vector3 TargetPosition;

        /// <summary>
        /// The underlying back-end behaviour class.
        /// </summary>
        [Tooltip("Allows to specify the settings of this behaviour.")]
        public Follow Follow = new Follow();

        #endregion // Fields

        #region Properties =============================================================================================

        /// <summary>
        /// Polymorphic reference to the underlying back-end class (read only).
        /// </summary>
        public override SteeringBehaviour SteeringBehaviour
        {
            get { return Follow; }
        }

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Determines whether the underlying back-end class is thread-safe (read only).
        /// <para/>
        /// Returns always <c>true</c>.
        /// </summary>
        public override bool ThreadSafe
        {
            get { return true; }
        }

        #endregion // Properties

        #region Methods ================================================================================================

        /// <summary>
        /// When <see cref="AIMSteeringBehaviour.PrepareEvaluation"/> is called, this method is used in order to
        /// transfer the data from <see cref="Target"/> or <see cref="TargetPosition"/> to <see
        /// cref="AIMPerceptBehaviour{T}.GameObjects"/>.
        /// </summary>
        public override void PrepareEvaluation()
        {
            if (FilteredEnvironments.Count != 0)
                FilteredEnvironments.Clear();

            if (GameObjects.Count == 1)
            {
                GameObjects[0] = Target;
            }
            else
            {
                GameObjects.Clear();
                GameObjects.Add(Target);
            }

            base.PrepareEvaluation();

            if (Target == null)
            {
                PerceptBehaviour.Percepts[0].Position = TargetPosition;
                PerceptBehaviour.Percepts[0].Active = true;
                PerceptBehaviour.Percepts[0].Significance = 1f;
            }
        }

        #endregion // Methods
    } // class AIMFollow
} // namespace Polarith.AI.Move
