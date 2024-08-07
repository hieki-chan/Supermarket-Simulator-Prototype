using Supermarket;
using Supermarket.Products;
using UnityEngine;

public class Package : Interactable, IInteractButton01
{
    public static MonoPool<Package> Pool;

#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init()  //domain reloading
    {
        Pool = null;
    }
#endif

    [NonEditable]
    public Furniture furniture;

    public void Pack(Furniture furniture)
    {
        this.furniture = furniture;
    }

    public void UnPack()
    {
        Furniture f = Furniture.Pool.GetOrCreate(furniture.GetType(), furniture, transform.position, Quaternion.identity);
        //f.OnGet(player);
        f.OnInteractEnter();
        //f.OnInteract(player);
        furniture = null;

        //Pool.Return(this);
    }

    public bool GetButtonState01()
    {
        return furniture;
    }

    public string GetButtonTitle01()
    {
        return "UnPack";
    }

    public void OnClick_Button01()
    {
        UnPack();
    }
}