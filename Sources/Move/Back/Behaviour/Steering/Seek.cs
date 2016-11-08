using System;

namespace Polarith.AI.Move
{
    /// <summary>
    /// <see cref="Seek"/> uses the percept's position as the target (back-end class). The godfather of all (context)
    /// steering algorithms. <see cref="Seek"/> simply computes a direction to the currently targeted percept and uses
    /// the perception radii of its base class <see cref="RadiusSteeringBehaviour"/> to decide whether a percept is
    /// important or if it can be ignored. The result magnitude is computed accordingly by using the position of the
    /// percept relative to the perception radii.
    /// <para/>
    /// This behaviour populates the <see cref="Context.Problem"/> with objective values which reflect the directly
    /// sampled positions of the processed percepts. The following holds true for the default parametrization: the
    /// closer a percept is, the greater the resulting objective values will be, and the sensor receptor which direction
    /// matches the direction towards the processed percept best is obtaining the greatest objective value.
    /// <para/>
    /// Back-end class of <see cref="AIMSeek"/>. This behaviour is thread-safe.
    /// </summary>
    [Serializable]
    public class Seek : RadiusSteeringBehaviour
    {
        #region Fields =================================================================================================

        /// <summary>
        /// If set to <c>true</c> the algorithm is applied for each receptor instead for each percept. This might be
        /// useful if the receptors of the associated sensor have got a different position.
        /// </summary>
        public bool ForEachReceptor;

        #endregion // Fields

        #region Properties =============================================================================================

        /// <summary>
        /// Determines if <see cref="PerceptSteering"/> is called within <see cref="SteeringBehaviour.Behave"/>
        /// (read only).
        /// </summary>
        protected override bool forEachPercept
        {
            get { return !ForEachReceptor; }
        }

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Determines if <see cref="ReceptorSteering"/> is called within <see cref="SteeringBehaviour.Behave"/>
        /// (read only).
        /// </summary>
        protected override bool forEachReceptor
        {
            get { return ForEachReceptor; }
        }

        #endregion // Properties

        #region Methods ================================================================================================

        /// <summary>
        /// Processes the steering algorithm for each percept using the same data for each processed receptor.
        /// </summary>
        protected override void PerceptSteering()
        {
            // Use agent's pivot point for computing the direction
            ResultDirection = startDirection;

            // Map distance to the percept and seek radii
            ResultMagnitude = startMagnitude;
        }

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Processes the steering algorithm for each receptor for each percept separately. This might be useful if the
        /// receptors of the associated sensor have got a different position.
        /// </summary>
        protected override void ReceptorSteering()
        {
            // Use receptor's position for computing the direction
            ResultDirection = percept.Position - self.Position - structure.Position;

            // Map distance to the percept and seek radii
            ResultMagnitude = MapSpecialSqr(
                RadiusMapping, sqrInnerRadius, sqrOuterRadius, ResultDirection.sqrMagnitude);
        }

        #endregion // Methods
    } // class Seek
} // namespace Polarith.AI.Move
