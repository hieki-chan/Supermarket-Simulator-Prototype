using System;
using UnityEngine;
using UnityEngine.UI;

public class MoneyChanger : MonoBehaviour
{
    public Button OkButton;
    public Button ResetButton;

    [Space]
    public Button bankote1;
    public Button bankote5;
    public Button bankote10;
    public Button bankote20;
    public Button bankote50;
    [Space]
    public Button coin0_01;
    public Button coin0_05;
    public Button coin0_10;
    public Button coin0_20;
    public Button coin0_50;

    public Action<float, float> OnClick;
    public Action OnReset;
    public Func<float, bool> OnOk;

    float totalGiving = 0;

    private void Awake()
    {
        OkButton.onClick.AddListener(() =>
        {
            if ((bool)(OnOk?.Invoke(totalGiving)))
            {
                gameObject.SetActive(false);
            }
        });
        ResetButton.onClick.AddListener(() => { OnReset?.Invoke(); totalGiving = 0; });

        bankote1.onClick.AddListener(() => { Give(1); });
        bankote5.onClick.AddListener(() => { Give(5); });
        bankote10.onClick.AddListener(() => { Give(10); });
        bankote20.onClick.AddListener(() => { Give(20); });
        bankote50.onClick.AddListener(() => { Give(50); });

        coin0_01.onClick.AddListener(() => { Give(.01f); });
        coin0_05.onClick.AddListener(() => { Give(.05f); });
        coin0_10.onClick.AddListener(() => { Give(.10f); });
        coin0_20.onClick.AddListener(() => { Give(.20f); });
        coin0_50.onClick.AddListener(() => { Give(.50f); });
    }

    public void Check(Action<float, float> OnClick, Action OnReset, Func<float, bool> OnOK)
    {
        this.OnClick = OnClick;
        this.OnReset = OnReset;
        this.OnOk = OnOK;
        gameObject.SetActive(true);
        totalGiving = 0;
    }

    public void Give(float val)
    {
        totalGiving += val;
        OnClick?.Invoke(val, totalGiving);
    }
}