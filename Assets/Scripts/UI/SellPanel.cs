using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class SellPanel : MonoBehaviour
{
    public TextMeshProUGUI furnitureName;
    public TextMeshProUGUI value;

    public Button confirmButton;
    public Button cancelButton;

    Action OnComfirm;

    private void Awake()
    {
        confirmButton.onClick.AddListener(Confirm);
        cancelButton.onClick.AddListener(Cancel);
    }

    public void Sell(Action OnComfirm)
    {
        this.OnComfirm = OnComfirm;

        gameObject.SetActive(true);
        Appear().Forget();
    }

    async UniTaskVoid Appear()
    {
        transform.localScale = Vector3.zero;

        await transform.DOScale(1, .25f);
    }

    void Confirm()
    {
        OnComfirm?.Invoke();
        gameObject.SetActive(false);
    }

    void Cancel()
    {
        gameObject.SetActive(false);
    }
}
