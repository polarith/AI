using Polarith.AI.Criteria;
using UnityEngine;

namespace Polarith.AI.Move
{
    /// <summary>
    /// <see cref="AIMRetention"/> is used to remember objective values for multiple frames (front-end component).
    /// Front-end component of the underlying <see cref="Move.Retention"/> class. This behaviour is thread-safe.
    /// </summary>
    [AddComponentMenu("Polarith AI » Move/Behaviours/Processing/AIM Retention")]
    public sealed class AIMRetention : AIMBehaviour
    {
        #region Fields =================================================================================================

        /// <summary>
        /// The underlying back-end behaviour class.
        /// </summary>
        [Tooltip("Allows to specify the settings of this behaviour.")]
        public Retention Retention = new Retention();

        //--------------------------------------------------------------------------------------------------------------

        private int i;

        #endregion // Fields

        #region Properties =============================================================================================

        /// <summary>
        /// Polymorphic reference to the underlying back-end class (read only).
        /// </summary>
        public override MoveBehaviour Behaviour
        {
            get { return Retention; }
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
        /// Needs to be called within the main thread to prepare <see cref="Context.Evaluate"/>.
        /// </summary>
        public override void PrepareEvaluation()
        {
            base.PrepareEvaluation();

            for (i = 0; i < Retention.TargetObjectives.Count; i++)
            {
                if (Retention.TargetObjectives[i] < 0 ||
                    Retention.TargetObjectives[i] >= context.Problem.ObjectiveCount)
                {
                    Debug.LogWarning('(' + typeof(AIMRetention).Name + ") " + gameObject.name + ": " +
                        "the set target objective no. '" + i + "' with value '" + Retention.TargetObjectives[i] +
                        "' is not valid");
                }
            }
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
            Retention.TargetObjectives = GetDefaultTargetObjectives();

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
            CheckFirstAndCentralOrder(typeof(AIMRetention));
        }

        #endregion // Methods
    } // class AIMRetention
} // namespace Polarith.AI.Move
