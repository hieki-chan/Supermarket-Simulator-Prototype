using Supermarket.Customers;
using Supermarket.Pricing;
using Supermarket.Products;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class SupermarketManager : MonoBehaviour
{
    public static SupermarketManager Mine { get; private set; }

    [NonEditable] public SupermarketState State;

    [NonEditable] public List<Storage> Storages;
    [NonEditable] public CheckoutDesk CheckoutDesk;
    [NonEditable] public Laptop Laptop;

    private List<ItemPricing> itemPricings = new List<ItemPricing>(10);

    public unit Money => money;
    [SerializeField] private unit money;

    [Header("UI"), Space]
    public PriceSettingView priceSettings;

    private void Awake()
    {
        if (Mine == null)
            Mine = this;
        else
            Debug.LogWarning($"there's more than 1 {GetType()} in the scene", this);

        Storages = FindObjectsByType<Storage>(FindObjectsSortMode.None).ToList();
        CheckoutDesk = FindObjectOfType<CheckoutDesk>();
        Laptop = FindObjectOfType<Laptop>();
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

            if (storage.CheckProduct())
            {
                return storage;
            }

            loopIndex++;
            realIndex = (int)Mathf.Repeat(loopIndex, count - 1);
        }

        return null;
    }

    public bool TryConsume(unit amout)
    {
        if(money >= amout)
        {
            money -= amout;
            return true;
        }

        return false;
    }

    public void Store(unit amout)
    {
        money += amout;
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
