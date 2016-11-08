using System;
using UnityEngine;

namespace Polarith.AI.Move
{
    /// <summary>
    /// <see cref="Align"/> fits an agent's orientation to the orientation of one target percept (back-end class). It
    /// functions analogously to the usual steering behaviour 'Alignment'. Therefore, <see cref="AIMAlign.Target"/> is
    /// used. The orientation of the target percept is obtained using the <see cref="Transform.rotation"/> and the
    /// forward vector of the agent. The <see cref="SteeringBehaviour.ResultMagnitude"/> is always one. In order to
    /// weight the objective values with other steering behaviours, you might use <see
    /// cref="SteeringBehaviour.MagnitudeMultiplier"/>. Note, the behaviour concentrates only on one target at a time.
    /// <para/>
    /// Back-end class of <see cref="AIMAlign"/>. This behaviour is thread-safe.
    /// </summary>
    [Serializable]
    public class Align : SteeringBehaviour
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
        /// The <see cref="SteeringBehaviour.ResultDirection"/> is obtained by the multiplication of <see
        /// cref="SteeringPercept.Rotation"/> with the forward vector of the agent.
        /// <para/>
        /// Note, the forward vector depends on <see cref="VectorProjectionType"/>. If <see
        /// cref="SteeringBehaviour.VectorProjection"/> is <see cref="VectorProjectionType.PlaneXY"/>, it is <see
        /// cref="Vector3.up"/>, <see cref="Vector3.forward"/> otherwise.
        /// </summary>
        /// <returns>
        /// Always <c>true</c> because only one percept is considered independent with respect to its location relative
        /// to the agent.
        /// </returns>
        protected override bool StartSteering()
        {
            if (VectorProjection == VectorProjectionType.PlaneXY)
                ResultDirection = percept.Rotation * Vector3.up;
            else
                ResultDirection = percept.Rotation * Vector3.forward;

            ResultMagnitude = 1f;
            return true;
        }

        #endregion // Methods
    } // class Align
} // namespace Polarith.AI.Move
