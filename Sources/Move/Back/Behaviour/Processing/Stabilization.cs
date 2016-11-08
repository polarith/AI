using Polarith.AI.Criteria;
using System;
using UnityEngine;

namespace Polarith.AI.Move
{
    /// <summary>
    /// <see cref="Stabilization"/> increases the objective values along the movement direction of the agent (back-end
    /// class). This is useful in order to ensure that the agent retains its current direction. This might be helpful in
    /// situations where the <see cref="AIMContext.DecidedDirection"/> varies widely from frame to frame. For that, a
    /// value around <see cref="MaxIncrease"/> is added to those objective values whose receptors' <see
    /// cref="Structure.Direction"/> matches more or less with <see cref="LocalDirection"/>. This direction can be set
    /// in two different ways: first, by the velocity of the agent, second, by the user.
    /// <para/>
    /// The default parametrization of this class increases the objective values along the movement direction by a value
    /// of 0.3. All receptors where the <see cref="Structure.Direction"/> differs more than 45 degrees to the movement
    /// direction are ignored.
    /// <para/>
    /// Back-end class of <see cref="AIMStabilization"/>. This behaviour is thread-safe.
    /// </summary>
    [Serializable]
    public class Stabilization : MoveBehaviour
    {
        #region Fields =================================================================================================

        /// <summary>
        /// Specifies the local direction which is used to modify the objective values.
        /// </summary>
        [Tooltip("Specifies the local direction which is used to modify the objective values.")]
        public Vector3 LocalDirection = Vector3.up;

        /// <summary>
        /// Sets the mapping type for obtaining objective values.
        /// </summary>
        [Tooltip("Sets the mapping type for obtaining objective values.")]
        public MappingType AngleMapping = MappingType.InverseLinear;

        /// <summary>
        /// Defines the objective for writing values.
        /// </summary>
        [Tooltip("Defines the objective for writing values.")]
        public int TargetObjective = 0;

        /// <summary>
        /// Determines the maximum possible addition.
        /// </summary>
        [Tooltip("Determines the maximum possible addition.")]
        public float MaxIncrease = 0.3f;

        /// <summary>
        /// Determines the maximum possible deviation in degrees of <see cref="LocalDirection"/>.
        /// </summary>
        [Tooltip("Determines the maximum possible deviation in degrees of 'LocalDirection'.")]
        [Range(0f, 180f)]
        public float MaxAngle = 45f;

        /// <summary>
        /// If <c>true</c>, the velocity of the agent is used as the <see cref="LocalDirection"/>.
        /// </summary>
        [Tooltip("If 'true', the velocity of the agent is used as the 'LocalDirection'.")]
        public bool UseVelocity;

        // -------------------------------------------------------------------------------------------------------------

        private readonly SteeringPercept self = new SteeringPercept();

        #endregion // Fields

        #region Properties =============================================================================================

        /// <summary>
        /// The data of the associated agent itself.
        /// </summary>
        public SteeringPercept Self
        {
            get { return self; }
            set { self.Copy(value); }
        }

        #endregion // Properties

        #region Methods ================================================================================================

        /// <summary>
        /// This method executes the main algorithm of this behaviour and is called within <see
        /// cref="Context.Evaluate"/> in order to set/modify objective values for the associated <see
        /// cref="Context.Problem"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">
        /// If the inherited <see cref="MoveBehaviour.Context"/> or its associated <see cref="Context.Problem"/>
        /// respectively <see cref="Context{TValue, TStructure}.Sensor"/> are <c>null</c>.
        /// </exception>
        public override void Behave()
        {
            if (TargetObjective < 0 || TargetObjective >= Context.Problem.ObjectiveCount)
                return;

            ISensor<Structure> sensor = Context.Sensor;
            for (int i = 0; i < sensor.ReceptorCount; i++)
            {
                float angle;
                if (UseVelocity)
                    angle = Vector3.Angle(Context.WorldToLocalMatrix * self.Velocity, sensor[i].Structure.Direction);
                else
                    angle = Vector3.Angle(LocalDirection, sensor[i].Structure.Direction);

                if (angle <= MaxAngle)
                {
                    WriteValue(ValueWritingType.Addition, TargetObjective, i,
                        MapSpecial(AngleMapping, 0f, MaxAngle, angle) * MaxIncrease);
                }
            }
        }

        #endregion // Methods
    } // class Stabilization
} // namespace Polarith.AI.Move
