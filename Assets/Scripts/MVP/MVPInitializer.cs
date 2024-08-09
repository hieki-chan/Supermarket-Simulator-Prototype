using UnityEngine;

[DefaultExecutionOrder(-199)]
public class MVPInitializer : MonoBehaviour
{
    public SupermarketPresenter SupermarketPresenter;

    public InteractionPresenter InteractionPresenter;

    public DeliveryPresenter DeliveryPresenter;

    public DayTimePresenter DayTimePresenter;

    public SettingsPresenter settingsPresenter;

    private void Awake()
    {
        SupermarketPresenter.Initialize();
        DeliveryPresenter.Initialize();
        InteractionPresenter.Initialize();
        DayTimePresenter.Initialize();
        settingsPresenter.Initialize();
    }

    private void OnDestroy()
    {
        settingsPresenter.OnDestroy();
    }
}
