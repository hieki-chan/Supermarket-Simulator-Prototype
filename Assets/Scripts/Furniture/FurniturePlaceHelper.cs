using System;
using UnityEngine;
using Supermarket;
using Supermarket.Player;
using Supermarket.Products;
using Hieki.Pubsub;
using Cysharp.Threading.Tasks;

public class FurniturePlaceHelper : Interactable, IInteractButton01
{
    public static FurniturePlaceHelper instance { get; private set; }

    public UnitPool<Type, Transform> FakeFurniturePool = new UnitPool<Type, Transform>();

    ISubscriber subscriber = new Subscriber();

    protected override void Awake()
    {
        base.Awake();
        if (instance != null)
        {
#if UNITY_EDITOR
            Debug.LogWarning("there's more than one funiture placer, there's should be one");
#endif
            return;
        }

        instance = this;

        subscriber.Subscribe<Furniture.MoveMessage>(Furniture.moveTopic, OnMoveStart);
    }

#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void DomainReload()
    {
        instance = null;
    }
#endif

    [NonEditable, SerializeField]
    private Furniture currentFurniture;

    [NonEditable, SerializeField]
    private Transform currentFakeFurniture;

    [SerializeField]
    private float radius;

    Transform playerTrans;

    public void Place()
    {

    }

    public void CanPlace()
    {

    }

    //------------------------------------MOVE--------------------------------\\

    private void OnMoveStart(Furniture.MoveMessage message)
    {
        currentFurniture = message.furniture;
        currentFurniture.gameObject.SetActive(false);
        currentFakeFurniture = FakeFurniturePool.GetOrCreate(currentFurniture.GetType(), currentFurniture.FurnitureInfo.PlaceEffect.transform,
            currentFurniture.transform.position, currentFurniture.transform.rotation);

        this.Publish(PlayerTopics.interactTopic, new InteractMessage(this));
        playerTrans = currentFurniture.playerTrans;
        _ = OnMoving();
    }

    private async UniTaskVoid OnMoving()
    {
        while (true)
        {
            await UniTask.NextFrame();

            Vector3 fwd = playerTrans.position + playerTrans.forward * 2;
            currentFakeFurniture.transform.position = new Vector3(fwd.x, .014f, fwd.z);
        }
    }

    public void Ok()
    {
        currentFurniture.transform.position = currentFakeFurniture.position;
        currentFurniture.gameObject.SetActive(true);
        FakeFurniturePool.Return(currentFurniture.GetType(), currentFakeFurniture);

        this.Publish(PlayerTopics.interactTopic, new InteractMessage(null));

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
        Ok();
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