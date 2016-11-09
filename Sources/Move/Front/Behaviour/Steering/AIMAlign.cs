using UnityEngine;

namespace Polarith.AI.Move
{
    /// <summary>
    /// <see cref="AIMAlign"/> fits an agent's orientation to the orientation of one target percept (front-end
    /// component). Note, changes to the inherited <see cref="AIMPerceptBehaviour{T}.FilteredEnvironments"/> and <see
    /// cref="AIMPerceptBehaviour{T}.GameObjects"/> fields have no effect, since they are reset <see
    /// cref="PrepareEvaluation"/>.
    /// <para/>
    /// Front-end component of the underlying <see cref="Move.Align"/> class. This behaviour is thread-safe.
    /// </summary>
    [AddComponentMenu("Polarith AI » Move/Behaviours/Steering/AIM Align")]
    public sealed class AIMAlign : AIMSteeringBehaviour
    {
        #region Fields =================================================================================================

        /// <summary>
        /// The <see cref="SteeringBehaviour.ResultDirection"/> matches with the orientation of this game object.
        /// </summary>
        [Tooltip("The 'ResultDirection' matches with the orientation of this game object.")]
        public GameObject Target;

        /// <summary>
        /// The underlying back-end behaviour class.
        /// </summary>
        [Tooltip("Allows to specify the settings of this behaviour.")]
        public Align Align = new Align();

        #endregion // Fields

        #region Properties =============================================================================================

        /// <summary>
        /// Polymorphic reference to the underlying back-end class (read only).
        /// </summary>
        public override SteeringBehaviour SteeringBehaviour
        {
            get { return Align; }
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
            Align.VectorProjection = VectorProjectionType.PlaneXY;
        }

        #endregion // Methods
    } // class AIMAlign
} // namespace Polarith.AI.Move
