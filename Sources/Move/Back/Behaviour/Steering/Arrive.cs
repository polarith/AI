using Polarith.Utils;
using System;
using UnityEngine;

namespace Polarith.AI.Move
{
    /// <summary>
    /// <see cref="Arrive"/> is used in order to modify the velocity of an agent if it reaches a target (back-end
    /// class). It functions analogously to the usual steering behaviour 'Arrive'. The procedure is activated if the
    /// agent is within the <see cref="RadiusSteeringBehaviour.OuterRadius"/> and <see
    /// cref="RadiusSteeringBehaviour.InnerRadius"/>, otherwise, the velocity is constant. The normal use of this
    /// behaviour would be to reduce the velocity but it can also be used to increase it. The velocity modification is
    /// reached by multiplication of <see cref="SteeringBehaviour.ResultMagnitude"/> with the velocity, like in <see
    /// cref="AIMSimpleController2D.Update"/>. Be sure that you use <see cref="AIMContext.DecidedValues"/> and not to
    /// use the <see cref="AIMContext.DecidedDirection"/> in order to modify the velocity if you use one of our
    /// controllers or your own.
    /// <para/>
    /// Note, the default mapping maps a value between the two radii. If you want to add you own mapping, inherit from
    /// <see cref="Arrive"/> and implement <see cref="SteeringBehaviour.StartSteering"/>. The <see
    /// cref="SteeringBehaviour.ResultDirection"/> is the direction to the <see cref="AIMArrive.Target"/> and the <see
    /// cref="SteeringBehaviour.ResultMagnitude"/> is used as a multiplier for the velocity. Furthermore, the behaviour
    /// concentrates only on one target at a time.
    /// <para/>
    /// Back-end class of <see cref="AIMArrive"/>. This behaviour is thread-safe.
    /// </summary>
    [Serializable]
    public class Arrive : RadiusSteeringBehaviour
    {
        #region Fields =================================================================================================

        /// <summary>
        /// The default multiplier for the velocity if the agent is outside of the radii interval.
        /// </summary>
        [Tooltip("The default multiplier for the velocity if the agent is outside of the radii interval.")]
        public float BaseMagnitude;

        #endregion // Fields

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
        /// The <see cref="SteeringBehaviour.ResultDirection"/> is the direction to the desired <see
        /// cref="AIMArrive.Target"/>. The <see cref="SteeringBehaviour.ResultMagnitude"/> is obtained by mapping the
        /// current position of the agent to the two given radii.
        /// <para/>
        /// Note, with an inverse <see cref="RadiusSteeringBehaviour.RadiusMapping"/> the following holds true: the
        /// closer the agent is to the target, the greater the velocity increases.
        /// </summary>
        /// <returns>Returns always <c>true</c>.</returns>
        protected override bool StartSteering()
        {
            ResultDirection = percept.Position - self.Position;
            sqrOuterRadius = OuterRadius * OuterRadius;
            sqrInnerRadius = InnerRadius * InnerRadius;

            // If true, then agent is within the radius
            if (ResultDirection.sqrMagnitude < sqrOuterRadius)
            {
                // If true, then decreasing
                if (RadiusMapping == MappingType.Linear || RadiusMapping == MappingType.Quadratic ||
                    RadiusMapping == MappingType.SquareRoot)
                {
                    ResultMagnitude = Mathf2.MapLinear(0f, BaseMagnitude, 0f, 1f,
                        MapSpecialSqr(RadiusMapping, sqrInnerRadius, sqrOuterRadius, ResultDirection.sqrMagnitude));
                }
                else
                {
                    ResultMagnitude = Mathf2.MapLinear(BaseMagnitude, 1f, 0f, 1f,
                        MapSpecialSqr(RadiusMapping, sqrInnerRadius, sqrOuterRadius, ResultDirection.sqrMagnitude));
                }
            }
            else
            {
                ResultMagnitude = BaseMagnitude;
            }

            return true;
        }

        #endregion // Methods
    } // class Arrive
} // namespace Polarith.AI.Move
