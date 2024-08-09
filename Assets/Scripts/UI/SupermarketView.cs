using Supermarket.Pricing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SupermarketView : MonoBehaviour
{
    public TextMeshProUGUI moneyText;

    public TextMeshProUGUI storeLevel;
    public Image levelProgress;

    public void OnLevelUpgraded(int level)
    {
        storeLevel.text = $"level: {level}";
        levelProgress.fillAmount = 0;
    }

    public void OnLevelProgresss(float ratio)
    {
        levelProgress.fillAmount = ratio;
    }

    public void OnMoneyChange(unit amount)
    {
        moneyText.text = amount;
    }
}
