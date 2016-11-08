using UnityEngine;

namespace Polarith.AI.Move
{
    /// <summary>
    /// <see cref="Flee"/> uses the opposing direction to the percept as target direction (front-end component).
    /// Front-end component of the underlying <see cref="Move.Flee"/> class. This behaviour is thread-safe.
    /// </summary>
    [AddComponentMenu("Polarith AI » Move/Behaviours/Steering/AIM Flee")]
    public sealed class AIMFlee : AIMRadiusSteeringBehaviour
    {
        #region Fields =================================================================================================

        /// <summary>
        /// The underlying back-end behaviour class.
        /// </summary>
        [Tooltip("Allows to specify the settings of this behaviour.")]
        public Flee Flee = new Flee();

        #endregion // Fields

        #region Properties =============================================================================================

        /// <summary>
        /// Polymorphic reference to the underlying back-end class (read only).
        /// </summary>
        public override RadiusSteeringBehaviour RadiusSteeringBehaviour
        {
            get { return Flee; }
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
    } // class AIMFlee
} // namespace Polarith.AI.Move
