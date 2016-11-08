namespace Polarith.AI.Move
{
    /// <summary>
    /// Used within the <see cref="MoveBehaviour.MapSpecialSqr(MappingType, float, float, float)"/> method in order to
    /// specify the desired type of the applied mapping function.
    /// </summary>
    public enum MappingType
    {
        /// <summary>
        /// Results in 1 constantly.
        /// </summary>
        Constant,

        /// <summary>
        /// Maps linearly from the min/max interval to 0 and 1.
        /// </summary>
        Linear,

        /// <summary>
        /// Maps inverse linearly from the max/min interval to 1 and 0.
        /// </summary>
        InverseLinear,

        /// <summary>
        /// Applies a quadratic mapping from the min/max interval to 0 and 1.
        /// </summary>
        Quadratic,

        /// <summary>
        /// Applies an inverse quadratic mapping from the max/min interval to 1 and 0.
        /// </summary>
        InverseQuadratic,

        /// <summary>
        /// Applies a square root mapping from the min/max interval to 0 and 1.
        /// </summary>
        SquareRoot,

        /// <summary>
        /// Applies an inverse square root mapping from the max/min interval to 1 and 0.
        /// </summary>
        InverseSquareRoot
    } // enum MappingType
} // namespace Polarith.AI.Move
