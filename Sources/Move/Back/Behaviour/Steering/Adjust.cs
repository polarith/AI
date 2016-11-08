using System;
using UnityEngine;

namespace Polarith.AI.Move
{
    /// <summary>
    /// <see cref="Adjust"/> fits an agent's orientation with those of its neighbours (back-end class). This might be
    /// useful in swarming behaviours. The orientation for each percept is obtained by considering the <see
    /// cref="Transform.rotation"/> and the forward vector of the agent. Note, the weighting of the orientations is
    /// applied only if <see cref="ValueWritingType"/> is set to <see cref="ValueWritingType.Addition"/>.
    /// Hint: you might use a third objective in order to restrict the orientation of the agent.
    /// <para/>
    /// Back-end class of <see cref="AIMAdjust"/>. This behaviour is thread-safe.
    /// </summary>
    [Serializable]
    public class Adjust : RadiusSteeringBehaviour
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
        /// In addition to the filter process for each percept in <see cref="RadiusSteeringBehaviour.StartSteering"/>,
        /// the <see cref="SteeringBehaviour.ResultDirection"/> is obtained through the multiplication of <see
        /// cref="SteeringPercept.Rotation"/> with the forward vector of the agent.
        /// <para/>
        /// Note, the forward vector depends on <see cref="VectorProjectionType"/>. If <see
        /// cref="SteeringBehaviour.VectorProjection"/> is <see cref="VectorProjectionType.PlaneXY"/>, then it is <see
        /// cref="Vector3.up"/>, <see cref="Vector3.forward"/> otherwise.
        /// </summary>
        /// <returns>
        /// <c>true</c>: if the percept lies in between the <see cref="RadiusSteeringBehaviour.InnerRadius"/> and <see
        /// cref="RadiusSteeringBehaviour.OuterRadius"/>, <c>false</c>: otherwise.
        /// </returns>
        protected override bool StartSteering()
        {
            // If true, percept is in range
            if (base.StartSteering())
            {
                if (VectorProjection == VectorProjectionType.PlaneXY)
                    ResultDirection = percept.Rotation * Vector3.up;
                else
                    ResultDirection = percept.Rotation * Vector3.forward;

                ResultMagnitude = startMagnitude;
                return true;
            }

            return false;
        }

        #endregion // Methods
    } // class Adjust
} // namespace Polarith.AI.Move
