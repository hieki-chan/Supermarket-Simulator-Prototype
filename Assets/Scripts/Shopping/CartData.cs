using Supermarket;
using Supermarket.Pricing;
using Supermarket.Products;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CartData
{
    public Dictionary<Company, List<CartItem>> CartItems { get => m_CartItems; private set => m_CartItems = value; }

    [SerializeField]
    private Dictionary<Company, List<CartItem>> m_CartItems = new Dictionary<Company, List<CartItem>>();

    [SerializeField, NonEditable]
    private StandardCurrency totalCost;

    public void Add(CartItem item)
    {
        ProductInfo info = item.product;

        if (m_CartItems.TryGetValue(info.Company, out List<CartItem> cartItems))
        {
            int i = cartItems.FindIndex(i => i.product == info);
            if (i != -1)
            {
                CartItem it = cartItems[i];
                it.amount += item.amount;
                cartItems[i] = it;

                //totalCost += item.product.UnitCost * item.amount;
            }
            else
            {
                cartItems.Add(item);
                //totalCost += item.product.UnitCost * item.amount;
            }
        }
        else
        {
            cartItems = new List<CartItem>
            {
                item
            };
            m_CartItems.Add(info.Company, cartItems);
        }
        totalCost += (info.UnitCost * info.UnitPerPack * item.amount);
    }

    public void Remove(Company company, int index)
    {
        if (m_CartItems.TryGetValue(company, out List<CartItem> cartItems))
        {
            ProductInfo info = cartItems[index].product;
            totalCost -= (info.UnitCost * info.UnitPerPack * cartItems[index].amount);
            cartItems.RemoveAt(index);
        }
    }

    public void Clear()
    {
        CartItems.Clear();
        totalCost = 0;
    }

    public StandardCurrency TotalCost()
    {
        //StandardCurrency cost = new StandardCurrency();
        //foreach (var item in CartItems)
        //{
        //    cost += item.product.UnitCost * item.amount;
        //}

        //return cost;

        return totalCost;
    }
}