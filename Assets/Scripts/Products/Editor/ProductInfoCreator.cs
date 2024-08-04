using System;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using UnityEngine;
using Supermarket.Products;

public class ProductInfoCreator : EditorWindow
{
    public const string ProductAssetPath = "Assets/GameAssets/Products/";
    public const string GROUP_NAME = "Default Local Group";
    public const string LABEL = "products";
    public static Action<ProductInfo> OnCreated;
    public static void OpenWindow(Action<ProductInfo> OnCreated)
    {
        var window = GetWindow<ProductInfoCreator>();
        window.titleContent = new GUIContent("New Product");
        window.maxSize = window.minSize = new Vector2(400, 300);
        window.Show();

        ProductInfoCreator.OnCreated = OnCreated;
    }

    string path;

    private void OnEnable()
    {
        path = $"{ProductAssetPath}{$"New Product Info {Guid.NewGuid().ToString().Substring(0, 8)}"}.asset";
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        path = GUILayout.TextField(path);
        if (GUILayout.Button(new GUIContent("...")))
        {
            path = EditorUtility.SaveFilePanelInProject("New Product Info", $"New Product Info {Guid.NewGuid().ToString().Substring(0, 8)}", "asset", "Choose The Path");
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button(new GUIContent("Create")))
        {
            ProductInfo data = ScriptableObject.CreateInstance<ProductInfo>();
            Create(data, path);
        }
    }

    private ProductInfo Create(ProductInfo asset, string path)
    {
        if (!asset)
        {
            Debug.LogError("failed to create asset");
            return null;
        }
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        //EditorUtility.RevealInFinder(Path.GetDirectoryName(AssetDatabase.GetAssetPath(asset)));
        MoveToAddressable(GROUP_NAME, LABEL, path);
        OnCreated(asset);
        Selection.activeObject = asset;
        return asset;
    }

    private void MoveToAddressable(string group, string label, string assetPath)
    {
        // Create or find the Addressable settings
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(true);

        if (settings == null)
        {
            Debug.LogError("Addressable settings not found. Please create Addressable settings from the Addressables window.");
            return;
        }

        // Load the ScriptableObject
        ScriptableObject asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
        if (asset == null)
        {
            Debug.LogError("Asset not found at path: " + assetPath);
            return;
        }

        // Create a new Addressable Asset Entry
        string guid = AssetDatabase.AssetPathToGUID(assetPath);
        AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, settings.DefaultGroup);

        if (entry == null)
        {
            Debug.LogError("Failed to create or move addressable asset entry.");
            return;
        }

        // Assign a label to the Addressable asset
        entry.SetLabel(label, true);

        // Save the settings
        settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true);
        AssetDatabase.SaveAssets();
    }
}