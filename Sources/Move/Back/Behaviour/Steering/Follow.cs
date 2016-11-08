using System;

namespace Polarith.AI.Move
{
    /// <summary>
    /// By using <see cref="Follow"/> the agent follows one target independent on its distance to the agent (back-end
    /// class). It functions analogously to the usual steering behaviour 'Follow'. As the name implies, the purpose of
    /// the behaviour is to follow one given <see cref="AIMFollow.Target"/>, this is independent of its location or
    /// velocity. In order to combine <see cref="Follow"/> with a pathfinding algorithm, you can set the <see
    /// cref="AIMFollow.TargetPosition"/> via script. Note, if <see cref="AIMFollow.Target"/> is <c>null</c>, <see
    /// cref="AIMFollow.TargetPosition"/> is used as the next target position.
    /// <para/>
    /// Back-end class of <see cref="AIMFollow"/>. This behaviour is thread-safe.
    /// </summary>
    [Serializable]
    public class Follow : SteeringBehaviour
    {
        #region Properties =============================================================================================

        /// <summary>
        /// Determines if <see cref="SteeringBehaviour.PerceptSteering"/> is called within <see
        /// cref="SteeringBehaviour.Behave"/> (read only).
        /// <para/>
        /// Returns always <c>false</c> because all the work is done within <see
        /// cref="SteeringBehaviour.StartSteering"/>.
        /// </summary>
        protected override bool forEachPercept
        {
            get { return false; }
        }

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Determines if <see cref="SteeringBehaviour.ReceptorSteering"/> is called within <see
        /// cref="SteeringBehaviour.Behave"/> (read only).
        /// <para/>
        /// Returns always <c>false</c> because all the work is done within <see
        /// cref="SteeringBehaviour.StartSteering"/>.
        /// </summary>
        protected override bool forEachReceptor
        {
            get { return false; }
        }

        #endregion // Properties

        #region Methods ================================================================================================

        /// <summary>
        /// Computes the <see cref="SteeringBehaviour.ResultDirection"/> through the vector subtraction of the target
        /// position and the agent position. The <see cref="SteeringBehaviour.ResultMagnitude"/> is always one. In order
        /// to weight the objective values in contrast to other <see cref="SteeringBehaviour"/>, you might use the <see
        /// cref="SteeringBehaviour.MagnitudeMultiplier"/>.
        /// </summary>
        /// <returns>Returns always <c>true</c>.</returns>
        protected override bool StartSteering()
        {
            ResultDirection = percept.Position - self.Position;
            ResultMagnitude = 1f;

            return true;
        }

        #endregion // Methods
    } // class Follow
} // namespace Polarith.AI.Move
