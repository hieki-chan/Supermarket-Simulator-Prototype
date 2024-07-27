using Supermarket;
using UnityEngine;

public class GarbageBin : Interactable
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<Cartons>(out var cartons) && cartons.IsEmpty())
        {
            Cartons.Pool.Return(cartons);
        }
    }
}
