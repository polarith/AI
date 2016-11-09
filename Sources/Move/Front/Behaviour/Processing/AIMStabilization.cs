using Polarith.AI.Criteria;
using UnityEngine;

namespace Polarith.AI.Move
{
    /// <summary>
    /// <see cref="AIMStabilization"/> increases the objective values along the movement direction of the agent
    /// (front-end component).
    /// <para/>
    /// Front-end component of the underlying <see cref="Move.Stabilization"/> class. This behaviour is thread-safe.
    /// </summary>
    [AddComponentMenu("Polarith AI » Move/Behaviours/Processing/AIM Stabilization")]
    public sealed class AIMStabilization : AIMBehaviour
    {
        #region Fields =================================================================================================

        /// <summary>
        /// The underlying back-end behaviour class.
        /// </summary>
        [Tooltip("Allows to specify the settings of this behaviour.")]
        public Stabilization Stabilization = new Stabilization();

        //--------------------------------------------------------------------------------------------------------------

        private AIMFilter<SteeringPercept> filter;

        #endregion // Fields

        #region Properties =============================================================================================

        /// <summary>
        /// Polymorphic reference to the underlying back-end class (read only).
        /// </summary>
        public override MoveBehaviour Behaviour
        {
            get { return Stabilization; }
        }

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Determines whether the underlying back-end class is thread-safe (read only).
        /// <para/>
        /// Returns always <c>true</c>.
        /// </summary>
        public override bool ThreadSafe
        {
            get { return true; }
        }

        #endregion // Properties

        #region Methods ================================================================================================

        /// <summary>
        /// Checks if the <see cref="SteeringBehaviour.TargetObjective"/> is valid to use and prints appropriate debug
        /// warnings if it is not.
        /// <para/>
        /// Needs to be called from within the main thread.
        /// </summary>
        public override void PrepareEvaluation()
        {
            base.PrepareEvaluation();

            // Check parameters
            if (Stabilization.TargetObjective < 0 ||
                Stabilization.TargetObjective >= context.Problem.ObjectiveCount)
            {
                Debug.LogWarning('(' + typeof(AIMStabilization).Name + ") " + gameObject.name + ": " +
                    "the set target objective with value '" + Stabilization.TargetObjective + "' is not valid");
                return;
            }

            // Obtain data via a filter if it is necessary and there is one
            if (filter != null)
                Stabilization.Self = filter.Self;
            else
                Stabilization.Self.Receive(gameObject);
        }

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Reset to default values.
        /// <para/>
        /// Reset is called when the user hits the Reset button in the Inspector's context menu or when adding the
        /// component the first time. This function is only called in editor mode. Reset is most commonly used to give
        /// good default values in the inspector.
        /// </summary>
        protected override void Reset()
        {
            Order = CriteriaBehaviour.CentralOrder;
            base.Reset();
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
            CheckFirstAndCentralOrder(typeof(AIMStabilization));
        }

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// This method is called when the object becomes enabled and active.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            filter = GetComponent<AIMFilter<SteeringPercept>>();
        }

        #endregion // Methods
    } // class AIMStabilization
} // namespace Polarith.AI.Move
