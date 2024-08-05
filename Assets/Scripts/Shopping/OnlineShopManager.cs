using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Supermarket.Products;
using System.Linq;
using Supermarket.Pricing;
using Cysharp.Threading.Tasks;

public class OnlineShopManager : MonoBehaviour
{
    const string PRODUCTS_KEY = "products";
    const string LICENSES_KEY = "license";

    private List<ProductInfo> productsInfo;
    private List<License> licenses;

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

        LoadAllItems().Forget();
    }

    private async UniTaskVoid LoadAllItems()
    {
        //Product Infos
        AsyncOperationHandle<IList<ProductInfo>> handle = Addressables.LoadAssetsAsync<ProductInfo>(PRODUCTS_KEY, loadedProduct => { });

        await handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            productsInfo = handle.Result.ToList();
            shopView.OnItemsLoaded(productsInfo);
        }

        //Licenses
        AsyncOperationHandle<IList<License>> handle2 = Addressables.LoadAssetsAsync<License>(LICENSES_KEY, loadedProduct => { });

        await handle2;

        if(handle2.Status == AsyncOperationStatus.Succeeded)
        {
            licenses = handle2.Result.ToList();
            shopView.OnLicensesLoaded(licenses);
        }
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
        unit totalCost = cartData.TotalCost();
        if (SupermarketManager.Mine.Money < totalCost)
        {
            //
        }

        deliveryManager.Order(cartData);
        cartData.Clear();
        shopView.OnBought();
    }
}