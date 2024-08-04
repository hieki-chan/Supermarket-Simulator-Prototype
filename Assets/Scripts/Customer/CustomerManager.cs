using Supermarket.Customers;
using Supermarket;
using System.Collections.Generic;
using UnityEngine;
using Hieki.Utils;

public class CustomerManager : MonoBehaviour
{
    MonoPool<Customer> CustomerPool;

    public List<Customer> CustomerPrefabs;
    public int countPerEach = 2;
    public int maxActiveCustomer = 8;

    List<Customer> activeCustomers;

    public MovementPath[] MovementPaths;

    [Header("Random Settings")]
    public float maxTime;
    public float minTime;
    [SerializeField, NonEditable]
    private float randomTime;
    [SerializeField, NonEditable]
    private float timer;


    private void Start()
    {
        MovementPaths = GetComponentsInChildren<MovementPath>();

        CustomerPool = new MonoPool<Customer>();
        for (int i = 0; i < CustomerPrefabs.Count; i++)
        {
            CustomerPool.Create(CustomerPrefabs[i], countPerEach, c =>
            {
                c.path = MovementPaths.PickOne();
                c.OnPathComplete += () =>
                {
                    CustomerPool.Return(c);
                    activeCustomers.Remove(c);
                };
            });
        }

        activeCustomers = new List<Customer>(CustomerPrefabs.Count * countPerEach);

        RandomTime();

    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= randomTime)
        {
            SpawnCustomer();
            RandomTime();
        }

        UpdateCustomers();
    }

    void UpdateCustomers()
    {
        for (int i = 0; i < activeCustomers.Count; i++)
        {
            activeCustomers[i].OnUpdate();
        }
    }

    void SpawnCustomer()
    {
        if (activeCustomers.Count >= maxActiveCustomer)
            return;

        MovementPath randomPath = MovementPaths.PickOne();
        Customer randomCustomer = CustomerPool.Get(randomPath[0], Quaternion.identity);
        if (randomCustomer)
        {
            randomCustomer.path = randomPath;
            activeCustomers.Add(randomCustomer);
        }
    }

    void RandomTime()
    {
        randomTime = Random.Range(minTime, maxTime);
        timer = 0;
    }
}