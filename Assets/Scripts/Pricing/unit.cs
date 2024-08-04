using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Supermarket.Pricing
{
    [Serializable]
    public struct unit
    {
        public static readonly unit zero = new unit();

        public float Value 
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => value;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { this.value = value; Rounded(); } 
        }

        [SerializeField] private float value;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Rounded()
        {
            return value = Mathf.Clamp(Mathf.Round(value * 100f) / 100f, 0, 999999);
        }

        #region OVERRIDES

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return $"${Value:F2}";
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return value.GetHashCode();
            }
        }

        #endregion

        #region OPERATORS

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unit operator +(unit value1, unit value2)
        {
            return new unit() { Value = value1.Value + value2.Value };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unit operator -(unit value1, unit value2)
        {
            return new unit() { Value = value1.Value - value2.Value };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unit operator *(unit value1, float f)
        {
            return new unit() { Value = value1.Value * f };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(unit val1, unit val2)
        {
            return val1.Value == val2.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(unit val1, unit val2)
        {
            return val1.Value != val2.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(unit value1, unit value2)
        {
            return value1.Value > value2.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(unit value1, unit value2)
        {
            return value1.Value < value2.Value;
        }

        #endregion

        #region IMPLICITS

        [MethodImpl(MethodImplOptions.AggressiveInlining)]

        public static implicit operator float(unit value)
        {
            return value.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator unit(float v)
        {
            return new unit()
            {
                Value = v,
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator string(unit c)
        {
            return c.ToString();
        }

        #endregion
    }
}
