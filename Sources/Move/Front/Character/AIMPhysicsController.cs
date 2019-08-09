//
// Copyright (c) 2016-2019 Polarith. All rights reserved.
// Licensed under the Polarith Software and Source Code License Agreement.
// See the LICENSE file in the project root for full license information.
//

using UnityEngine;

namespace Polarith.AI.Move
{
    /// <summary>
    /// A very simple and basic physics-based controller component suitable for the use in 3D projects where the agent
    /// has a clear orientation to a ground plane. The purpose of this controller is to show the AI results under the
    /// assumption that there is physics involved for the movement so that the resulting AI outputs are smoothed over
    /// time through using forces. It rotates the forward vector of the agent towards the direction decided by the AI
    /// and translates along. Requires an <see cref="AIMContext"/> and <see cref="Rigidbody"/> component. If <see
    /// cref="Context"/> or <see cref="Body"/> are <c>null</c>, this controller attempts to get these components in the
    /// OnEnable method. If they are still <c>null</c>, the controller will stay disabled.
    /// <para/>
    /// For debugging purposes, this component is acceptable, but for production, you should definitely implement your
    /// own character controller which matches your application or game best.
    /// <para/>
    /// Only one single component can be attached to one <see cref="GameObject"/> at a time.
    /// </summary>
    [AddComponentMenu("Polarith AI » Move/Character/AIM Physics Controller")]
    [HelpURL("http://docs.polarith.com/ai/component-aim-physicscontroller.html")]
    [DisallowMultipleComponent]
    public sealed class AIMPhysicsController : MonoBehaviour
    {
        #region Fields =================================================================================================

        /// <summary>
        /// Determines the base value of the applied force for rotating the character towards the decided direction.
        /// This value is highly dependent on the <see cref="Rigidbody.angularDrag"/>, <see cref="Rigidbody.mass"/> and
        /// the <see cref="PhysicMaterial"/> used by the involved <see cref="Collider"/> instances.
        /// <para/>
        /// For the default value, you may use a <see cref="Rigidbody"/> configuration with mass = 1, angular drag = 5
        /// and a default collider material.
        /// </summary>
        [Tooltip("Determines the base value of the applied force for rotating the character towards the decided " +
            "direction. This value is highly dependent on the 'Rigidbody.angularDrag', 'Rigidbody.mass' and the " +
            "'PhysicMaterial' used by the involved collider instances.\n\nFor the default value, you may use a " +
            "rigidbody configuration with mass = 1, angular drag = 5 and a default collider material.")]
        public float Torque = 1f;

        /// <summary>
        /// Determines the base value specifying how fast the character moves. This value is highly dependent on the
        /// <see cref="Rigidbody.drag"/>, <see cref="Rigidbody.mass"/> and the <see cref="PhysicMaterial"/> used by the
        /// involved <see cref="Collider"/> instances.
        /// <para/>
        /// For the default value, you may use a <see cref="Rigidbody"/> configuration with mass = 1, drag = 1 and a
        /// default collider material.
        /// </summary>
        [Tooltip("Determines the base value specifying how fast the character moves. This value is highly dependent " +
            "on the 'Rigidbody.drag', 'Rigidbody.mass' and the 'PhysicMaterial' used by the involved collider " +
            "instances.\n\nFor the default value, you may use a rigidbody configuration with mass = 1, drag = 1 and " +
            "a default collider material.")]
        public float Speed = 15.0f;

        /// <summary>
        /// If set equal to or greater than 0, the evaluated AI decision value is multiplied to the <see cref="Speed"/>.
        /// </summary>
        [Tooltip("If set equal to or greater than 0, the evaluated AI decision value is multiplied to the 'Speed'.")]
        [TargetObjective(true)]
        public int ObjectiveAsSpeed = -1;

        /// <summary>
        /// The <see cref="ForceMode"/> which is applied to the <see cref="Rigidbody.AddForce(Vector3, ForceMode)"/>
        /// method of the associated <see cref="Body"/>.
        /// </summary>
        [Tooltip("The 'ForceMode' which is applied to the 'AddForce' method of the associated rigidbody.")]
        public ForceMode Mode = ForceMode.Acceleration;

        /// <summary>
        /// The <see cref="AIMContext"/> which provides the next movement direction that is applied to the <see
        /// cref="Body"/>.
        /// </summary>
        [Tooltip("The 'AIMContext' which provides the next movement direction that is applied to the rigidbody.")]
        public AIMContext Context;

        /// <summary>
        /// The <see cref="Rigidbody"/> which gets manipulated by this controller.
        /// </summary>
        [Tooltip("The rigidbody which gets manipulated by this controller.")]
        public Rigidbody Body;

        //--------------------------------------------------------------------------------------------------------------

        private Vector3 forward;
        private Vector3 up;
        private Vector3 cross;
        private float angleDiff;
        private float velocity;

        #endregion // Fields

        #region Methods ================================================================================================

        private void OnEnable()
        {
            if (Body == null)
                Body = gameObject.GetComponentInChildren<Rigidbody>();
            if (Context == null)
                Context = gameObject.GetComponentInChildren<AIMContext>();

            // Disable if the setup is invalid.
            if (Body == null || Context == null)
                enabled = false;
        }

        //--------------------------------------------------------------------------------------------------------------

        private void FixedUpdate()
        {
            // Excepts at least 1 objective
            if (Context.ObjectiveCount == 0 || Context.DecidedValues.Count <= 0)
                return;

            // Find out the relation of the current movement direction to the decided direction
            forward = transform.forward;
            up = transform.up;

            angleDiff = 0.0f;
            if (Context.DecidedDirection != Vector3.zero && Context.DecidedValues[0] > 0.0f)
                angleDiff = Vector3.Angle(forward, Context.DecidedDirection);

            // Find out which sign has to be used.
            cross = Vector3.Cross(up, forward);

            if (Vector3.Dot(cross, Context.DecidedDirection) < 0.0f)
                angleDiff = -angleDiff;

            // Orient towards decision direction using torque
            Body.AddTorque(up * Torque * angleDiff);

            // Translate along oriented direction using the force (which may be with you, of course)
            if (ObjectiveAsSpeed >= 0 && ObjectiveAsSpeed < Context.DecidedValues.Count)
            {
                velocity = Context.DecidedValues[ObjectiveAsSpeed] * Speed;
                velocity = velocity > Speed ? Speed : velocity;
            }
            else
            {
                velocity = Speed;
            }

            Body.AddForce(velocity * forward, Mode);
        }

        #endregion // Methods
    } // class AIMPhysicsController
} // namespace Polarith.AI.Move
