using Hieki.AI;
using Supermarket.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class IsChoosingProducts : Node<Customer>
{
    public IsChoosingProducts(Customer customer) : base(customer) { }

    public override NodeState Evaluate()
    {
        throw new NotImplementedException();
    }
}
