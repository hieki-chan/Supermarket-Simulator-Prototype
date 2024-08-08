using UnityEngine;
using Supermarket.Products;
using Supermarket;

public class FurniturePlacer : Interactable, IInteractButton01
{
    [NonEditable] public Furniture currentFurniture;

    public void Place()
    {

    }

    public void CanPlace()
    {

    }

    //------------------------------------MOVE--------------------------------\\

    public void Move()
    {
        /*if (player.currentInteraction == this)
        {
            MoveDone();
            return;
        }
        player.currentInteraction = this;
        transform.parent = player.transform;
        Vector3 fwd = player.transform.position + player.transform.forward * 2;
        transform.position = new Vector3(fwd.x, .014f, fwd.z);
        state = FurnitureState.Moving;*/
    }

    private void MoveDone()
    {
        /*player.currentInteraction = null;
        transform.parent = null;
        state = FurnitureState.Normal;*/
    }

    //-----------------------------------ROTATION-------------------------------\\

    void RotateLeft90()
    {
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + 90, 0);
    }

    void RotateRight90()
    {
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y - 90, 0);
    }

    //----------------------------------------------------INTERACTING----------------------------------------------\\

    //----------------------------------- Button 01: Move/Ok -------------------------------\\
    public bool GetButtonState01()
    {
        return true;
    }

    public string GetButtonTitle01()
    {
        return "Ok";
    }

    public void OnClick_Button01()
    {
        Move();
    }

    //----------------------------------- Button 02: Rotate Left 90 -------------------------------\\

    public bool GetButtonState02()
    {
        return true;
    }

    public string GetButtonTitle02()
    {
        return "Rotate Left 90";
    }

    public void OnClick_Button02()
    {
        RotateLeft90();
    }

    //-----------------------------------Button 03: Rotate Right 90 -------------------------------\\

    public bool GetButtonState03()
    {
        return true;
    }

    public string GetButtonTitle03()
    {
        return "Rotate Right 90";
    }

    public void OnClick_Button03()
    {
        RotateRight90();
    }
}
