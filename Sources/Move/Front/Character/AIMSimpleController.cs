//
// Copyright (c) 2016-2019 Polarith. All rights reserved.
// Licensed under the Polarith Software and Source Code License Agreement.
// See the LICENSE file in the project root for full license information.
//

using Polarith.Utils;
using UnityEngine;

namespace Polarith.AI.Move
{
    /// <summary>
    /// A very simple and basic character controller component suitable for the use in 3D projects. Its purpose is to
    /// show the direct output of the movement AI algorithms for debugging. Requires an <see cref="AIMContext"/>
    /// component. If <see cref="Context"/> is <c>null</c>, this controller attempts to get these components in the
    /// OnEnable method. If they are still <c>null</c>, the controller is disabled.
    /// <para/>
    /// For debugging purposes, this component is acceptable, but for production, you should definitely implement your
    /// own character controller which matches your application or game best.
    /// <para/>
    /// Only one single component can be attached to one <see cref="GameObject"/> at a time.
    /// </summary>
    [AddComponentMenu("Polarith AI » Move/Character/AIM Simple Controller")]
    [HelpURL("http://docs.polarith.com/ai/component-aim-simplecontroller.html")]
    [DisallowMultipleComponent]
    public sealed class AIMSimpleController : MonoBehaviour
    {
        #region Fields =================================================================================================

        /// <summary>
        /// The direction which is used to rotate the forward direction according to the decision made by the <see
        /// cref="Context"/>.
        /// <para/>
        /// This vector needs to be perpendicular to an agent's forward direction, e.g., if the agent moves in the
        /// x/z-plane, this vector needs always to be (0, 1, 0).
        /// </summary>
        [Tooltip("The direction which is used to rotate the forward direction according to the decision made by the " +
            "'Context'.\n\n" +
            "This vector needs to be perpendicular to an agent's forward direction, e.g., if the agent moves in the " +
            "x/z-plane, this vector needs always to be (0, 1, 0).")]
        public Vector3 Up = Vector3.up;

        /// <summary>
        /// Determines the base value specifying how fast the character moves.
        /// </summary>
        [Tooltip("Determines the base value specifying how fast the character moves.")]
        public float Speed = 1f;

        /// <summary>
        /// If set equal to or greater than 0, the evaluated AI decision value is multiplied to the <see cref="Speed"/>.
        /// </summary>
        [Tooltip("If set equal to or greater than 0, the evaluated AI decision value is multiplied to the 'Speed'.")]
        [TargetObjective(true)]
        public int ObjectiveAsSpeed = -1;

        /// <summary>
        /// The <see cref="AIMContext"/> which provides the next movement direction that is applied to the agent's <see
        /// cref="Transform"/>.
        /// </summary>
        [Tooltip("The AIMContext which provides the next movement direction that is applied to the agent's transform.")]
        public AIMContext Context;

        //--------------------------------------------------------------------------------------------------------------

        private float velocity;

        #endregion // Fields

        #region Methods ================================================================================================

        private void OnEnable()
        {
            if (Context == null)
                Context = GetComponentInChildren<AIMContext>();

            if (Context == null)
                enabled = false;
        }

        //--------------------------------------------------------------------------------------------------------------

        private void Update()
        {
            if (Mathf2.Approximately(Context.DecidedDirection.sqrMagnitude, 0))
                return;

            // Orient towards decision direction
            transform.rotation = Quaternion.LookRotation(Context.DecidedDirection, Up);

            // Translate along oriented direction
            if (ObjectiveAsSpeed >= 0)
            {
                velocity = Context.DecidedValues[ObjectiveAsSpeed] * Speed;
                velocity = velocity > Speed ? Speed : velocity;
            }
            else
            {
                velocity = Speed;
            }

            transform.position += Time.deltaTime * velocity * Context.DecidedDirection;
        }

        #endregion // Methods
    } // class AIMSimpleController
} // namespace Polarith.AI.Move
