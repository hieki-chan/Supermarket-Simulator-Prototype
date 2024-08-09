using DG.Tweening;
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

        rectTransform.DOMoveY(50, .2f);
    }

    public void Hide()
    {
        if (!gameObject.activeSelf)
            return;

        rectTransform.DOMoveY(0, .2f).onComplete += () => gameObject.SetActive(false);
    }
}