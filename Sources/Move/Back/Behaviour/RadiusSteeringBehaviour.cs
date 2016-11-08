using UnityEngine;

namespace Polarith.AI.Move
{
    /// <summary>
    /// This class extends the <see cref="SteeringBehaviour"/> through adding a kind of local perception model based on
    /// an <see cref="InnerRadius"/> and <see cref="OuterRadius"/> (back-end class). Only the percepts lying between
    /// these radii are important for an agent. If this is not the case, <see cref="StartSteering"/> returns
    /// <c>false</c> and the percept is skipped. This improves performance because the computation-heavy parts of the
    /// inherited <see cref="SteeringBehaviour.Behave"/> are skipped, too. All derived <see
    /// cref="RadiusSteeringBehaviour"/> classes are intended to map the inherited <see
    /// cref="SteeringBehaviour.ResultMagnitude"/> accordingly to the position of the processed percept relative to the
    /// those two radii. The function for this mapping can be controlled via <see cref="RadiusMapping"/>.
    /// <para/>
    /// Every derived <see cref="RadiusSteeringBehaviour"/> needs to implement the properties <see
    /// cref="SteeringBehaviour.forEachPercept"/> and <see cref="SteeringBehaviour.forEachReceptor"/> to determine if
    /// the corresponding methods <see cref="SteeringBehaviour.PerceptSteering"/> and <see
    /// cref="SteeringBehaviour.ReceptorSteering"/> are called within <see cref="SteeringBehaviour.Behave"/>. Usually,
    /// you might only want either <see cref="SteeringBehaviour.PerceptSteering"/> or <see
    /// cref="SteeringBehaviour.ReceptorSteering"/> to be called at once.
    /// <para/>
    /// Besides the inherited references for quick data access of the <see cref="SteeringBehaviour"/> class, this class
    /// defines the following references for quick access to be used within the <see
    /// cref="SteeringBehaviour.PerceptSteering"/> and <see cref="SteeringBehaviour.ReceptorSteering"/> implementation
    /// in derived classes: <see cref="startDirection"/>, <see cref="startMagnitude"/>, <see cref="sqrInnerRadius"/> and
    /// <see cref="sqrOuterRadius"/>.
    /// <para/>
    /// A <see cref="RadiusSteeringBehaviour"/> can be marked as thread-safe in its corresponding front-end class
    /// through setting <see cref="AIMBehaviour.ThreadSafe"/> to <c>true</c>. If you do this, be sure not to use
    /// reference types of the Unity scripting API and not to make any possibly unsafe variable writes. If at least one
    /// behaviour is not marked <see cref="AIMBehaviour.ThreadSafe"/>, the whole agent will not be part of the
    /// multithreading in <see cref="AIMThreading"/>.
    /// <para/>
    /// Base back-end class of every derived <see cref="AIMRadiusSteeringBehaviour"/>.
    /// </summary>
    public abstract class RadiusSteeringBehaviour : SteeringBehaviour
    {
        #region Fields =================================================================================================

        /// <summary>
        /// Determines how the <see cref="startMagnitude"/> is mapped according to the <see cref="InnerRadius"/> and
        /// <see cref="OuterRadius"/>. The <see cref="startMagnitude"/> might be used to compute the <see
        /// cref="SteeringBehaviour.ResultMagnitude"/>.
        /// </summary>
        [Tooltip("Influences how the result magnitude is mapped according to the 'InnerRadius' and 'OuterRadius'.")]
        public MappingType RadiusMapping = MappingType.InverseLinear;

        /// <summary>
        /// The minimum radius for considering percepts. If a percept lies below this threshold, it is ignored by this
        /// behaviour.
        /// </summary>
        [Tooltip("The minimum radius for considering percepts. If a percept lies below this threshold, it is ignored " +
            "by this behaviour.")]
        public float InnerRadius = 0.001f;

        /// <summary>
        /// The maximum radius for considering percepts. If a percept lies above this threshold, it is ignored by this
        /// behaviour.
        /// </summary>
        [Tooltip("The maximum radius for considering percepts. If a percept lies above this threshold, it is ignored " +
            "by this behaviour.")]
        public float OuterRadius = 20f;

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Direction from the self position to the percept position (including distance magnitude).
        /// <para/>
        /// Might be used in <see cref="SteeringBehaviour.PerceptSteering"/> and <see
        /// cref="SteeringBehaviour.ReceptorSteering"/>.
        /// </summary>
        protected Vector3 startDirection;

        /// <summary>
        /// Magnitude obtained from mapping the percept position relative to <see cref="InnerRadius"/> and <see
        /// cref="OuterRadius"/>.
        /// <para/>
        /// Might be used in <see cref="SteeringBehaviour.PerceptSteering"/> and <see
        /// cref="SteeringBehaviour.ReceptorSteering"/>.
        /// </summary>
        protected float startMagnitude;

        /// <summary>
        /// Squared <see cref="InnerRadius"/>.
        /// <para/>
        /// Might be used in <see cref="SteeringBehaviour.PerceptSteering"/> and <see
        /// cref="SteeringBehaviour.ReceptorSteering"/>.
        /// </summary>
        protected float sqrInnerRadius;

        /// <summary>
        /// Squared <see cref="OuterRadius"/>.
        /// <para/>
        /// Might be used in <see cref="SteeringBehaviour.PerceptSteering"/> and <see
        /// cref="SteeringBehaviour.ReceptorSteering"/>.
        /// </summary>
        protected float sqrOuterRadius;

        #endregion // Fields

        #region Methods ================================================================================================

        /// <summary>
        /// Sets the <see cref="startDirection"/> and <see cref="startMagnitude"/> accordingly to the position of the
        /// processed percept relative to the <see cref="InnerRadius"/> and <see cref="OuterRadius"/> of this behaviour.
        /// In addition, it computes and pre-caches <see cref="sqrInnerRadius"/> and <see cref="sqrOuterRadius"/>.
        /// <para/>
        /// All these pre-cached values might be accessed within <see cref="SteeringBehaviour.PerceptSteering"/> and
        /// <see cref="SteeringBehaviour.ReceptorSteering"/> for derived classes in order to ease further computations.
        /// </summary>
        /// <returns>
        /// <c>true</c>: if the percept lies in between the <see cref="InnerRadius"/> and <see cref="OuterRadius"/>,
        /// <c>false</c>: otherwise.
        /// </returns>
        protected override bool StartSteering()
        {
            // Compute direction from agent to percept
            startDirection = percept.Position - self.Position;

            // Compute squared radii
            sqrInnerRadius = InnerRadius * InnerRadius;
            sqrOuterRadius = OuterRadius * OuterRadius;

            // Check if the currently processed percept is relevant and abort if it is not
            if (startDirection.sqrMagnitude < sqrInnerRadius || startDirection.sqrMagnitude > sqrOuterRadius)
                return false;

            // Map distance to the percept and seek radii as specified
            startMagnitude = MapSpecialSqr(RadiusMapping, sqrInnerRadius, sqrOuterRadius, startDirection.sqrMagnitude);

            // The percept is relevant
            return true;
        }

        #endregion // Methods
    } // class RadiusSteeringBehaviour
} // class Polarith.AI.Move
