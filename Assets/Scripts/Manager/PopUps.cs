using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Supermarket.Products;
using Cysharp.Threading.Tasks;
using Hieki.Pubsub;

public class PopUps : MonoBehaviour
{
    static PaymentTerminal paymentTerminal;
    static MoneyChanger moneyChanger;
    static SellPanel sellPopup;

    public AssetReferenceGameObject paymentTerminalAsset;
    public AssetReferenceGameObject moneyChangerAsset;
    public AssetReferenceGameObject sellPopupAsset;

    public Transform parent;

    ISubscriber subscriber = new Subscriber();

    private void Awake()
    {
        subscriber.Subscribe<CheckoutDesk.CardPayNotify>(CheckoutDesk.CardPayTopic, CardPay);
        subscriber.Subscribe<CheckoutDesk.CashPayNotify>(CheckoutDesk.CashPayTopic, CashPay);

        subscriber.Subscribe<Furniture.TrySellNotify>(Furniture.trySellTopic, SellFurniture);
    }

    public void CardPay(CheckoutDesk.CardPayNotify notify)
    {
        PaymentTerminal(notify.value, notify.OnCorrect, notify.OnIncorrect).Forget();
    }

    public void CashPay(CheckoutDesk.CashPayNotify notify)
    {
        MoneyChange(notify.value, notify.OnClick, notify.OnReset, notify.OnCorrect, notify.OnIncorrect).Forget();
    }

    public void SellFurniture(Furniture.TrySellNotify notify)
    {
        SellPopup(notify.OnConfirm).Forget();
    }

    public async UniTaskVoid PaymentTerminal(float value, Action OnCorrect, Action OnIncorrect)
    {
        if (paymentTerminal == null)
        {
            paymentTerminal = await LoadAssetAsync<PaymentTerminal>(paymentTerminalAsset, parent);
        }

        paymentTerminal.Check(value, OnCorrect, OnIncorrect);
    }

    public async UniTaskVoid MoneyChange(float value, Action<float, float> OnGet, Action OnReset, Action OnCorrect, Action OnIncorrect)
    {
        if (moneyChanger == null)
        {
            moneyChanger = await LoadAssetAsync<MoneyChanger>(moneyChangerAsset, parent);
        }

        moneyChanger.Check(value, OnGet, OnReset, OnCorrect, OnIncorrect);
    }

    public async UniTaskVoid SellPopup(Action OnConfirm)
    {
        if (sellPopup == null)
        {
            sellPopup = await LoadAssetAsync<SellPanel>(sellPopupAsset, parent);
        }

        sellPopup.Sell(OnConfirm);
    }

    private async static UniTask<T> LoadAssetAsync<T>(AssetReferenceGameObject asset, Transform parent = null) where T : UnityEngine.Object
    {
        var handle = Addressables.LoadAssetAsync<GameObject>(asset);

        await handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var go = Instantiate(handle.Result);
            go.SetActive(false);
            if (parent)
            {
                go.transform.SetParent(parent, false);
                go.transform.SetSiblingIndex(parent.childCount - 2);
            }

            return go.GetComponent<T>();
        }

        return null;
    }

#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ReloadDomain()
    {
        paymentTerminal = null;
        moneyChanger = null;
        sellPopup = null;
    }
#endif
}