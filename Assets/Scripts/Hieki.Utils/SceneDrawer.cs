using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Hieki.Utils
{
    public static class SceneDrawer
    {
        #region DRAW WIRE CUBES
        public static void DrawWireCubeDebug(Vector3 center, Vector3 size, Quaternion rotation, Color color, float duration)
        {
#if UNITY_EDITOR

            var (down_bottom_left, down_top_left, down_top_right, down_bottom_right, up_bottom_left, up_top_left, up_top_right, up_bottom_right) = GetBoxCorners(center, size, rotation);

            Debug.DrawLine(down_bottom_left, down_top_left, color, duration);
            Debug.DrawLine(down_top_left, down_top_right, color, duration);
            Debug.DrawLine(down_top_right, down_bottom_right, color, duration);
            Debug.DrawLine(down_bottom_right, down_bottom_left, color, duration);

            Debug.DrawLine(up_bottom_left, up_top_left, color, duration);
            Debug.DrawLine(up_top_left, up_top_right, color, duration);
            Debug.DrawLine(up_top_right, up_bottom_right, color, duration);
            Debug.DrawLine(up_bottom_right, up_bottom_left, color, duration);

            Debug.DrawLine(down_bottom_left, up_bottom_left, color, duration);
            Debug.DrawLine(down_top_left, up_top_left, color, duration);
            Debug.DrawLine(down_top_right, up_top_right, color, duration);
            Debug.DrawLine(down_bottom_right, up_bottom_right, color, duration);
#endif
        }

        public static void DrawWireCubeGizmo(Vector3 center, Vector3 size, Quaternion rotation, Color color)
        {
#if UNITY_EDITOR
            var (down_bottom_left, down_top_left, down_top_right, down_bottom_right, up_bottom_left, up_top_left, up_top_right, up_bottom_right) = GetBoxCorners(center, size, rotation);

            Gizmos.color = color;

            Gizmos.DrawLine(down_bottom_left, down_top_left);
            Gizmos.DrawLine(down_top_left, down_top_right);
            Gizmos.DrawLine(down_top_right, down_bottom_right);
            Gizmos.DrawLine(down_bottom_right, down_bottom_left);

            Gizmos.DrawLine(up_bottom_left, up_top_left);
            Gizmos.DrawLine(up_top_left, up_top_right);
            Gizmos.DrawLine(up_top_right, up_bottom_right);
            Gizmos.DrawLine(up_bottom_right, up_bottom_left);

            Gizmos.DrawLine(down_bottom_left, up_bottom_left);
            Gizmos.DrawLine(down_top_left, up_top_left);
            Gizmos.DrawLine(down_top_right, up_top_right);
            Gizmos.DrawLine(down_bottom_right, up_bottom_right);
#endif
        }

        public static void DrawWireCubeHandles(Vector3 center, Vector3 size, Quaternion rotation, Color color, float thickness)
        {
#if UNITY_EDITOR
            var (down_bottom_left, down_top_left, down_top_right, down_bottom_right, up_bottom_left, up_top_left, up_top_right, up_bottom_right) = GetBoxCorners(center, size, rotation);

            Handles.color = color;

            Handles.DrawLine(down_bottom_left, down_top_left, thickness);
            Handles.DrawLine(down_top_left, down_top_right, thickness);
            Handles.DrawLine(down_top_right, down_bottom_right, thickness);
            Handles.DrawLine(down_bottom_right, down_bottom_left, thickness);

            Handles.DrawLine(up_bottom_left, up_top_left, thickness);
            Handles.DrawLine(up_top_left, up_top_right, thickness);
            Handles.DrawLine(up_top_right, up_bottom_right, thickness);
            Handles.DrawLine(up_bottom_right, up_bottom_left, thickness);

            Handles.DrawLine(down_bottom_left, up_bottom_left, thickness);
            Handles.DrawLine(down_top_left, up_top_left, thickness);
            Handles.DrawLine(down_top_right, up_top_right, thickness);
            Handles.DrawLine(down_bottom_right, up_bottom_right, thickness);
#endif
        }


        public static (Vector3 down_bottom_left, Vector3 down_top_left, Vector3 down_top_right, Vector3 down_bottom_right,
            Vector3 up_bottom_left, Vector3 up_top_left, Vector3 up_top_right, Vector3 up_bottom_right
            ) GetBoxCorners(Vector3 center, Vector3 size, Quaternion rotation)
        {
            float halfSizeX = size.x / 2;
            float halfSizeY = size.y / 2;
            float halfSizeZ = size.z / 2;

            Vector3 down_bottom_left = center + rotation * new Vector3(-halfSizeX, -halfSizeY, -halfSizeZ);
            Vector3 down_top_left = center + rotation * new Vector3(-halfSizeX, -halfSizeY, halfSizeZ);
            Vector3 down_top_right = center + rotation * new Vector3(halfSizeX, -halfSizeY, halfSizeZ);
            Vector3 down_bottom_right = center + rotation * new Vector3(halfSizeX, -halfSizeY, -halfSizeZ);

            Vector3 up_bottom_left = center + rotation * new Vector3(-halfSizeX, halfSizeY, -halfSizeZ);
            Vector3 up_top_left = center + rotation * new Vector3(-halfSizeX, halfSizeY, halfSizeZ);
            Vector3 up_top_right = center + rotation * new Vector3(halfSizeX, halfSizeY, halfSizeZ);
            Vector3 up_bottom_right = center + rotation * new Vector3(halfSizeX, halfSizeY, -halfSizeZ);

            return (down_bottom_left, down_top_left, down_top_right, down_bottom_right, up_bottom_left, up_top_left, up_top_right, up_bottom_right);
        }

        #endregion

        #region DRAW ARROWS

        public static void ArrowGizmo(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
#if UNITY_EDITOR
            Gizmos.DrawRay(pos, direction);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
#endif
        }

        public static void ArrowGizmo(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
#if UNITY_EDITOR
            Gizmos.color = color;
            Gizmos.DrawRay(pos, direction);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
#endif
        }

        public static void ArrowDebug(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
#if UNITY_EDITOR
            Debug.DrawRay(pos, direction);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Debug.DrawRay(pos + direction, right * arrowHeadLength);
            Debug.DrawRay(pos + direction, left * arrowHeadLength);
#endif
        }
        public static void ArrowDebug(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
#if UNITY_EDITOR
            Debug.DrawRay(pos, direction, color);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Debug.DrawRay(pos + direction, right * arrowHeadLength, color);
            Debug.DrawRay(pos + direction, left * arrowHeadLength, color);
#endif
        }
        #endregion
    }
}
