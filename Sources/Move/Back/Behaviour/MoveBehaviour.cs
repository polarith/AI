using Polarith.AI.Criteria;
using Polarith.Utils;
using System;
using UnityEngine;

namespace Polarith.AI.Move
{
    /// <summary>
    /// <see cref="MoveBehaviour"/> is the abstract base class for all the behaviours in the Move module of Polarith AI
    /// (back-end class). It is derived from <see cref="CriteriaBehaviour"/>. In addition, it holds a reference for
    /// quick access to the associated <see cref="Context"/> so that every <see cref="MoveBehaviour"/> is able to obtain
    /// all the necessary information for writing/modifying objective values in the corresponding <see
    /// cref="Context.Problem"/>.
    /// <para/>
    /// This class also defines and implements additional mechanics for manipulating objective values. The methods <see
    /// cref="MapBySensitivity(MappingType, Structure, Vector3, float)"/> and <see cref="WriteValue"/> are designed to
    /// support this process.
    /// <para/>
    /// A <see cref="MoveBehaviour"/> can be marked as thread-safe in its corresponding front-end class through setting
    /// <see cref="AIMBehaviour.ThreadSafe"/> to <c>true</c>. If you do this, be sure not to use reference types of the
    /// Unity scripting API and not to make any possibly unsafe variable writes. If at least one behaviour is not marked
    /// <see cref="AIMBehaviour.ThreadSafe"/>, the whole agent will not be part of the multithreading in <see
    /// cref="AIMThreading"/>.
    /// <para/>
    /// Base back-end class of every derived <see cref="AIMBehaviour"/>.
    /// </summary>
    public abstract class MoveBehaviour : CriteriaBehaviour
    {
        #region Fields =================================================================================================

        /// <summary>
        /// Reference for quick access to the associated context instance.
        /// </summary>
        [NonSerialized]
        public Context Context;

        #endregion // Fields

        #region Methods ================================================================================================

