using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Hieki.Utils;
using Cysharp.Threading.Tasks;

[DefaultExecutionOrder(505)]
public class TabGroup : MonoBehaviour
{
    public TabEvent onChangeTab => m_onChangeTab;
    public List<IndexedButton> IndexedBttns { get; private set; } = new List<IndexedButton>();

    [SerializeField] private Transform buttonsContainer;
    [SerializeField] private List<GameObject> tabs = new List<GameObject>();
    public int defaultTab = 0;
    private int selected;
    public bool coloring;
    public Color normalColor = Color.grey;
    public Color selectedColor = Color.white;

    public Transform underLine;
    public Vector3 offset = new Vector3(0, -20, 0);
    public float duration;

    [Space]
    [SerializeField] public TabEvent m_onChangeTab;

    async void Awake()
    {
        selected = defaultTab;
        int i = 0;
        buttonsContainer.ForeachChilds<IndexedButton>(b =>
        {
            IndexedBttns.Add(b);
            IndexedBttns[i].index = i;
            IndexedBttns[i].OnSelected.AddListener(OnSelected);
            i++;
        });

        for (i = 0; i < tabs.Count; i++)
        {
            tabs[i].SetActive(i == selected);
        }

        await UniTask.Yield();

        if(underLine)
            underLine.transform.position = IndexedBttns[0].transform.position + offset;
    }

    void OnSelected(int index)
    {
        if (selected < tabs.Count && selected > -1)
            tabs[selected].SetActive(false);
        selected = index;

        if (coloring)
        {
            for (int i = 0; i < IndexedBttns.Count; i++)
            {
                IndexedBttns[i].image.color = normalColor;
            }
            IndexedBttns[selected].image.color = selectedColor;
        }
        onChangeTab?.Invoke(selected);

        if (selected < tabs.Count && selected > -1)
            tabs[selected].SetActive(true);
        if(underLine)
            underLine.DOMove(IndexedBttns[selected].transform.position + offset, duration);
    }


    [System.Serializable]
    public class TabEvent : UnityEvent<int> { }
}