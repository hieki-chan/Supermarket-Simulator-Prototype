using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Supermarket.Products;
using System.Linq;
using Supermarket.Pricing;
using Supermarket;

public class OnlineShopManager : MonoBehaviour
{

    const string PRODUCTS_KEY = "products";

    private List<ProductInfo> productsInfo;

    [SerializeField] CartData cartData;
    Laptop laptop;

    public ShopView shopView;

    public DeliveryManager deliveryManager;

    private void Awake()
    {
        cartData = new CartData();
        shopView.OnAddToCart += OnAddToCart;
        shopView.OnRemovedFromCart += OnRemoveFromCart;
        shopView.OnShopClosed += () => laptop.OnShopClosed();
        shopView.OnBuy += Buy;
        laptop = SupermarketManager.Mine.Laptop;
        laptop.OnOpenShop += () => shopView.gameObject.SetActive(true);

        StartCoroutine(LoadAllItems());
    }

    IEnumerator LoadAllItems()
    {
        AsyncOperationHandle<IList<ProductInfo>> handle = Addressables.LoadAssetsAsync<ProductInfo>(PRODUCTS_KEY, loadedProduct => { });

        yield return handle;

        productsInfo = handle.Result.ToList();

        shopView.LoadAllItems(productsInfo);
    }

    public void OnAddToCart(CartItem cartItem)
    {
        cartData.Add(cartItem);
        var cost = cartData.TotalCost();
        shopView.DisplayTotalCost(cost.ToString());

        //CurrencyConverter.Convert<StandardCurrency, UnitedStatesDollar>(cost);
    }

    void OnRemoveFromCart(Company company, int index)
    {
        cartData.Remove(company, index);
        shopView.DisplayTotalCost(cartData.TotalCost().ToString());
    }

    void Buy()
    {
        StandardCurrency totalCost = cartData.TotalCost();
        if (SupermarketManager.Mine.Money < totalCost)
        {
            //
        }

        deliveryManager.Order(cartData);
        cartData.Clear();
        shopView.OnBought();
    }
}