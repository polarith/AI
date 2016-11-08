using Polarith.AI.Criteria;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Polarith.AI.Move
{
    /// <summary>
    /// <see cref="SteeringBehaviour"/> provides the base functionality for writing and mapping objective values
    /// (back-end class). This derived <see cref="PerceptBehaviour{T}"/> is the very foundation for all context steering
    /// algorithms. The objective values are written into an associated <see cref="Context.Problem"/> which is evaluated
    /// in <see cref="Context.Evaluate"/> based on the inherited <see cref="PerceptBehaviour{T}.Percepts"/> and <see
    /// cref="PerceptBehaviour{T}.Self"/>. In order to obtain and write objective values, the three methods <see
    /// cref="StartSteering"/>, <see cref="PerceptSteering"/> and <see cref="ReceptorSteering"/> are used for setting up
    /// an appropriate <see cref="ResultDirection"/> and <see cref="ResultMagnitude"/>, whereby the switches <see
    /// cref="ValueMapping"/> and <see cref="ValueWriting"/> determine how objective values are written. In addition,
    /// with the help of <see cref="VectorProjection"/> it is possible to project the perceived vector data which are
    /// necessary for obtaining objective values into a specified plane. This might be useful, for example, when there
    /// is a (virtual) ground and height map in the world, so this is always the case when the scene objects differ
    /// significantly in height.
    /// <para/>
    /// Every derived <see cref="SteeringBehaviour"/> needs to implement the properties <see cref="forEachPercept"/> and
    /// <see cref="forEachReceptor"/> to determine if the corresponding methods <see cref="PerceptSteering"/> and <see
    /// cref="ReceptorSteering"/> are called within <see cref="Behave"/>. Usually, you might only want either <see
    /// cref="PerceptSteering"/> or <see cref="ReceptorSteering"/> to be called at once.
    /// <para/>
    /// A <see cref="SteeringBehaviour"/> can be marked as thread-safe in its corresponding front-end class through
    /// setting <see cref="AIMBehaviour.ThreadSafe"/> to <c>true</c>. If you do this, be sure not to use reference types
    /// of the Unity scripting API and not to make any possibly unsafe variable writes. If at least one behaviour is not
    /// marked <see cref="AIMBehaviour.ThreadSafe"/>, the whole agent will not be part of the multithreading in <see
    /// cref="AIMThreading"/>.
    /// <para/>
    /// Base back-end class of every derived <see cref="AIMSteeringBehaviour"/>.
    /// </summary>
    public abstract class SteeringBehaviour : PerceptBehaviour<SteeringPercept>
    {
        #region Fields =================================================================================================

        /// <summary>
        /// The direction vector used for obtaining objective values. It gets compared to the direction vectors of every
        /// sensor receptor in order to determine the resulting objective value.
        /// </summary>
        [NonSerialized]
        public Vector3 ResultDirection;

        /// <summary>
        /// The magnitude value used for obtaining objective values. It is used as weight when comparing the <see
        /// cref="ResultDirection"/> to receptor directions.
        /// </summary>
        [NonSerialized]
        public float ResultMagnitude;

        /// <summary>
        /// Sets the mapping type for obtaining objective values.
        /// </summary>
        [Tooltip("Sets the mapping type for obtaining objective values.")]
        public MappingType ValueMapping = MappingType.InverseLinear;

        /// <summary>
        /// Sets the type for writing objective values.
        /// </summary>
        [Tooltip("Sets the type for writing objective values.")]
        public ValueWritingType ValueWriting = ValueWritingType.AssignGreater;

        /// <summary>
        /// Sets the type for projecting the perceived vector data into a plane.
        /// </summary>
        [Tooltip("Sets the type for projecting the perceived vector data into a plane.")]
        public VectorProjectionType VectorProjection = VectorProjectionType.None;

        /// <summary>
        /// Defines the objective for writing values.
        /// </summary>
        [Tooltip("Defines the objective for writing values.")]
        public int TargetObjective;

        /// <summary>
        /// Is multiplied to the <see cref="ResultMagnitude"/> in order to weight between different behaviours.
        /// </summary>
        [Tooltip("Is multiplied to the result magnitude in order to weight between different behaviours.")]
        public float MagnitudeMultiplier = 1f;

        /// <summary>
        /// Is added to the <see cref="Structure.Sensitivity"/> as threshold for writing objective values.
        /// </summary>
        [Tooltip("Is added to the sensitivity of a structure as threshold for writing objective values.")]
        public float SensitivityOffset;

        /// <summary>
        /// Determines if the <see cref="SteeringPercept.Significance"/> (if there is a <see cref="AIMSteeringTag"/>) is
        /// multiplied to the <see cref="ResultMagnitude"/> in order to weight between different percepts.
        /// </summary>
        [Tooltip("Determines if the significance of the perceived object (if there is a steering tag) is multiplied " +
            "to the result magnitude in order to weight between different percepts.")]
        public bool UseSignificance = true;

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// The data of the associated agent itself (read only).
        /// <para/>
        /// This is valid to use within: <see cref="StartSteering"/>, <see cref="PerceptSteering"/> and <see
        /// cref="ReceptorSteering"/>.
        /// </summary>
        protected readonly SteeringPercept self = new SteeringPercept();

        /// <summary>
        /// The data of the currently processed percept (read only).
        /// <para/>
        /// This is valid to use within: <see cref="StartSteering"/>, <see cref="PerceptSteering"/> and <see
        /// cref="ReceptorSteering"/>.
        /// </summary>
        protected readonly SteeringPercept percept = new SteeringPercept();

        /// <summary>
        /// Quick access to the currently processed objective.
        /// <para/>
        /// This is valid to use within: <see cref="StartSteering"/>, <see cref="PerceptSteering"/> and <see
        /// cref="ReceptorSteering"/>.
        /// </summary>
        protected IList<float> objective;

        /// <summary>
        /// Quick access to the currently processed sensor.
        /// <para/>
        /// This is valid to use within: <see cref="StartSteering"/>, <see cref="PerceptSteering"/> and <see
        /// cref="ReceptorSteering"/>.
        /// </summary>
        protected ISensor<Structure> sensor;

        /// <summary>
        /// Quick access to the currently processed receptor.
        /// <para/>
        /// This is valid to use only within: <see cref="ReceptorSteering"/>.
        /// </summary>
        protected IReceptor<Structure> receptor;

        /// <summary>
        /// Quick access to the currently processed structure.
        /// <para/>
        /// This is valid to use only within: <see cref="ReceptorSteering"/>.
        /// </summary>
        protected Structure structure;

        #endregion // Fields

        #region Properties =============================================================================================

        /// <summary>
        /// Determines if <see cref="PerceptSteering"/> is called within <see cref="Behave"/> (read only).
        /// <para/>
        /// Usually, you might only want either <see cref="PerceptSteering"/> or <see cref="ReceptorSteering"/> to be
        /// called at once. In some cases, a behaviour only needs to call <see cref="StartSteering"/>, e.g. like <see
        /// cref="Align"/> and <see cref="Wander"/>.
        /// </summary>
        protected abstract bool forEachPercept { get; }

        /// <summary>
        /// Determines if <see cref="ReceptorSteering"/> is called within <see cref="Behave"/> (read only).
        /// <para/>
        /// Usually, you might only want either <see cref="PerceptSteering"/> or <see cref="ReceptorSteering"/> to be
        /// called at once. In some cases, a behaviour only needs to call <see cref="StartSteering"/>, e.g. like <see
        /// cref="Align"/> and <see cref="Wander"/>.
        /// </summary>
        protected abstract bool forEachReceptor { get; }

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// The data of the associated agent itself. When assigned, this properties deep copies the data of the given
        /// instance into an internal hold <see cref="SteeringPercept"/> instance to ensure thread-safety.
        /// </summary>
        public override SteeringPercept Self
        {
            get { return self; }
            set { self.Copy(value); }
        }

        #endregion // Properties

        #region Methods ================================================================================================

        /// <summary>
        /// This method executes the main context steering algorithm and is called within <see cref="Context.Evaluate"/>
        /// in order to set/modify objective values for the associated <see cref="Context.Problem"/>. It pre-caches
        /// multiple references for quick access in the three called steering methods, projects vector data as specified
        /// with <see cref="VectorProjection"/> and finally obtain/write objective values based on the set <see
        /// cref="ValueMapping"/> and <see cref="ValueWriting"/>.
        /// <para/>
        /// For each existing instance in <see cref="PerceptBehaviour{T}.Percepts"/>, the <see cref="ResultDirection"/>
        /// and <see cref="ResultMagnitude"/> are used for obtaining the objective values so that they need to be set
        /// through the implementation of <see cref="StartSteering"/>, <see cref="PerceptSteering"/> and <see
        /// cref="ReceptorSteering"/> in derived <see cref="SteeringBehaviour"/> classes.
        /// <para/>
        /// The final objective value which is written for one corresponding receptor is a sum which is obtained by
        /// weighting the angle between the <see cref="ResultDirection"/> and receptor direction, considering the <see
        /// cref="ResultMagnitude"/> and, optionally, the <see cref="SteeringPercept.Significance"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">
        /// If the inherited <see cref="MoveBehaviour.Context"/> or its associated <see cref="Context.Problem"/>
        /// respectively <see cref="Context{TValue, TStructure}.Sensor"/> are <c>null</c>.
        /// </exception>
        public override void Behave()
        {
            if (TargetObjective < 0 || TargetObjective >= Context.Problem.ObjectiveCount)
                return;

            // Pre-caching (for derived behaviours)
            objective = Context.Problem.GetObjective(TargetObjective);
            sensor = Context.Sensor;

            // Project self vectors if necessary
            self.Project(VectorProjection);

            for (int i = 0; i < Percepts.Count; i++)
            {
                if (!Percepts[i].Active)
                    continue;

                // Copy (for derived behaviours)
                percept.Copy(Percepts[i]);

                // Project percept vectors if necessary
                percept.Project(VectorProjection);

                // Inject custom steering code at the beginning
                if (!StartSteering())
                    continue;

                // Inject custom steering code per percept
                if (forEachPercept)
                    PerceptSteering();

                for (int j = 0; j < sensor.ReceptorCount; j++)
                {
                    // Pre-caching (for derived behaviours)
                    receptor = sensor[j];
                    structure = receptor.Structure;

                    // Inject custom steering code per receptor
                    if (forEachReceptor)
                        ReceptorSteering();

                    // Compute and write objective value
                    float value = (UseSignificance ? percept.Significance : 1f) *
                        MagnitudeMultiplier * structure.Magnitude * ResultMagnitude *
                        MapBySensitivity(ValueMapping, structure, ResultDirection, SensitivityOffset);
                    WriteValue(ValueWriting, TargetObjective, receptor.ID, value);
                }
            }
        }

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets called at the beginning of the context steering algorithm for each processed percept and should be used
        /// to pre-compute things like distances or to check whether a percept is relevant at all. If a percept is not
        /// relevant, e.g. because it is too far away, this method should return <c>false</c> (otherwise, <c>true</c>)
        /// in order to force <see cref="Behave"/> to skip all further and maybe performance-heavy computations and to
        /// continue with the next percept. Furthermore, there might be some behaviours which require only this method
        /// to work properly, e.g. <see cref="Align"/> or <see cref="Wander"/>.
        /// <para/>
        /// To improve performance, things which remain constant for one percept, e.g. central distances, can be
        /// computed in this method and re-used in <see cref="PerceptSteering"/> and <see cref="ReceptorSteering"/>.
        /// Especially for the latter, this might improve the performance a lot.
        /// <para/>
        /// Within this method, the following references can be used for quick data access: <see cref="self"/>, <see
        /// cref="percept"/>, <see cref="objective"/> and <see cref="sensor"/>.
        /// <para/>
        /// To make a <see cref="SteeringBehaviour"/> thread-safe, only <see cref="self"/> and <see cref="percept"/>
        /// must be accessed by writes. All other references for quick access are not thread-safe. Furthermore, to be
        /// thread-safe, no reference types of the Unity scripting API must be used within this method. If you fulfil
        /// these restrictions, you can mark the derived <see cref="SteeringBehaviour"/> class as thread-safe through
        /// setting <see cref="AIMBehaviour.ThreadSafe"/> to <c>true</c> for its corresponding front-end class.
        /// </summary>
        /// <returns><c>true</c>: if the processed percept is relevant for the agent, <c>false</c>: otherwise.</returns>
        protected virtual bool StartSteering()
        {
            return true;
        }

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// This method gets called once for each processed percept. Its purpose is to set an appropriate <see
        /// cref="ResultDirection"/> and <see cref="ResultMagnitude"/> for obtaining/writing objective values. So the
        /// implementation of this method determines what kind of AI movement behaviour a derived <see
        /// cref="SteeringBehaviour"/> class really is.
        /// <para/>
        /// Within this method, the following references can be used for quick data access: <see cref="self"/>, <see
        /// cref="percept"/>, <see cref="objective"/> and <see cref="sensor"/>.
        /// <para/>
        /// Usually, you might only want either <see cref="PerceptSteering"/> or <see cref="ReceptorSteering"/> to be
        /// called at once.
        /// <para/>
        /// To make a <see cref="SteeringBehaviour"/> thread-safe, only <see cref="self"/> and <see cref="percept"/>
        /// must be accessed by writes. All other references for quick access are not thread-safe. Furthermore, to be
        /// thread-safe, no reference types of the Unity scripting API must be used within this method. If you fulfil
        /// these restrictions, you can mark the derived <see cref="SteeringBehaviour"/> class as thread-safe through
        /// setting <see cref="AIMBehaviour.ThreadSafe"/> to <c>true</c> for its corresponding front-end class.
        /// </summary>
        protected virtual void PerceptSteering()
        { }

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// This method gets called for each active shape receptor for each processed percept. Its purpose is to set an
        /// appropriate <see cref="ResultDirection"/> and <see cref="ResultMagnitude"/> for obtaining/writing objective
        /// values. So the implementation of this method determines what kind of AI movement behaviour a derived <see
        /// cref="SteeringBehaviour"/> class really is.
        /// <para/>
        /// Within this method, the following references can be used for quick data access: <see cref="self"/>, <see
        /// cref="percept"/>, <see cref="objective"/>, <see cref="sensor"/>, <see cref="receptor"/> and <see
        /// cref="structure"/>.
        /// <para/>
        /// Usually, you might only want either <see cref="PerceptSteering"/> or <see cref="ReceptorSteering"/> to be
        /// called at once.
        /// <para/>
        /// To make a <see cref="SteeringBehaviour"/> thread-safe, only <see cref="self"/> and <see cref="percept"/>
        /// must be accessed by writes. All other references for quick access are not thread-safe. Furthermore, to be
        /// thread-safe, no reference types of the Unity scripting API must be used within this method. If you fulfil
        /// these restrictions, you can mark the derived <see cref="SteeringBehaviour"/> class as thread-safe through
        /// setting <see cref="AIMBehaviour.ThreadSafe"/> to <c>true</c> for its corresponding front-end class.
        /// </summary>
        protected virtual void ReceptorSteering()
        { }

        #endregion // Methods
    } // class SteeringBehaviour
} // namespace Polarith.AI.Move
