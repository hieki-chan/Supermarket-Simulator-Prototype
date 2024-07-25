using Supermarket.Customers;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MovementPath))]
public class MovementPathEditor : Editor
{
    private MovementPath path;

    private void OnEnable()
    {
        path = (MovementPath)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        serializedObject.ApplyModifiedProperties();
    }

    private void OnSceneGUI()
    {
        if (!path)
            return;

        for (int i = 0; i < path.Count; i++)
        {
            Node vertex = path.Nodes[i];
            Vector3 worldPosition = path[i, true];

            EditorGUI.BeginChangeCheck();
            worldPosition = Handles.PositionHandle(worldPosition, Quaternion.Euler(0, vertex.rotateY, 0));
            if (EditorGUI.EndChangeCheck())
            {
                path[i, false] = worldPosition;
                //Undo.RecordObject(path, $"Move Position Path At {i}");
                serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.BeginChangeCheck();
            float newRotationY = Handles.RotationHandle(Quaternion.Euler(0, vertex.rotateY, 0), worldPosition).eulerAngles.y;
            if (EditorGUI.EndChangeCheck())
            {
                //Undo.RecordObject(path, $"Rotate Y At {i}");
                Node node = path.Nodes[i];
                node.rotateY = newRotationY;
                path.Nodes[i] = node;
            }

            Handles.DrawWireArc(worldPosition, Vector3.up, Vector3.forward, 360, vertex.range);
            Handles.Label(worldPosition, $"{i})", GUI.skin.box);
            //Handles.Label(worldPosition, $"{i} - ({(!vertex.connected ? Mathf.Clamp(i - 1, 0, i) : vertex.connectTo)})", GUI.skin.box);

            //if (vertex.connected)
            //{
            //    Handles.DrawLine(path[i, true], path[vertex.connectTo, true]);
            //}

            if (i + 1 < path.Count/* && !path.Nodes[i + 1].connected*/)
            {
                Vector3 from = path[i, true];
                Vector3 to = path[i + 1, true];
                //main line
                Handles.DrawLine(from, to);

                Node node = path.Nodes[i + 1];
                Vector3 fromDir = Quaternion.Euler(0, vertex.rotateY, 0) * Vector3.forward;
                Vector3 toDir = Quaternion.Euler(0, vertex.rotateY, 0) * Vector3.forward;

                Vector3 fromCross = Vector3.Cross(fromDir, Vector3.up);
                Vector3 toCross = Vector3.Cross(toDir, Vector3.up);
                //left
                Handles.DrawLine(from + fromCross * vertex.range, to + toCross * node.range);
                //right
                Handles.DrawLine(from - fromCross * vertex.range, to - toCross * node.range);
            }
        }
    }
}
