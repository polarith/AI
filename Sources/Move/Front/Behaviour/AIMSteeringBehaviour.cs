using UnityEngine;

namespace Polarith.AI.Move
{
    /// <summary>
    /// <see cref="AIMSteeringBehaviour"/> provides the base functionality for writing and mapping objective values
    /// (front-end component). Derived <see cref="AIMPerceptBehaviour{T}"/> front-end base component of every derived
    /// <see cref="Move.SteeringBehaviour"/>, whereby <see cref="SteeringPercept"/> is used as percept type.
    /// </summary>
    public abstract class AIMSteeringBehaviour : AIMPerceptBehaviour<SteeringPercept>
    {
        #region Properties =============================================================================================

        /// <summary>
        /// Polymorphic reference to the underlying back-end class (read only).
        /// <para/>
        /// Needs to be implemented by derived components.
        /// </summary>
        public abstract SteeringBehaviour SteeringBehaviour { get; }

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Polymorphic reference to the underlying back-end class (read only).
        /// <para/>
        /// The returned reference is originally of type <see cref="Move.SteeringBehaviour"/>.
        /// </summary>
        public override PerceptBehaviour<SteeringPercept> PerceptBehaviour
        {
            get { return SteeringBehaviour; }
        }

        #endregion // Properties

        #region Methods ================================================================================================

        /// <summary>
        /// Checks if the <see cref="SteeringBehaviour.TargetObjective"/> is valid to use, and prints appropriate debug
        /// warnings if it is not.
        /// <para/>
        /// Needs to be called from within the main thread.
        /// </summary>
        public override void PrepareEvaluation()
        {
            base.PrepareEvaluation();

            if (SteeringBehaviour.TargetObjective < 0 ||
                SteeringBehaviour.TargetObjective >= context.Problem.ObjectiveCount)
            {
                Debug.LogWarning('(' + typeof(AIMSteeringBehaviour).Name + ") " + gameObject.name + ": " +
                    "the set target objective with value '" + SteeringBehaviour.TargetObjective + "' is not valid");
            }
        }

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// This function is called when the script is loaded or a value is changed in the inspector (editor only).
        /// <para/>
        /// Use this function to validate the data of your MonoBehaviours. This can be used to ensure that when you
        /// modify data in an editor that the data stays within a certain range.
        /// </summary>
        protected override void OnValidate()
        {
            base.OnValidate();
            CheckFirstAndCentralOrder(typeof(AIMSteeringBehaviour));
        }

        #endregion // Methods
    } // class AIMSteeringBehaviour
} // namespace Polarith.AI.Move
