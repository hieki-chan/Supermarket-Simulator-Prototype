using UnityEngine;
using Supermarket.Products;
using Supermarket.Pricing;
using Hieki.Pubsub;

public class OnlineShopPresenter : MonoBehaviour
{
    [SerializeField] CartData cartData;

    public ShopView shopView;

    //----------------------------Open/Close Shop-----------------------------\\
    Topic shopTopic = Topic.FromString("shop-state");

    //----------------------------Buy-----------------------------\\
    Topic buyTopic = Topic.FromString("buy-delivery");

    //======================Publish/Subscribe========================\\
    IPublisher publisher = new Publisher();
    ISubscriber subscriber = new Subscriber();

    private async void Awake()
    {
        cartData = new CartData();
        shopView.OnAddToCart += OnAddToCart;
        shopView.OnRemovedFromCart += OnRemoveFromCart;
        shopView.OnBuy += Buy;
        shopView.OnShopClosed += OnClose;
        shopView.OnPurchaseLicense += OnPurchaseLicense;

        subscriber.Subscribe<OnlineShopMessage>(shopTopic, OnOpen);

        //Load Licenses
        bool licensesLoaded = await AssetManager.LoadLAllLicenses();

        if (licensesLoaded)
        {
            shopView.OnLicensesLoaded(AssetManager.Licenses);
        }

        //Load Products
        bool productsLoaded = await AssetManager.LoadAllProducts();

        if (productsLoaded)
        {
            shopView.OnItemsLoaded(AssetManager.ProductInfos);
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

    void OnOpen(OnlineShopMessage message)
    {
        shopView.gameObject.SetActive(message.state);
    }

    void OnClose()
    {
        publisher.Publish(shopTopic, new OnlineShopMessage(false));
    }

    void OnPurchaseLicense(int licenseId)
    {
        if(SupermarketManager.Mine.PurchaseLicense(licenseId))
        {
            shopView.OnPurchaseLicenseSuccess(licenseId);
        }
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