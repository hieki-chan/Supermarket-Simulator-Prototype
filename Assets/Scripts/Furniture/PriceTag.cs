using Supermarket;
using Supermarket.Player;
using Supermarket.Pricing;
using Supermarket.Products;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PriceTag : Interactable
{
    public static List<PriceTag> ActivePriceTags = new List<PriceTag>(6);

    public TextMeshPro text;

    public ProductInfo productInfo 
    {
        get => m_productInfo; 
        set 
        { 
            m_productInfo = value;
            ItemPricing item = SupermarketManager.Mine.GetItemPricing(productInfo);
            text.text = item.price;
        }
    }
    [SerializeField, NonEditable] private ProductInfo m_productInfo;

    public override void OnInteract(PlayerController targetPlayer)
    {
        ItemPricing item = SupermarketManager.Mine.GetItemPricing(productInfo);

        SupermarketManager.Mine.priceSettings.Set(item, (val) =>
        {
            targetPlayer.disableLook = false;
            targetPlayer.disableMove = false;
            if (val == item.price)
                return;
            //text.text = val;
            item.price = val;

            foreach (var tag in ActivePriceTags)
            {
                if(tag.productInfo == m_productInfo)
                    tag.text.text = val;
            }
        });

        targetPlayer.disableLook = true;
        targetPlayer.disableMove = true;
    }

#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init()  //domain reloading
    {
        ActivePriceTags = new List<PriceTag>(6);
    }
#endif
}
