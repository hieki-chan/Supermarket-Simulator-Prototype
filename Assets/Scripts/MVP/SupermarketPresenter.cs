using Supermarket.MVP;
using Supermarket.Pricing;
using System;

[Serializable]
public class SupermarketPresenter : Presenter<SupermarketManager, SupermarketView>
{
    public override void Initialize()
    {
        model.OnMoneyChanged += OnMoneyChanged;
        model.OnLevelUpgraded += OnLevelUpgraded;
        model.OnLevelInProgress += OnLevelProgress;
    }

    void OnMoneyChanged(unit unit)
    {
        view.OnMoneyChange(unit);
    }

    void OnLevelUpgraded(int level)
    {
        view.OnLevelUpgraded(level);
    }

    void OnLevelProgress(float progress)
    {
        view.OnLevelProgresss(progress);
    }
}