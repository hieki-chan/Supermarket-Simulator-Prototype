using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Supermarket.Products;
using System.Linq;
using Supermarket.Pricing;
using Cysharp.Threading.Tasks;
using Hieki.Pubsub;

public class OnlineShopManager : MonoBehaviour
{
    const string PRODUCTS_KEY = "products";
    const string LICENSES_KEY = "license";

    private List<ProductInfo> productsInfo;
    private List<License> licenses;

    [SerializeField] CartData cartData;

    public ShopView shopView;

    //----------------------------Open/Close Shop-----------------------------\\
    Topic shopTopic = Topic.FromString("shop-state");

    //----------------------------Buy-----------------------------\\
    Topic buyTopic = Topic.FromString("buy-delivery");

    //======================Publish/Subscribe========================\\
    IPublisher publisher = new Publisher();
    ISubscriber subscriber = new Subscriber();

    private void Awake()
    {
        cartData = new CartData();
        shopView.OnAddToCart += OnAddToCart;
        shopView.OnRemovedFromCart += OnRemoveFromCart;
        shopView.OnBuy += Buy;
        shopView.OnShopClosed += () => publisher.Publish(shopTopic, new OnlineShopMessage(false));

        subscriber.Subscribe<OnlineShopMessage>(shopTopic, (message) =>
        {
            shopView.gameObject.SetActive(message.state);
        });

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

        if (SupermarketManager.Mine.TryConsume(totalCost))
        {
            //
        }

        publisher.Publish(buyTopic, cartData);

        cartData.Clear();
        shopView.OnBought();
    }
}