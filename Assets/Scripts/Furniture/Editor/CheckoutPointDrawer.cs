using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Hieki.Utils;

[CustomEditor(typeof(CheckoutDesk))]
internal class CheckoutPointDrawer : Editor
{
    CheckoutDesk desk;

    private void OnEnable()
    {
        desk = (CheckoutDesk)target;
    }

    private void OnSceneGUI()
    {
        List<CheckoutDesk.CheckoutPoint> line = desk.CheckoutLine;
        if (line == null)
            return;

        for (int i = 0; i < line.Count; i++)
        {
            CheckoutDesk.CheckoutPoint point = line[i];
            Vector3 worldPosition = point.position;

            EditorGUI.BeginChangeCheck();
            worldPosition = Handles.PositionHandle(worldPosition, Quaternion.Euler(0, point.rotateY, 0));
            if (EditorGUI.EndChangeCheck())
            {
                point.position = worldPosition;
                serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.BeginChangeCheck();
            float newRotationY = Handles.RotationHandle(Quaternion.Euler(0, point.rotateY, 0), worldPosition).eulerAngles.y;
            if (EditorGUI.EndChangeCheck())
            {
                point.rotateY = newRotationY;
            }

            Color color = point.isTaked ? Color.red : Color.green;
            SceneDrawer.DrawWireCubeHandles(worldPosition, .35f * new Vector3(1, 0, 1), Quaternion.Euler(0, point.rotateY, 0), color, 1.5f);
            Handles.Label(worldPosition, $"{i}", GUI.skin.box);

            if (i + 1 < line.Count)
                Handles.DrawLine(point.position, line[i + 1].position);
        }
    }
}