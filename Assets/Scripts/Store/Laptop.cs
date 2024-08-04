using Supermarket;
using Supermarket.Player;
using UnityEngine.Events;

public sealed class Laptop : Interactable
{
    public UnityAction OnOpenShop;

    PlayerController player;

    public override void OnInteract(PlayerController targetPlayer)
    {
        player = targetPlayer;
        player.disableLook = true;
        player.disableMove = true;
        player.currentInteraction = this;

        OnOpenShop?.Invoke();
    }

    public void OnShopClosed()
    {
        if (player == null)
            return;
        player.disableLook = false;
        player.disableMove = false;
        player.currentInteraction = null;
    }
}
