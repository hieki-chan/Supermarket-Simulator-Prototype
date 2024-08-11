using UnityEngine;
using System.Collections.Generic;
using Supermarket.Products;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using Supermarket.Pricing;
using Supermarket;
using System.Linq;
using Hieki.Search;

public class ShopView : MonoBehaviour
{
    [Header("Products")]
    public ShopProductItem shopProductPrefab;
    public Transform itemContainer;

    private List<ShopProductItem> shopProducts;
    private List<License_UI> shopLicenses;

    [Header("Furniture")]
    //public ShopProductItem shopFurniturePrefab;
    public Transform furnitureContainer;

    [Header("License")]
    public License_UI licenseUIPrefab;
    public Transform licenseContainer;

    [Header("Search")]
    public TMP_InputField searchBar;
    public Button searchButton;
    public Button cancelButton;
    public Button closeButton;

    public UnityAction OnShopClosed;

    [Header("Cart")]
    public static MonoPool<CartItem_UI> cartPool;
    public CartItem_UI cartItemPrefab;
    public Transform cartItemContainer;
    public TextMeshProUGUI totalCostText;
    public TextMeshProUGUI totalCostText02;
    public TextMeshProUGUI cartItemCountText;
    public Button buyButton;

    List<CartItem_UI> activeCart;

    public UnityAction<CartItem> OnAddToCart { get; set; }
    public UnityAction<Company, int> OnRemovedFromCart;
    public UnityAction OnBuy;

    public void OnItemsLoaded(List<ProductInfo> productsInfo)
    {
        shopProducts = new List<ShopProductItem>(productsInfo.Count);

        foreach (ProductInfo productInfo in productsInfo)
        {
            var p = Instantiate(shopProductPrefab);
            shopProducts.Add(p);

            p.Load(productInfo);
            switch (productInfo.ProductType)
            {
                case ProductType.Products:
                    p.transform.SetParent(itemContainer, false);
                    break;
                case ProductType.Furniture:
                    p.transform.SetParent(furnitureContainer, false);
                    break;
                default:
                    break;
            }
            p.OnAddToCart.AddListener(AddToCart);
        }
    }

    public void OnLicensesLoaded(Dictionary<int, License> licenses)
    {
        shopLicenses = new List<License_UI>(licenses.Count);

        foreach (var license in licenses)
        {
            var l = Instantiate(licenseUIPrefab);
            shopLicenses.Add(l);
            l.transform.SetParent(licenseContainer, false);
            l.Load();
        }
    }

    private void Awake()
    {
        searchButton.onClick.AddListener(() => Search(searchBar.text));
        cancelButton.onClick.AddListener(() => CancelSearch());
        closeButton.onClick.AddListener(() =>
        {
            OnShopClosed?.Invoke();
            gameObject.SetActive(false);
        });

        //cart
        cartPool = new MonoPool<CartItem_UI>(cartItemPrefab);
        cartPool.Create(8, c => c.OnRemove.AddListener(OnRemoveFromCart));
        activeCart = new List<CartItem_UI>(10);

        cartItemCountText.text = "0";
        totalCostText.text = totalCostText02.text = new unit().ToString();

        buyButton.onClick.AddListener(() => OnBuy?.Invoke());
    }

    void AddToCart(CartItem cartItem)
    {
        OnAddToCart?.Invoke(cartItem);

        CartItem_UI cartUI = activeCart.FirstOrDefault(i => i.productName.text == cartItem.product.name);

        if (cartUI != null)
        {
            cartItem.amount += cartUI.amount;
            cartUI.Load(cartItem);
        }
        else
        {
            cartUI = cartPool.GetOrCreate(Vector3.zero, Quaternion.identity);
            cartUI.transform.SetParent(cartItemContainer, false);
            cartUI.Load(cartItem);
            activeCart.Add(cartUI);
        }

        cartItemCountText.text = activeCart.Count.ToString();
    }

    void OnRemoveFromCart(CartItem_UI cartItem_UI)
    {
        cartItem_UI.amount = 0;
        cartPool.Return(cartItem_UI);
        int i = activeCart.FindIndex(c => c == cartItem_UI);
        if (i == -1)
        {
            return;
        }
        activeCart.RemoveAt(i);
        cartItemCountText.text = activeCart.Count.ToString();
        OnRemovedFromCart?.Invoke(cartItem_UI.company, i);
    }

    public void DisplayTotalCost(string value)
    {
        totalCostText.text = totalCostText02.text = value;
    }

    public void OnBought()
    {
        totalCostText.text = totalCostText02.text = unit.zero.ToString();
        foreach (var c in activeCart)
        {
            cartPool.Return(c);
        }
        activeCart.Clear();
        cartItemCountText.text = activeCart.Count.ToString();
    }

    #region Search Bar

    void Search(string inputText)
    {
        if (inputText == string.Empty)
        {
            CancelSearch();
            return;
        }
        foreach (var item in shopProducts)
        {
            item.gameObject.SetActive(false);
        }
        Fuzz.Search(inputText, shopProducts, p => p.productName.text, (p, r) => p.gameObject.SetActive(true), .15f, 4);
    }

    void CancelSearch()
    {
        foreach (var item in shopProducts)
        {
            item.gameObject.SetActive(true);
        }
    }

    #endregion


    //domain reload
#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init()
    {
        cartPool = null;
    }
#endif
}