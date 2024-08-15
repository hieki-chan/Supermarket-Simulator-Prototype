using UnityEngine;
using Supermarket;
using Supermarket.Player;
using Supermarket.Products;
using Hieki.Pubsub;
using Cysharp.Threading.Tasks;
using System.Threading;
using FurShape = Supermarket.Products.FurnitureInfo.FurnitureShape;

public class FurniturePlaceHelper : Interactable, IInteractButton01, IInteractButton02, IInteractButton03
{
    public static FurniturePlaceHelper instance { get; private set; }

    public UnitPool<string, Transform> FakeFurniturePool = new UnitPool<string, Transform>();

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

        playerTrans = FindObjectOfType<PlayerController>().transform;
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

    [SerializeField]
    private LayerMask overlapMask;

    [NonEditable, SerializeField]
    private bool isValid;

    Collider[] cols = new Collider[64];

    public Transform playerTrans;

    CancellationTokenSource tokenSource;

    private void OnDestroy()
    {
        if (tokenSource == null)
            return;

        tokenSource.Cancel();
        tokenSource.Dispose();
    }

    //------------------------------------MOVE--------------------------------\\

    private void OnMoveStart(Furniture.MoveMessage message)
    {
        currentFurniture = message.furniture;
        currentFurniture.gameObject.SetActive(false);
        currentFakeFurniture = FakeFurniturePool.GetOrCreate(currentFurniture.name, currentFurniture.FurnitureInfo.PlaceEffect.transform,
            currentFurniture.transform.position, currentFurniture.transform.rotation);

        this.Publish(PlayerTopics.interactTopic, new InteractMessage(this));
        this.Publish(PlayerTopics.castingTopic, new CastingMessage(false));

        tokenSource = new CancellationTokenSource();

        _ = OnMoving();
    }

    private async UniTaskVoid OnMoving()
    {
        int t = 0;
        while (true)
        {
            await UniTask.NextFrame(tokenSource.Token, true);
            
            Vector3 fwd = playerTrans.position + playerTrans.forward * radius;
            currentFakeFurniture.position = new Vector3(fwd.x, .014f, fwd.z);

            t++;

            if (t < 4)
                continue;

            t = 0;

            FurnitureInfo furInfo =  currentFurniture.FurnitureInfo;

            int count = furInfo.Shape switch
            {
                FurShape.Box => Physics.OverlapBoxNonAlloc(currentFakeFurniture.position, furInfo.Size / 2, cols, currentFakeFurniture.rotation, overlapMask),
                FurShape.Circle => Physics.OverlapSphereNonAlloc(currentFakeFurniture.position, furInfo.Radius, cols, overlapMask),
                _ => -1
            };

            //for (int i = 0; i < count; i++)
            //{
            //    Debug.Log(cols[i], cols[i]);
            //}
            isValid = count == 0;
        }
    }

    private bool CanPlace()
    {
        return isValid;
    }

    private void Ok()
    {
        currentFurniture.transform.SetPositionAndRotation(currentFakeFurniture.position, currentFakeFurniture.rotation);
        currentFurniture.gameObject.SetActive(true);
        FakeFurniturePool.Return(currentFurniture.name, currentFakeFurniture);

        currentFakeFurniture = null;
        currentFurniture = null;

        tokenSource.Cancel();

        this.Publish(PlayerTopics.interactTopic, new InteractMessage(null));
        this.Publish(PlayerTopics.castingTopic, new CastingMessage(true));
    }

    //-----------------------------------ROTATION-------------------------------\\

    void RotateLeft90()
    {
        currentFakeFurniture.rotation = Quaternion.Euler(0, currentFakeFurniture.eulerAngles.y + 90, 0);
    }

    void RotateRight90()
    {
        currentFakeFurniture.rotation = Quaternion.Euler(0, currentFakeFurniture.eulerAngles.y - 90, 0);
    }

    //----------------------------------------------------INTERACTING----------------------------------------------\\

    //----------------------------------- Button 01: Move/Ok -------------------------------\\
    public bool GetButtonState01()
    {
        return CanPlace();
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