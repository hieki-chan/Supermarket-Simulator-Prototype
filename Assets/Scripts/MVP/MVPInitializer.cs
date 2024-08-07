using UnityEngine;

[DefaultExecutionOrder(-10)]
public class MVPInitializer : MonoBehaviour
{
    public InteractionPresenter InteractionPresenter;

    public DayTimePresenter DayTimePresenter;

    public SettingsPresenter settingsPresenter;

    private void Awake()
    {
        InteractionPresenter.Initialize();
        DayTimePresenter.Initialize();
        settingsPresenter.Initialize();
    }

    private void OnDestroy()
    {
        settingsPresenter.OnDestroy();
    }
}
