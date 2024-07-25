using UnityEngine;

namespace Hieki.Utils
{
    public static class VectorExtensions
    {
        /// <summary>
        /// Returns a <see cref="Vector3"/> that rotates <paramref name="dir"/> around the y axis
        /// </summary>
        public static Vector3 RotateVector(this Vector3 dir, float eulerAnglesY)
        {
            return Quaternion.Euler(0, eulerAnglesY, 0) * dir;
        }

        /// <summary>
        /// Returns a <see cref="Vector3"/> that rotates <paramref name="dir"/> around the y axis from origin
        /// </summary>
        public static Vector3 RotatePosition(this Vector3 pos, float eulerAnglesY, Vector3 origin)
        {
            return Quaternion.Euler(0, eulerAnglesY, 0) * (pos - origin);
        }
    }
}
