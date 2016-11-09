#if UNITY_EDITOR

using Polarith.UnityUtils;

#endif // UNITY_EDITOR

using UnityEngine;

namespace Polarith.AI.Move
{
    /// <summary>
    /// This class extends the <see cref="AIMSteeringBehaviour"/> through adding a kind of local perception model based
    /// on an <see cref="RadiusSteeringBehaviour.InnerRadius"/> and <see cref="RadiusSteeringBehaviour.OuterRadius"/>
    /// (front-end component). This component is responsible for drawing appropriate gizmos in order to visualize the
    /// <see cref="RadiusSteeringBehaviour.InnerRadius"/> and <see cref="RadiusSteeringBehaviour.OuterRadius"/> of the
    /// underlying back-end class within the scene view.
    /// <para/>
    /// Front-end component of the underlying <see cref="Move.RadiusSteeringBehaviour"/> class.
    /// </summary>
    public abstract class AIMRadiusSteeringBehaviour : AIMSteeringBehaviour
    {
        #region Fields =================================================================================================

#if UNITY_EDITOR

        /// <summary>
        /// Sets up the visualization of the inner radius (editor only).
        /// </summary>
        [Tooltip("Sets up the visualization of the inner radius.")]
        [SerializeField]
        protected CircleGizmo innerRadiusGizmo = new CircleGizmo();

        /// <summary>
        /// Sets up the visualization of the outer radius (editor only).
        /// </summary>
        [Tooltip("Sets up the visualization of the outer radius.")]
        [SerializeField]
        protected CircleGizmo outerRadiusGizmo = new CircleGizmo();

#endif // UNITY_EDITOR

        #endregion // Fields

        #region Constructors ===========================================================================================

        /// <summary>
        /// Constructs an <see cref="AIMRadiusSteeringBehaviour"/> instance.
        /// </summary>
        public AIMRadiusSteeringBehaviour()
        {
#if UNITY_EDITOR
            innerRadiusGizmo.Color = new Color(152f / 255f, 255f / 255f, 145f / 255f);
#endif // UNITY_EDITOR
        }

        #endregion // Constructors

        #region Properties =============================================================================================

        /// <summary>
        /// Polymorphic reference to the underlying back-end class (read only).
        /// <para/>
        /// Needs to be implemented by derived components.
        /// </summary>
        public abstract RadiusSteeringBehaviour RadiusSteeringBehaviour { get; }

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Polymorphic reference to the underlying back-end class (read only).
        /// <para/>
        /// The returned reference is originally of type <see cref="Move.RadiusSteeringBehaviour"/>.
        /// </summary>
        public override SteeringBehaviour SteeringBehaviour
        {
            get { return RadiusSteeringBehaviour; }
        }

        #endregion // Properties

        #region Methods ================================================================================================

#if UNITY_EDITOR

        /// <summary>
        /// Draws gizmos for the inner radius and outer radius of the underlying back-end <see
        /// cref="Move.RadiusSteeringBehaviour"/> within the scene view (editor only).
        /// </summary>
        /// <exception cref="System.NullReferenceException">
        /// If <see cref="RadiusSteeringBehaviour"/> is <c>null</c>.
        /// </exception>
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (aimContext != null && aimContext.Sensor != null)
            {
                if (innerRadiusGizmo.Enabled)
                {
                    innerRadiusGizmo.Draw(gameObject.transform.position,
                        transform.rotation * aimContext.Sensor.Sensor.Rotation,
                        RadiusSteeringBehaviour.InnerRadius);
                }
                if (outerRadiusGizmo.Enabled)
                {
                    outerRadiusGizmo.Draw(gameObject.transform.position,
                        transform.rotation * aimContext.Sensor.Sensor.Rotation,
                        RadiusSteeringBehaviour.OuterRadius);
                }
            }
        }

#endif // UNITY_EDITOR

        #endregion // Methods
    } // class AIMRadiusSteeringBehaviour
} // namespace Polarith.AI.Move
