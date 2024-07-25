using UnityEngine;

[DefaultExecutionOrder(-10)]
public class MVPInitializer : MonoBehaviour
{
    public InteractionPresenter InteractionPresenter;

    private void Awake()
    {
        InteractionPresenter.Initialize();
    }
}
