using UnityEngine;
using UnityEditor;
using Supermarket.Customers;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Node))]
internal class NodeEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Draw the foldout
        property.isExpanded = EditorGUI.Foldout(
            new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
            property.isExpanded,
            label
        );

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;

            SerializedProperty verticesProp = property.FindPropertyRelative("vertex");
            SerializedProperty rotateYProp = property.FindPropertyRelative("rotateY");
            SerializedProperty rangeProp = property.FindPropertyRelative("range");
            //SerializedProperty connectedProp = property.FindPropertyRelative("connected");
            //SerializedProperty connectToProp = property.FindPropertyRelative("connectTo");

            PropField(verticesProp, position, 1);

            PropField(rotateYProp, position, 2);

            PropField(rangeProp, position, 3);

            //PropField(connectedProp, position, 4);

            // draw the connectTo property if connected is true
            //if (connectedProp.boolValue)
            //{
            //    PropField(connectToProp, position, 5);
            //}

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    void PropField(SerializedProperty prop, Rect position, int i)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;
        Rect rotateYRect = new Rect(position.x, position.y + (lineHeight + spacing) * i, position.width, lineHeight);
        EditorGUI.PropertyField(rotateYRect, prop);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int lineCount = 2;
        if (property.isExpanded)
        {
            //if (property.FindPropertyRelative("connected").boolValue)
            //{
            //    lineCount++;
            //}
            lineCount+=2;
        }

        return lineCount * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
    }
}
#endif