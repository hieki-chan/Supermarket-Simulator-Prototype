using Supermarket.Products;
using System.Collections.Generic;

public struct Order
{
    public Company Company;

    public List<CartItem> CartItems;

    public float time;
}
