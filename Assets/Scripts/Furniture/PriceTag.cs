using Supermarket;
using Supermarket.Player;
using Supermarket.Products;
using TMPro;

public class PriceTag : Interactable
{
    public TextMeshPro text;

    public ProductOnSale product;

    public override void OnInteract(PlayerController targetPlayer)
    {
        SupermarketManager.Mine.priceSettings.Set(product.ProductInfo, (val) =>
        {
            text.text = $"${val}";
        });
    }
}
