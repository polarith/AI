using System;

namespace Polarith.AI.Move
{
    /// <summary>
    /// <see cref="Flee"/> uses the opposing direction to the percept as target direction (back-end class). It inherits
    /// from <see cref="Seek"/> and works like its base class except that it negates the result direction for obtaining
    /// objective values.
    /// <para/>
    /// Back-end class of <see cref="AIMFlee"/>. This behaviour is thread-safe.
    /// </summary>
    [Serializable]
    public class Flee : Seek
    {
        #region Methods ================================================================================================

        /// <summary>
        /// Processes the steering algorithm for each percept using the same data for each processed receptor.
        /// <para/>
        /// First, <see cref="Seek.PerceptSteering"/> is called, and then, the obtained <see
        /// cref="SteeringBehaviour.ResultDirection"/> is negated.
        /// </summary>
        protected override void PerceptSteering()
        {
            base.PerceptSteering();
            ResultDirection *= -1f;
        }

        // -------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Processes the steering algorithm for each receptor for each percept separately. This might be useful if the
        /// receptors of the associated sensor have got a different position.
        /// <para/>
        /// First, <see cref="Seek.ReceptorSteering"/> is called, and then, the obtained <see
        /// cref="SteeringBehaviour.ResultDirection"/> is negated.
        /// </summary>
        protected override void ReceptorSteering()
        {
            base.ReceptorSteering();
            ResultDirection *= -1f;
        }

        #endregion // Methods
    } // class Flee
} // namespace Polarith.AI.Move
