using Hieki.Utils;
using TMPro;
using UnityEngine;

public class TooltipView : MonoBehaviour
{
    public TextMeshProUGUI tooltipText;

    RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Show(string text)
    {
        tooltipText.text = text;
        gameObject.SetActive(true);
        rectTransform.anchoredPosition = Vector3.zero;
        LeanTween.moveY(rectTransform, 50, .2f);
    }

    public void Hide()
    {
        if (!gameObject.activeSelf)
            return;
        LeanTween.moveY(rectTransform, 0, .2f);
        Flag.Disable(gameObject, .25f);
    }
}