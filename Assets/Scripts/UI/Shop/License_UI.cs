using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class License_UI : MonoBehaviour
{
    public TextMeshProUGUI header;
    public TextMeshProUGUI description;
    public TextMeshProUGUI allows;
    public TextMeshProUGUI requires;
    public TextMeshProUGUI cost;

    public Button purchaseBttn;
    public GameObject owned;


    private void Awake()
    {
        purchaseBttn.onClick.AddListener(OnPurchase);
    }

    public void Load()
    {

    }

    private void OnPurchase()
    {

    }
}