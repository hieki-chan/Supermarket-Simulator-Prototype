using Supermarket;
using Supermarket.Player;
using TMPro;

public class OpenClosedSignBoard : Interactable
{
    const string OPEN = "Open";
    const string CLOSED = "Closed";

    public TextMeshPro textMesh;

    bool isOpen;
    public override void OnInteract(PlayerController targetPlayer)
    {
        isOpen = !isOpen;
        textMesh.text = isOpen ? OPEN : CLOSED;
    }
}
