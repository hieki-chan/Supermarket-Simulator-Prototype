using Supermarket.MVP;
using System;

[Serializable]
public class DeliveryPresenter : Presenter<DeliveryManager, DeliveryLogger>
{
    public override void Initialize()
    {
        model.OnArrived += () =>
        {
            view.Log();
        };
    }
}