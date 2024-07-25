using Supermarket.Products;
using System;

[Serializable]
public struct CartItem
{
    public ProductInfo product;
    public int amount;
}