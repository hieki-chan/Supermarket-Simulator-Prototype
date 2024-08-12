using Supermarket;
using Supermarket.Customers;
using Supermarket.Pricing;
using Supermarket.Products;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-99)]
public class SupermarketManager : MonoBehaviour
{
    public static SupermarketManager Mine { get; private set; }

    //---------------------------STORE DATA------------------------------\\
    private StoreData storeData;

    private unit Money { get => storeData.money; set => storeData.money = value; }

    //-------------------------------------------------------------------\\

    [NonEditable] public SupermarketState State;

    [NonEditable] public List<Storage> Storages;
    [NonEditable] public CheckoutDesk CheckoutDesk;

    
    private List<ItemPricing> itemPricings = new List<ItemPricing>(10);


    [Header("UI"), Space]
    public PriceSettingView priceSettings;

    //----------------------------EVENTS---------------------------------\\
    public UnityAction<unit> OnMoneyChanged;
    public UnityAction<int> OnLevelUpgraded;
    public UnityAction<float> OnLevelInProgress;

    //-------------------------------------------------------------------\\


    private void Awake()
    {
        if (Mine == null)
            Mine = this;
        else
            Debug.LogWarning($"there's more than 1 {GetType()} in the scene", this);

        Storages = FindObjectsByType<Storage>(FindObjectsSortMode.None).ToList();
        CheckoutDesk = FindObjectOfType<CheckoutDesk>();

        storeData = new StoreData()
        {
            levelProgress = 0,
            storeLevel = 0,
            money = 10000,
        };

        OnMoneyChanged?.Invoke(Money);
        OnLevelInProgress?.Invoke(storeData.storeLevel);
        OnLevelUpgraded?.Invoke(storeData.storeLevel);
    }

    public ItemPricing GetItemPricing(ProductInfo product)
    {
        ItemPricing item = itemPricings.FirstOrDefault(a => a.product == product);

        if (item == null)
        {
            item = new ItemPricing()
            {
                product = product,
                price = product.UnitCost,
            };
            itemPricings.Add(item);
        }

        return item;
    }

    public Storage GetAvailableStorage()
    {
        int count = Storages.Count;
        int realIndex = Random.Range(0, count);
        int loopIndex = realIndex;

        for (int i = 0; i < count; i++)
        {
            var storage = Storages[realIndex];

            if (storage.IsAvailable())
            {
                return storage;
            }

            loopIndex++;
            realIndex = loopIndex >= count ? loopIndex - count : loopIndex;
        }

        return null;
    }

    public void SellFurniture()
    {

    }

    public bool TryConsume(unit amout)
    {
        if(Money >= amout)
        {
            Money -= amout;
            OnMoneyChanged?.Invoke(Money);
            return true;
        }

        return false;
    }

    public void Store(unit amout)
    {
        Money += amout;
        OnMoneyChanged?.Invoke(Money);
    }


#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init()  //domain reloading
    {
        Mine = null;
    }
#endif
}

public enum SupermarketState
{
    Open,
    Closed,
}
