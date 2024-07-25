using Supermarket;
using Supermarket.MVP;
using Supermarket.Player;
using System;

[Serializable]
public class InteractionPresenter : Presenter<PlayerController, InteractButtonsView>
{
    public override void Initialize()
    {
        model.OnInteractWithUpdated += ShowButtons;
    }

    void ShowButtons(Interactable interaction)
    {
        view.ShowButtons(interaction);
    }
}