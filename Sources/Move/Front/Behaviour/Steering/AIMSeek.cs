using UnityEngine;

namespace Polarith.AI.Move
{
    /// <summary>
    /// <see cref="AIMSeek"/> uses the percept's position as the target (front-end component). Front-end component of
    /// the underlying <see cref="Move.Seek"/> class. This behaviour is thread-safe.
    /// </summary>
    [AddComponentMenu("Polarith AI » Move/Behaviours/Steering/AIM Seek")]
    public sealed class AIMSeek : AIMRadiusSteeringBehaviour
    {
        #region Fields =================================================================================================

        /// <summary>
        /// The underlying back-end behaviour class.
        /// </summary>
        [Tooltip("Allows to specify the settings of this behaviour.")]
        public Seek Seek = new Seek();

        #endregion // Fields

        #region Properties =============================================================================================

        /// <summary>
        /// Polymorphic reference to the underlying back-end class (read only).
        /// </summary>
        public override RadiusSteeringBehaviour RadiusSteeringBehaviour
        {
            get { return Seek; }
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
    } // class AIMSeek
} // namespace Polarith.AI.Move
