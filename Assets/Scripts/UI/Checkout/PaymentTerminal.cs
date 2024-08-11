using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Text;
using TMPro;
using UnityEngine;

using UnityEngine.UI;

public class PaymentTerminal : MonoBehaviour
{
    public TextMeshProUGUI totalValueText;
    [Space]
    public Button numberButton1;
    public Button numberButton2;
    public Button numberButton3;
    public Button numberButton4;
    public Button numberButton5;
    public Button numberButton6;
    public Button numberButton7;
    public Button numberButton8;
    public Button numberButton9;
    public Button numberButton0;

    [Space]
    public Button deleteButton;
    public Button dotButton;
    public Button okButton;

    StringBuilder textValue;
    int digitIndex;

    RectTransform rectTrans;

    float value;
    Action OnIncorrect;
    Action OnCorrect;

    private void Awake()
    {
        numberButton1.onClick.AddListener(() => OnNumberButtonClick(1));
        numberButton2.onClick.AddListener(() => OnNumberButtonClick(2));
        numberButton3.onClick.AddListener(() => OnNumberButtonClick(3));
        numberButton4.onClick.AddListener(() => OnNumberButtonClick(4));
        numberButton5.onClick.AddListener(() => OnNumberButtonClick(5));
        numberButton6.onClick.AddListener(() => OnNumberButtonClick(6));
        numberButton7.onClick.AddListener(() => OnNumberButtonClick(7));
        numberButton8.onClick.AddListener(() => OnNumberButtonClick(8));
        numberButton9.onClick.AddListener(() => OnNumberButtonClick(9));
        numberButton0.onClick.AddListener(() => OnNumberButtonClick(0));

        deleteButton.onClick.AddListener(OnDelete);
        dotButton.onClick.AddListener(OnDegitClick);
        okButton.onClick.AddListener(OnOk);

        textValue = new StringBuilder(6);
        digitIndex = -1;

        rectTrans = GetComponent<RectTransform>();
    }

    private async void OnEnable()
    {
        rectTrans.anchoredPosition = new Vector2(rectTrans.anchoredPosition.x, -1000);

        await rectTrans.DOMoveY(0, .2f);
    }

    public void Check(float value, Action OnCorrect, Action OnIncorrect)
    {
        this.value = value;
        this.OnCorrect = OnCorrect;
        this.OnIncorrect = OnIncorrect;

        SetText();
        gameObject.SetActive(true);
    }

    void OnNumberButtonClick(int val)
    {
        if (textValue.Length >= 6)
            return;

        textValue.Append(val.ToString());
        SetText();
    }

    void OnDegitClick()
    {
        if (textValue.Length >= 6 || digitIndex != -1)
            return;
        textValue = textValue.Append('.');
        SetText();

        digitIndex = textValue.Length - 1;
    }

    void OnDelete()
    {
        if (textValue.Length <= 0)
            return;

        textValue = textValue.Remove(textValue.Length - 1, 1);
        if (textValue.Length <= digitIndex)
            digitIndex = -1;
        SetText();
    }

    void SetText()
    {
        totalValueText.text = $"${textValue}";
    }

    void OnOk()
    {
        if (textValue.Length == 0 || textValue[^1] == '.')
        {
            OnIncorrect?.Invoke();
            return;
        }
        if(float.TryParse(textValue.ToString(), out float result))
        {
            if (result == value)
            {
                OnCorrect?.Invoke();
                gameObject.SetActive(false);
                textValue.Clear();
                return;
            }
        }

        OnIncorrect?.Invoke();
    }
}