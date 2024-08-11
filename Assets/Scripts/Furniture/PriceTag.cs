using Supermarket;
using Supermarket.Player;
using Supermarket.Pricing;
using Supermarket.Products;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Hieki.Pubsub;

public sealed class PriceTag : Interactable
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

    public sealed override void OnInteract(Transform playerTrans, Transform cameraTrans)
    {
        ItemPricing item = SupermarketManager.Mine.GetItemPricing(productInfo);

        SupermarketManager.Mine.priceSettings.Set(item, (val) =>
        {
            this.Publish(PlayerTopics.controlTopic, new ControlStateMessage(true));

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

        this.Publish(PlayerTopics.controlTopic, new ControlStateMessage(false));
    }

#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init()  //domain reloading
    {
        ActivePriceTags = new List<PriceTag>(6);
    }
#endif
}
