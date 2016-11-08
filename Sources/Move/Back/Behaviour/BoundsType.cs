namespace Polarith.AI.Move
{
    /// <summary>
    /// Intended to distinguish between the different types of bounding boxes, e.g., within the <see
    /// cref="PlanarAvoidBounds"/>, <see cref="PlanarSeekBounds"/> and <see cref="PlanarFleeBounds"/> behaviours.
    /// </summary>
    public enum BoundsType
    {
        /// <summary>
        /// Selects axis-aligned bounding boxes (AABB) in world coordinates.
        /// </summary>
        ColliderAABB,

        /// <summary>
        /// Selects oriented bounding boxes (OBB) in local coordinates.
        /// </summary>
        ColliderOBB,

        /// <summary>
        /// Selects visual (sprite/mesh) bounds in local coordinates.
        /// </summary>
        Visual
    } // enum BoundsType
} // namespace Polarith.AI.Move
