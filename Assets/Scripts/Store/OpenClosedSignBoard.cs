using Supermarket;
using Supermarket.Player;
using TMPro;

public sealed class OpenClosedSignBoard : Interactable
{
    const string OPEN = "Open";
    const string CLOSED = "Closed";

    public TextMeshPro textMesh;

    bool isOpen;
    public sealed override void OnInteract(PlayerController targetPlayer)
    {
        isOpen = !isOpen;
        textMesh.text = isOpen ? OPEN : CLOSED;
    }
}
