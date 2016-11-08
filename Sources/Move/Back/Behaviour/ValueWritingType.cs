namespace Polarith.AI.Move
{
    /// <summary>
    /// Sets the operation for writing objective values in <see cref="MoveBehaviour.WriteValue"/>.
    /// </summary>
    public enum ValueWritingType
    {
        /// <summary>
        /// Writes a new value iff it is greater than the value which already exists.
        /// </summary>
        AssignGreater,

        /// <summary>
        /// Writes a new value iff it is lesser than the value which already exists.
        /// </summary>
        AssignLesser,

        /// <summary>
        /// Adds the new value to the value which already exists.
        /// </summary>
        Addition,

        /// <summary>
        /// Subtracts the new value from the value which already exists.
        /// </summary>
        Subtraction,

        /// <summary>
        /// Multiplies the new value to the value which already exists.
        /// </summary>
        Multiplication,

        /// <summary>
        /// Divides the value which already exists by the new value.
        /// </summary>
        Division
    }
} // namespace Polarith.AI.Move
