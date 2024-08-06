using UnityEngine;

[DefaultExecutionOrder(-10)]
public class MVPInitializer : MonoBehaviour
{
    public InteractionPresenter InteractionPresenter;

    public DayTimePresenter DayTimePresenter;

    private void Awake()
    {
        InteractionPresenter.Initialize();
        DayTimePresenter.Initialize();
    }
}
