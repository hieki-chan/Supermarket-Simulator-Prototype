/*using UnityEngine;
using UnityEditor;
using Supermarket.Customers;
using System.Reflection;
using System.Linq;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(CustomerSM))]
internal class CustomerStateHandlerDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var target = property.serializedObject.targetObject;

        FieldInfo fieldInfo = target.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
            .Where(f => f.FieldType == typeof(CustomerSM)).FirstOrDefault();

        EditorGUILayout.PrefixLabel(fieldInfo.Name);

        CustomerSM handler = (CustomerSM)fieldInfo.GetValue(target);
        CustomerTree currState = handler.editorCurrentState;

        EditorGUI.indentLevel++;
        EditorGUILayout.LabelField(new GUIContent($"Current State : {(currState == null ? "None" : currState.GetType().FullName)}"), GUI.skin.box);

        if(currState != null)
        {
            var viewables = currState.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(f => f.GetCustomAttribute<ViewableAttribute>() != null);

            foreach (var viewable in viewables)
            {
                EditorGUILayout.LabelField($"     {viewable.Name} : {viewable.GetValue(currState)}     ", GUI.skin.box);
            }
        }
        EditorGUI.indentLevel--;
        

        EditorGUI.EndProperty();
    }
}
#endif*/