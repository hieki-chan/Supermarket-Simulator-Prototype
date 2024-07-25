using UnityEngine;

namespace Hieki.Utils
{
    public static class Position
    {
        public static Vector3 Offset(Transform transform, Vector3 offset)
        {
            return transform.position + transform.forward * offset.z + transform.right * offset.x + transform.up * offset.y;
        }
    }
}