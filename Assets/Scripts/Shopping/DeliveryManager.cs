using System.Collections.Generic;
using UnityEngine;
using Supermarket;
using Supermarket.Products;
using Hieki.Pubsub;
using Cysharp.Threading.Tasks;

public class DeliveryManager : MonoBehaviour
{
    public Package packagePrefab;
    public Cartons cartonBoxPrefab;

    public List<Order> Orders = new List<Order>();
    bool delivering;

    Topic orderTopic = Topic.FromString("buy-delivery");
    ISubscriber subscriber;

    private void Awake()
    {
        Cartons.Pool = new MonoPool<Cartons>(cartonBoxPrefab, 8);
        Package.Pool = new MonoPool<Package>(packagePrefab, 8);

        subscriber = new Subscriber();
        subscriber.Subscribe<CartData>(orderTopic, (orderMessage) =>
        {
            Order(orderMessage);
        });
    }

    private void Order(CartData data)
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
            Delivery().Forget();
    }


    private async UniTaskVoid Delivery()
    {
        delivering = true;
        float t = 0;
        while (Orders.Count > 0)
        {
            t += Time.deltaTime;

            if (t < Orders[0].time)
            {
                await UniTask.NextFrame();
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
                                Cartons box = Cartons.Pool.GetOrCreate(Vector3.up * 5, Quaternion.identity, false);
                                box.PackItem(item.product.ProductOnSale, item.product.UnitPerPack);
                                await EnablePackageDelayed(box.gameObject);

                                break;
                            }

                        case ProductType.Furniture:
                            {
                                Package pack = Package.Pool.Get();
                                pack.Pack(item.product.Furniture);
                                await EnablePackageDelayed(pack.gameObject);
                                break;
                            }
                        default:
                            break;
                    }

                    //await UniTask.Delay(TimeSpan.FromSeconds(0.8333f));
                }
            }
            Orders.RemoveAt(0);
            t = 0;

            await UniTask.NextFrame();
        }

        delivering = false;
    }

    private async UniTask EnablePackageDelayed(GameObject GO)
    {
        await UniTask.Delay(833);
        GO.SetActive(true);
    }
}
