using Hieki.Pubsub;
using Supermarket;
using TMPro;
using UnityEngine;

public sealed class OpenClosedSignBoard : Interactable
{
    const string OPEN = "Open";
    const string CLOSED = "Closed";

    public TextMeshPro textMesh;

    bool isOpen;

    Topic buyTopic = Topic.FromMessage<CustomerBuyMessage>();

    private void Start()
    {
        isOpen = true;
        textMesh.text = isOpen ? OPEN : CLOSED;

        this.Publish(buyTopic, new CustomerBuyMessage(isOpen));
    }

    public sealed override void OnInteract(Transform playerTrans, Transform cameraTrans)
    {
        isOpen = !isOpen;
        textMesh.text = isOpen ? OPEN : CLOSED;

        this.Publish(buyTopic, new CustomerBuyMessage(isOpen));
    }
}
