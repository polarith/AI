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
    /// A very simple and basic character controller component suitable for the use in 2D projects. Its purpose is to
    /// show the direct output of the movement AI algorithms for debugging, whereby it rotates the up vector of the
    /// agent towards the direction decided by the AI and translates along. Furthermore, it is assumed that the
    /// character moves in parallel to the x/y-plane. Requires an <see cref="AIMContext"/> component. If <see
    /// cref="Context"/> is <c>null</c>, this controller attempts to get these components in the OnEnable method. If
    /// they are still <c>null</c>, the controller is disabled.
    /// <para/>
    /// For debugging purposes, this component is acceptable, but for production, you should definitely implement your
    /// own character controller which matches your application or game best.
    /// <para/>
    /// Only one single component can be attached to one <see cref="GameObject"/> at a time.
    /// </summary>
    [AddComponentMenu("Polarith AI » Move/Character/AIM Simple Controller 2D")]
    [HelpURL("http://docs.polarith.com/ai/component-aim-simplecontroller.html")]
    [DisallowMultipleComponent]
    public sealed class AIMSimpleController2D : MonoBehaviour
    {
        #region Fields =================================================================================================

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
            transform.rotation = Quaternion.LookRotation(Vector3.forward, Context.DecidedDirection);

            // Translate along oriented direction
            if (ObjectiveAsSpeed >= 0 && ObjectiveAsSpeed < Context.DecidedValues.Count)
            {
                velocity = Context.DecidedValues[ObjectiveAsSpeed] * Speed;
                velocity = velocity > Speed ? Speed : velocity;
            }
            else
            {
                velocity = Speed;
            }

            transform.Translate(Time.deltaTime * velocity * Vector3.up);
        }

        #endregion // Methods
    } // class AIMSimpleController2D
} // namespace Polarith.AI.Move
