using Supermarket.Pricing;
using TMPro;
using UnityEngine;

public class CheckoutScreen : MonoBehaviour
{
    public TextMeshProUGUI totalCostText;
    [Header("Cash Change")]
    public GameObject cashPaymentScreen;
    public TextMeshProUGUI receivedText;
    public TextMeshProUGUI totalText;
    public TextMeshProUGUI changeText;
    public TextMeshProUGUI givingText;

    [Header("Credit Card Change")]
    public GameObject creditCardPaymentScreen;
    public TextMeshProUGUI totalPaymentText;


    public void DisplayCost(unit totalCost)
    {
        totalCostText.text = totalCost;
        creditCardPaymentScreen.SetActive(false);
        cashPaymentScreen.SetActive(false);
    }
    /// <summary>
    /// Cash Payment
    /// </summary>
    public void Display(unit received, unit total, unit change, unit giving)
    {
        receivedText.text = $"Received: {received}";
        totalText.text = $"Total: {total}";
        changeText.text = $"Change: {change}";
        givingText.text = $"Giving: {giving}";

        totalCostText.gameObject.SetActive(false);
        creditCardPaymentScreen.gameObject.SetActive(false);
        cashPaymentScreen.gameObject.SetActive(true);
    }

    public void DisplayGiving(unit giving)
    {
        givingText.text = $"Giving: {giving}";
    }

    /// <summary>
    /// Credit Card Payment
    /// </summary>
    public void Display(unit total)
    {
        totalPaymentText.text = $"Total:{total}";

        totalCostText.gameObject.SetActive(false);
        creditCardPaymentScreen.gameObject.SetActive(true);
        cashPaymentScreen.gameObject.SetActive(false);
    }
}