        /// <summary>
        /// Maps a <paramref name="value"/> lying between <paramref name="min"/> and <paramref name="max"/> to a
        /// resulting value between 0 and 1.
        /// <para/>
        /// The function used for the mapping is specified via the <paramref name="mapping"/> parameter, see <see
        /// cref="MappingType"/>.
        /// </summary>
        /// <param name="mapping">Specifies the applied type of the mapping function.</param>
        /// <param name="min">The minimum of the function argument interval.</param>
        /// <param name="max">The maximum of the function argument interval.</param>
        /// <param name="value">The argument value to be mapped.</param>
        /// <returns>The mapped function value.</returns>
        public static float MapSpecial(MappingType mapping, float min, float max, float value)
        {
            if (mapping == MappingType.Constant)
                return 1f;

            // Clamp if value is outside of the min/max interval
            if (value < min + Mathf2.Epsilon)
                return (int)mapping % 2 != 0 ? 0f : 1f;
            else if (value > max - Mathf2.Epsilon)
                return (int)mapping % 2 != 0 ? 1f : 0f;

            // Map according to the specified mapping type
            float result;
            switch (mapping)
            {
                case MappingType.Linear:
                    result = Mathf2.MapLinear(0f, 1f, min, max, value);
                    break;

                case MappingType.InverseLinear:
                    result = 1f - Mathf2.MapLinear(0f, 1f, min, max, value);
                    break;

                case MappingType.Quadratic:
                    result = Mathf2.MapLinear(0f, 1f, min, max, value);
                    result *= result;
                    break;

                case MappingType.InverseQuadratic:
                    result = Mathf2.MapLinear(0f, 1f, min, max, value);
                    result *= result;
                    result = 1f - result;
                    break;

                case MappingType.SquareRoot:
                    result = Mathf.Sqrt(Mathf2.MapLinear(0f, 1f, min, max, value));
                    break;

                case MappingType.InverseSquareRoot:
                    result = 1f - Mathf.Sqrt(Mathf2.MapLinear(0f, 1f, min, max, value));
                    break;

                default: // MappingType.Constant
                    result = 1f;
                    break;
            }

            return result;
        }

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Maps a <paramref name="sqrValue"/> lying between <paramref name="sqrMin"/> and <paramref name="sqrMax"/> to
        /// a resulting value between 0 and 1, whereby all the given parameters are expected to be squared.
        /// <para/>
        /// The function used for the mapping is specified via the <paramref name="mapping"/> parameter, see <see
        /// cref="MappingType"/>.
        /// </summary>
        /// <param name="mapping">Specifies the applied type of the mapping function.</param>
        /// <param name="sqrMin">The squared minimum of the function argument interval.</param>
        /// <param name="sqrMax">The squared maximum of the function argument interval.</param>
        /// <param name="sqrValue">The squared argument value to be mapped.</param>
        /// <returns>The mapped function value.</returns>
        public static float MapSpecialSqr(MappingType mapping, float sqrMin, float sqrMax, float sqrValue)
        {
            if (mapping == MappingType.Constant)
                return 1f;

            // Clamp if value is outside of the min/max interval
            if (sqrValue < sqrMin + Mathf2.Epsilon)
                return (int)mapping % 2 != 0 ? 0f : 1f;
            else if (sqrValue > sqrMax - Mathf2.Epsilon)
                return (int)mapping % 2 != 0 ? 1f : 0f;

            // Map according to the specified mapping type
            float result;
            switch (mapping)
            {
                case MappingType.Linear:
                    result = Mathf.Sqrt(Mathf2.MapLinear(0f, 1f, sqrMin, sqrMax, sqrValue));
                    break;

                case MappingType.InverseLinear:
                    result = 1f - Mathf.Sqrt(Mathf2.MapLinear(0f, 1f, sqrMin, sqrMax, sqrValue));
                    break;

                case MappingType.Quadratic:
                    result = Mathf2.MapLinear(0f, 1f, sqrMin, sqrMax, sqrValue);
                    break;

                case MappingType.InverseQuadratic:
                    result = 1f - Mathf2.MapLinear(0f, 1f, sqrMin, sqrMax, sqrValue);
                    break;

                case MappingType.SquareRoot:
                    result = Mathf.Sqrt(Mathf.Sqrt(Mathf2.MapLinear(0f, 1f, sqrMin, sqrMax, sqrValue)));
                    break;

                case MappingType.InverseSquareRoot:
                    result = 1f - Mathf.Sqrt(Mathf.Sqrt(Mathf2.MapLinear(0f, 1f, sqrMin, sqrMax, sqrValue)));
                    break;

                default: // MappingType.Constant
                    result = 1f;
                    break;
            }
            return result;
        }

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Maps (magnitude) values by sensitivity so that the <see cref="Structure.Sensitivity"/> (plus the given
        /// <paramref name="sensitivityOffset"/>) is used as similarity threshold for the angle between the given
        /// <paramref name="direction"/> and the <see cref="Structure.Direction"/>.
        /// <para/>
        /// When <paramref name="mapping"/> is <see cref="MappingType.InverseLinear"/>, then: the more similar the
        /// direction vectors, the greater the returned result. If the angle between the two vectors is greater than or
        /// equal to the applied threshold, then the result is 0. If the two directions are equal (which means that the
        /// angle is 0), then the result is 1. This logic can be applied analogously for the other mapping types.
        /// </summary>
        /// <param name="mapping">Determines the type of the used mapping function.</param>
        /// <param name="structure">The <see cref="Structure"/> to obtain the sensitivity and direction for.</param>
        /// <param name="direction">The direction which gets compared to the <see cref="Structure.Direction"/>.</param>
        /// <param name="sensitivityOffset">Is added to the <see cref="Structure.Sensitivity"/> as threshold.</param>
        /// <returns>The mapped value between 0 and 1.</returns>
        /// <exception cref="NullReferenceException">If <paramref name="structure"/> is <c>null</c>.</exception>
        protected float MapBySensitivity(
            MappingType mapping,
            Structure structure,
            Vector3 direction,
            float sensitivityOffset = 0.0f)
        {
            float threshold = structure.Sensitivity + sensitivityOffset;
            float angle = Vector3.Angle(direction, Context.LocalToWorldMatrix.MultiplyVector(structure.Direction));
            if (threshold < Mathf2.Epsilon || angle > threshold)
                return 0f;
            return MapSpecial(mapping, 0f, threshold, angle);
        }

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Writes objective values to the <see cref="Context.Problem"/> as specified with <paramref
        /// name="valueWriting"/>.
        /// </summary>
        /// <param name="valueWriting">Specifies the operation for writing values.</param>
        /// <param name="objectiveIndex">Specifies the objective for writing.</param>
        /// <param name="valueIndex">Specifies the value index for writing.</param>
        /// <param name="value">The value to write.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If there is no value at the specified <paramref name="objectiveIndex"/> and/or <paramref
        /// name="valueIndex"/>.
        /// </exception>
        protected void WriteValue(ValueWritingType valueWriting, int objectiveIndex, int valueIndex, float value)
        {
            IProblem<float> problem = Context.Problem;

            switch (valueWriting)
            {
                case ValueWritingType.AssignGreater:
                    if (value > problem[objectiveIndex][valueIndex] + Mathf2.Epsilon)
                        problem.SetValue(objectiveIndex, valueIndex, value);
                    break;

                case ValueWritingType.AssignLesser:
                    if (value < problem[objectiveIndex][valueIndex] - Mathf2.Epsilon)
                        problem.SetValue(objectiveIndex, valueIndex, value);
                    break;

                case ValueWritingType.Addition:
                    problem.SetValue(objectiveIndex, valueIndex, problem[objectiveIndex][valueIndex] + value);
                    break;

                case ValueWritingType.Subtraction:
                    problem.SetValue(objectiveIndex, valueIndex, problem[objectiveIndex][valueIndex] - value);
                    break;

                case ValueWritingType.Multiplication:
                    problem.SetValue(objectiveIndex, valueIndex, problem[objectiveIndex][valueIndex] * value);
                    break;

                case ValueWritingType.Division:
                    if (value != 0f)
                        problem.SetValue(objectiveIndex, valueIndex, problem[objectiveIndex][valueIndex] / value);
                    break;

                default: // ValueWritingType.AssignGreater
                    if (value > problem[objectiveIndex][valueIndex] + Mathf2.Epsilon)
                        problem.SetValue(objectiveIndex, valueIndex, value);
                    break;
            }
        }

        #endregion // Methods
    } // class MoveBehaviour
} // namespace Polarith.AI.Move
