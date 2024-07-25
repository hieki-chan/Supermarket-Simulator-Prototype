using UnityEngine;

public class GameManager : MonoBehaviour
{
#if UNITY_EDITOR
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0, 0, 200, 100));
        GUILayout.Box("Game Speed:" + Mathf.Round(Time.timeScale) + "x");
        Time.timeScale = GUILayout.HorizontalSlider(Time.timeScale, 0, 5);
        GUILayout.EndArea();
    }
#endif
}
