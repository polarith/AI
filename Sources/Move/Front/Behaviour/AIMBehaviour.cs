using Polarith.AI.Criteria;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Polarith.AI.Move
{
    /// <summary>
    /// <see cref="AIMBehaviour"/> is the abstract base class for all the behaviours in the Move module of Polarith AI
    /// (front-end component). Every <see cref="AIMBehaviour"/> component have got a certain <see cref="Type"/>
    /// specifying the processing order of the back-end <see cref="MoveBehaviour"/> within <see
    /// cref="Context.Evaluate"/>.
    /// <para/>
    /// If the underlying <see cref="MoveBehaviour"/> is thread-safe, this should be indicated through returning
    /// <c>true</c> for the <see cref="ThreadSafe"/> property in derived <see cref="AIMBehaviour"/> components. An
    /// associated <see cref="AIMContext"/> is thread-safe if all of its behaviours are thread-safe, too.
    /// <para/>
    /// Abstract base front-end component of every derived <see cref="MoveBehaviour"/>.
    /// </summary>
    [RequireComponent(typeof(AIMContext))]
    public abstract class AIMBehaviour : MonoBehaviour, IEvaluationPreparer
    {
        #region Fields =================================================================================================

        /// <summary>
        /// Specifies the execution order of this behaviour. If changed at runtime, the internal hold behaviour
        /// collections need to be re-sorted.
        /// </summary>
        [Tooltip("Specifies the execution order of this behaviour. If changed at runtime, the internal hold " +
            "behaviour collections need to be re-sorted.")]
        public int Order;

        /// <summary>
        /// Name to identify this component, e.g. within a state machine.
        /// </summary>
        [Tooltip("Name to identify this component, e.g. within a state machine.")]
        public string Label;

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Quick access reference for derived classes to the component of the associated movement AI context.
        /// </summary>
        protected AIMContext aimContext;

        /// <summary>
        /// Quick access reference for derived classes to the associated movement AI context.
        /// </summary>
        protected Context context;

        //--------------------------------------------------------------------------------------------------------------

        [SerializeField]
        [HideInInspector]
        private bool initialized;

        #endregion // Fields

        #region Properties =============================================================================================

        /// <summary>
        /// Polymorphic reference to the underlying back-end class (read only).
        /// <para/>
        /// Needs to be implemented by derived components.
        /// </summary>
        public abstract MoveBehaviour Behaviour { get; }

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Determines whether the underlying back-end <see cref="MoveBehaviour"/> is thread-safe (read only).
        /// <para/>
        /// An associated <see cref="AIMContext"/> is thread-safe if all of its behaviours are thread-safe, too.
        /// </summary>
        public virtual bool ThreadSafe
        {
            get { return false; }
        }

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Determines whether this component is enabled.
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        #endregion // Properties

        #region Methods ================================================================================================

        /// <summary>
        /// If necessary, re-registers this behaviour to its associated <see cref="AIMContext"/> according to the
        /// currently set <see cref="Order"/>.
        /// <para/>
        /// Needs to be called within the main thread.
        /// </summary>
        public virtual void PrepareEvaluation()
        {
            // Compare with last behaviour type and re-register if necessary
            if (Order != Behaviour.Order)
            {
                Behaviour.Order = Order;
                UnregisterBehaviour();
                RegisterBehaviour();
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
        protected virtual void Reset()
        {
            Behaviour.Order = Order;
            initialized = true;
        }

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            aimContext = GetComponent<AIMContext>();
            aimContext.EvaluationPreparers.Add(this);
            context = aimContext.Context;
            Behaviour.Context = context;
            Behaviour.Order = Order;
            RegisterBehaviour();
            if (!initialized)
                Reset();

            gameObject.GetComponents<AIMSeek>();
        }

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// This method is called when the object becomes enabled and active.
        /// </summary>
        protected virtual void OnEnable()
        {
            Behaviour.Enabled = true;
        }

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// This method is called when the behaviour becomes disabled or inactive.
        /// </summary>
        protected virtual void OnDisable()
        {
            Behaviour.Enabled = false;
        }

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// This method is called when the MonoBehaviour will be destroyed.
        /// </summary>
        protected virtual void OnDestroy()
        {
            aimContext.EvaluationPreparers.Remove(this);
            UnregisterBehaviour();
        }

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn (editor only).
        /// </summary>
        protected virtual void OnDrawGizmos()
        { }

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// This function is called when the script is loaded or a value is changed in the inspector (editor only).
        /// <para/>
        /// Use this function to validate the data of your MonoBehaviours. This can be used to ensure that when you
        /// modify data in an editor that the data stays within a certain range.
        /// </summary>
        protected virtual void OnValidate()
        {
            if (!Application.isPlaying)
                aimContext = GetComponent<AIMContext>();
        }

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Generates a list containing all objectives currently available within the associated <see cref="Context"/>.
        /// <para/>
        /// This may be used as default parametrization for behaviours allowing the use of multiple target objectives,
        /// like for instance <see cref="PlanarConvolution"/> and <see cref="Retention"/>.
        /// </summary>
        /// <returns>
        /// A list containing all objectives currently available within the associated <see cref="Context"/>.
        /// </returns>
        protected List<int> GetDefaultTargetObjectives()
        {
            List<int> targetObjectives = new List<int>();
            AIMContext aimContext = GetComponent<AIMContext>();
            if (aimContext != null)
            {
                // The following is ugly but necessary due to Unity's arbitrary order of calling Awake
                if (aimContext.Context.Problem.ObjectiveCount == 0)
                    aimContext.BuildContext();

                for (int i = 0; i < aimContext.Context.Problem.ObjectiveCount; i++)
                    targetObjectives.Add(i);
            }
            return targetObjectives;
        }

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Should be used in <see cref="OnValidate"/> for derived classes to ensure that they are executed at first.
        /// </summary>
        protected void CheckFirstAndCentralOrder(Type type)
        {
            if (Order >= CriteriaBehaviour.LastOrder)
            {
                Order = CriteriaBehaviour.LastOrder - 1;
                Debug.Log('(' + type.Name + ") " + gameObject.name + ": " +
                    "'Order' needs to be lesser than " + CriteriaBehaviour.LastOrder);
            }
        }

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Should be used in <see cref="OnValidate"/> for derived classes to ensure that they are executed at last.
        /// </summary>
        protected void CheckLastOrder(Type type)
        {
            if (Order < CriteriaBehaviour.LastOrder)
            {
                Order = CriteriaBehaviour.LastOrder;
                Debug.Log('(' + type.Name + ") " + gameObject.name + ": " +
                    "'Order' needs to be greater than or equal to " + CriteriaBehaviour.LastOrder);
            }
        }

        //--------------------------------------------------------------------------------------------------------------

        private void RegisterBehaviour()
        {
            aimContext.Behaviours.Add(this);
            aimContext.BehaviourSortRequired = true;
            aimContext.ThreadSafetyCheckRequired = true;
            context.Behaviours.Add(Behaviour);
        }

        //--------------------------------------------------------------------------------------------------------------

        private void UnregisterBehaviour()
        {
            aimContext.Behaviours.Remove(this);
            aimContext.ThreadSafetyCheckRequired = true;
            context.Behaviours.Remove(Behaviour);
        }

        #endregion // Methods
    } // class AIMBehaviour
} // namespace Polarith.AI.Move
