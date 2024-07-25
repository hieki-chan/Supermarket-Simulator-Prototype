using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Supermarket;

public class InteractButtonsView : MonoBehaviour
{
    public Button button01;
    public Button button02;
    public Button button03;
    public Button button04;
    public Button button05;

    public TextMeshProUGUI text01;
    public TextMeshProUGUI text02;
    public TextMeshProUGUI text03;
    public TextMeshProUGUI text04;
    public TextMeshProUGUI text05;

    public IInteractButton01 interact01;
    public IInteractButton02 interact02;
    public IInteractButton03 interact03;
    public IInteractButton04 interact04;
    public IInteractButton05 interact05;

    private void Start()
    {
        button01.onClick.AddListener(OnClick_Button01);
        button02.onClick.AddListener(OnClick_Button02);
        button03.onClick.AddListener(OnClick_Button03);
        button04.onClick.AddListener(OnClick_Button04);
        button05.onClick.AddListener(OnClick_Button05);
        DisbableButtons();
    }

    private void DisbableButtons()
    {
        button01.gameObject.SetActive(false);
        button02.gameObject.SetActive(false);
        button03.gameObject.SetActive(false);
        button04.gameObject.SetActive(false);
        button05.gameObject.SetActive(false);
    }

    public void ShowButtons(Interactable interaction)
    {
        if(interaction == null)
        {
            DisbableButtons();
            return;
        }


        interact01 = interaction as IInteractButton01;
        if (interact01 != null)
        {
            text01.text = interact01.GetButtonTitle01();
            button01.gameObject.SetActive(interact01.GetButtonState01());
        }

        interact02 = interaction as IInteractButton02;
        if (interact02 != null)
        {
            text02.text = interact02.GetButtonTitle02();
            button02.gameObject.SetActive(interact02.GetButtonState02());
        }

        interact03 = interaction as IInteractButton03;
        if (interact03 != null)
        {
            text03.text = interact03.GetButtonTitle03();
            button03.gameObject.SetActive(interact03.GetButtonState03());
        }

        interact04 = interaction as IInteractButton04;
        if (interact04 != null)
        {
            text04.text = interact04.GetButtonTitle04();
            button04.gameObject.SetActive(interact04.GetButtonState04());
        }

        interact05 = interaction as IInteractButton05;
        if (interact05 != null)
        {
            text05.text = interact05.GetButtonTitle05();
            button05.gameObject.SetActive(interact05.GetButtonState05());
        }
    }

    private void OnClick_Button01()
    {
        interact01?.OnClick_Button01();
    }
    private void OnClick_Button02()
    {
        interact02?.OnClick_Button02();
    }
    private void OnClick_Button03()
    {
        interact03?.OnClick_Button03();
    }
    private void OnClick_Button04()
    {
        interact04?.OnClick_Button04();
    }
    private void OnClick_Button05()
    {
        interact05?.OnClick_Button05();
    }
}