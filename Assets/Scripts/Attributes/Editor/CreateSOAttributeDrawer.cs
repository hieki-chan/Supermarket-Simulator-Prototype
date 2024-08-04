using UnityEngine;
using UnityEditor;
using System.Reflection;
using Supermarket.Products;
using Unity.VisualScripting;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(NewProductAttribute))]
public class CreateSOAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        position.width -= 80;
        EditorGUI.ObjectField(position, property, label);

        position.x += position.width;
        position.width = 80;
        if (GUI.Button(position, new GUIContent("New")))
        {
            ProductInfoCreator.OpenWindow((productInfo) =>
            {
                if (productInfo)
                {
                    property.objectReferenceValue = productInfo;
                    if (property.serializedObject.targetObject is Furniture fur)
                    {
                        SetValue(productInfo, "m_ProductType", ProductType.Furniture);
                        SetValue(productInfo, "m_Furniture", fur.GetPrefabDefinition());
                    }
                    else if (property.serializedObject.targetObject is ProductOnSale pos)
                    {
                        SetValue(productInfo, "m_ProductType", ProductType.Products);
                        SetValue(productInfo, "m_ProductOnSale", pos.GetPrefabDefinition());
                    }
                    property.serializedObject.ApplyModifiedProperties();
                }
            });

        }
        EditorGUI.EndProperty();
    }

    void SetValue(ProductInfo productInfo, string fieldName, object value)
    {
        FieldInfo f = productInfo.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        if (f != null)
        {
            f.SetValue(productInfo, value);
        }
    }
}
#endif
