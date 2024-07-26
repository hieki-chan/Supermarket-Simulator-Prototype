using Supermarket;
using Supermarket.MVP;
using Supermarket.Player;
using System;

[Serializable]
public class InteractionPresenter : Presenter<PlayerController, InteractButtonsView>
{
    public TooltipView tooltips;

    public override void Initialize()
    {
        model.OnInteractWithUpdated += OnInteracted;
        model.OnHoverEntered += OnHoverEntered;
        model.OnHoverExited += OnHoverExited;
    }

    void OnInteracted(Interactable interaction)
    {
        view.ShowButtons(interaction);
    }

    void OnHoverEntered(Interactable interaction)
    {
        if (interaction && interaction.interactionInfo)
        {
            string tooltip = interaction.interactionInfo.Tooltips;

            if (!string.IsNullOrEmpty(tooltip))
            {
                tooltips.Show(tooltip);
            }
        }
        OnInteracted(interaction);
    }
    void OnHoverExited(Interactable interaction)
    {
        tooltips.Hide();
        OnInteracted(null);
    }
}