using UnityEngine;

namespace Polarith.AI.Move
{
    /// <summary>
    /// <see cref="AIMArrive"/> is used in order to modify the velocity of an agent if it reaches a target (front-end
    /// component). Front-end component of the underlying <see cref="Move.Arrive"/> class. This behaviour is
    /// thread-safe.
    /// </summary>
    [AddComponentMenu("Polarith AI » Move/Behaviours/Steering/AIM Arrive")]
    public sealed class AIMArrive : AIMRadiusSteeringBehaviour
    {
        #region Fields =================================================================================================

        /// <summary>
        /// The target game object used by the agent to adapt its velocity for.
        /// </summary>
        [Tooltip("The target game object used by the agent to adapt its velocity for.")]
        public GameObject Target;

        /// <summary>
        /// The underlying back-end behaviour class.
        /// </summary>
        [Tooltip("Allows to specify the settings of this behaviour.")]
        public Arrive Arrive = new Arrive();

        #endregion // Fields

        #region Properties =============================================================================================

        /// <summary>
        /// Polymorphic reference to the underlying back-end class (read only).
        /// </summary>
        public override RadiusSteeringBehaviour RadiusSteeringBehaviour
        {
            get { return Arrive; }
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
        /// This method is used in order to transfer the data from <see cref="Target"/> to <see
        /// cref="AIMPerceptBehaviour{T}.GameObjects"/>. Afterwards, <see
        /// cref="AIMSteeringBehaviour.PrepareEvaluation"/> is called.
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
        }

        //--------------------------------------------------------------------------------------------------------------

#if UNITY_EDITOR

        /// <summary>
        /// Draws gizmos for the inner radius and outer radius of the underlying back-end <see
        /// cref="Move.RadiusSteeringBehaviour"/> within the scene view (editor only).
        /// </summary>
        /// <exception cref="System.NullReferenceException">
        /// If <see cref="RadiusSteeringBehaviour"/> is <c>null</c>.
        /// </exception>
        protected override void OnDrawGizmos()
        {
            if (outerRadiusGizmo.Enabled && Target != null)
                outerRadiusGizmo.Draw(Target.transform.position, transform.rotation, Arrive.OuterRadius);
            if (innerRadiusGizmo.Enabled && Target != null)
                innerRadiusGizmo.Draw(Target.transform.position, transform.rotation, Arrive.InnerRadius);
        }

#endif // UNITY_EDITOR

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Reset to default values.
        /// <para/>
        /// Reset is called when the user hits the Reset button in the Inspector's context menu or when adding the
        /// component the first time. This function is only called in editor mode. Reset is most commonly used to give
        /// good default values in the inspector.
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            Arrive.ValueWriting = ValueWritingType.Subtraction;
            Arrive.InnerRadius = 0f;
            Arrive.OuterRadius = 5f;
        }

        #endregion // Methods
    } // class AIMArrive
} // namespace Polarith.AI.Move
