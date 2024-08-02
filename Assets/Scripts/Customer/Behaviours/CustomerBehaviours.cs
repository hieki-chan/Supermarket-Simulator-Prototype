using Hieki.AI;
using Supermarket.Customers;
using System.Collections.Generic;

public class CustomerBehaviours : BehaviourTree<Customer>
{
    public override void InitTree(Customer data)
    {
        root = new Root(new Selector(new List<Node>()
        {
            new Sequence(new List<Node>()   //the customer is going shopping
            {
                new IsGoingShopping(data),
                new FindAvailableStorage(data),

                new Selector(new List<Node>()
                {
                    new Sequence(new List<Node>()
                    {
                        new IsChoosingProducts(data),
                        new ChoosingProducts(data),
                    }),

                    new Sequence(new List<Node>()
                    {
                        new IsReachedStorage(data).Invert(),    //if not reached storage
                        new HeadingToStorage(data),
                    }),
                }),
            }),


            new Sequence(new List<Node>()   //if the customer reached target position, move to next target node
            {
                new IsReachedPathNode(data),
                new NextPathNode(data),
                new OnPathCompleted(data),
            }),

            new WalkingNode(data),
        }));
    }
}
