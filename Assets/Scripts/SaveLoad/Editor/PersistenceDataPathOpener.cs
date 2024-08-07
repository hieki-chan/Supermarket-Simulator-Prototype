#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

internal static class PersistenceDataPathOpener
{
    [MenuItem("File/Show Persistence Data In Explorer", priority = 4)]
    public static void ShowPersistenceDataInExplorer()
    {
        EditorUtility.RevealInFinder(Application.persistentDataPath);
    }
}
#endif
