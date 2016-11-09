using UnityEngine;

namespace Polarith.AI.Move
{
    /// <summary>
    /// <see cref="AIMAdjust"/> fits an agent's orientation with those of its neighbours (front-end component).
    /// Front-end component of the underlying <see cref="Move.Adjust"/> class. This behaviour is thread-safe.
    /// </summary>
    [AddComponentMenu("Polarith AI » Move/Behaviours/Steering/AIM Adjust")]
    public sealed class AIMAdjust : AIMRadiusSteeringBehaviour
    {
        #region Fields =================================================================================================

        /// <summary>
        /// The underlying back-end behaviour class.
        /// </summary>
        [Tooltip("Allows to specify the settings of this behaviour.")]
        public Adjust Adjust = new Adjust();

        #endregion // Fields

        #region Properties =============================================================================================

        /// <summary>
        /// Polymorphic reference to the underlying back-end class (read only).
        /// </summary>
        public override RadiusSteeringBehaviour RadiusSteeringBehaviour
        {
            get { return Adjust; }
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
        /// Reset to default values.
        /// <para/>
        /// Reset is called when the user hits the Reset button in the Inspector's context menu or when adding the
        /// component the first time. This function is only called in editor mode. Reset is most commonly used to give
        /// good default values in the inspector.
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            Adjust.InnerRadius = 0f;
            Adjust.OuterRadius = 5f;
            Adjust.ValueWriting = ValueWritingType.Addition;
            Adjust.VectorProjection = VectorProjectionType.PlaneXY;
        }

        #endregion // Methods
    } // class AIMAdjust
} // namespace Polarith.AI.Move
