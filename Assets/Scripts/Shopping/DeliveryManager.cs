using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Supermarket;
using Supermarket.Products;

public class DeliveryManager : MonoBehaviour
{
    public Package packagePrefab;
    public Cartons cartonBoxPrefab;

    public List<Order> Orders = new List<Order>();
    bool delivering;

    private void Awake()
    {
        Cartons.Pool = new MonoPool<Cartons>(cartonBoxPrefab, 8);
        Package.Pool = new MonoPool<Package>(packagePrefab, 8);
    }

    public void Order(CartData data)
    {
        var items = data.CartItems;
        foreach (var key in items.Keys)
        {
            Orders.Add(new Order
            {
                Company = key,
                CartItems = items[key],
                time = 2,
            });
        }
        if (!delivering)
            StartCoroutine(Delivery());
    }

    WaitForSeconds waitForBox = new WaitForSeconds(0.8333f);
    public IEnumerator Delivery()
    {
        delivering = true;
        float t = 0;
        while (Orders.Count > 0)
        {
            t += Time.deltaTime;

            if (t < Orders[0].time)
            {
                yield return null;
                continue;
            }

            foreach (var item in Orders[0].CartItems)
            {
                for (int i = 0; i < item.amount; i++)
                {
                    switch (item.product.ProductType)
                    {
                        case ProductType.Products:
                            {
                                Cartons box = Cartons.Pool.GetOrCreate(Vector3.up * 5, Quaternion.identity);
                                box.PackItem(item.product.ProductOnSale, item.product.UnitPerPack);
                                break;
                            }

                        case ProductType.Furniture:
                            {
                                Package pack = Package.Pool.Get();
                                pack.Pack(item.product.Furniture);
                                break;
                            }
                        default:
                            break;
                    }

                    yield return waitForBox;
                }
            }
            Orders.RemoveAt(0);
            t = 0;

            yield return null;
        }

        delivering = false;
    }
}
