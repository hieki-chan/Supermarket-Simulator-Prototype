using Supermarket;
using Supermarket.Player;
using Supermarket.Customers;
using Supermarket.Products;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;

public class Cartons : Interactable, IInteractButton01, IInteractButton02, IInteractButton03
{
    public static MonoPool<Cartons> Pool;
#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init()  //domain reloading
    {
        Pool = null;
    }
#endif

    public ArrangementGrid grid;

    [Header("Pick Up")]
    [SerializeField] 
    private Vector3 handHoldPos;
    [SerializeField, NonEditable] 
    private bool pickedUp;

    private const float PICK_UP_SPEED = 2;
    private const float THROW_FORCE = 500;
    private const float THROW_FORCE_UP = 500;

    [Header("Open")]
    public Transform foldingLeft;
    public Transform foldingRight;
    [NonEditable, SerializeField]
    private bool opened;

    private CancellationTokenSource tokenSource;

    Rigidbody rb;
    Collider col;

    Storage storage;
    Transform tier;
    PlayerController player;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        tokenSource = new CancellationTokenSource();
    }

    //private void OnDisable()
    //{
    //    tokenSource.Cancel();
    //}

    private void OnDestroy()
    {
        tokenSource.Cancel();
        tokenSource.Dispose();
    }

    public override void OnInteract(PlayerController targetPlayer)
    {
        player = targetPlayer;
        player.currentInteraction = this;
        PickUp(targetPlayer.cameraTrans);
    }

    public override void OnHoverOther(Interactable other)
    {
        if (other is not Storage storage)
        {
            return;
        }
        if ((transform.position - storage.transform.position).sqrMagnitude >= 4)
            return;
        this.storage = storage;
    }

    public override void OnHoverOther(Collider collider)
    {
        tier = collider.transform;
    }

    public override void OnHoverOtherExit()
    {
        storage = null;
    }

    public override void OnInteractExit()
    {
        col.enabled = true;
        rb.isKinematic = false;
    }

    private void PickUp(Transform target)
    {
        transform.parent = target;
        transform.localPosition = handHoldPos;
        transform.localRotation = Quaternion.identity;

        col.enabled = false;
        rb.isKinematic = true;
        pickedUp = true;
    }

    public void Throw()
    {
        transform.parent = null;
        col.enabled = true;
        rb.isKinematic = false;
        rb.AddRelativeForce(Vector3.forward * (THROW_FORCE * Time.deltaTime), ForceMode.Impulse);
        rb.AddRelativeForce(Vector3.up * (THROW_FORCE_UP * Time.deltaTime), ForceMode.Impulse);

        player.currentInteraction = null;
        pickedUp = false;
    }

    public async UniTaskVoid OpenAsync()
    {
        await UniTask.WhenAll(
            foldingLeft.DOLocalRotate(new Vector3(0, 0, +150), .25f).WithCancellation(tokenSource.Token),
            foldingRight.DOLocalRotate(new Vector3(0, 0, -150), .25f).WithCancellation(tokenSource.Token)
            );

        opened = true;
    }

    public async UniTaskVoid CloseAsync()
    {
        opened = false;

        await UniTask.WhenAll(
            foldingLeft.DOLocalRotate(new Vector3(0, 0, 0), .25f).WithCancellation(tokenSource.Token),
            foldingRight.DOLocalRotate(new Vector3(0, 0, 0), .25f).WithCancellation(tokenSource.Token)
            );
    }

    public void PlaceItem()
    {
        ProductOnSale item = GetItem();

        if (storage.TryArrangeProduct(item, tier))
        {
            GetItemOut();
        }
    }

    public void PackItem(ProductOnSale product, int count)
    {
        for (int i = 0; i < count; i++)
        {
            ProductOnSale p = ProductOnSale.Pool.GetOrCreate(product.name, product, transform.position, Quaternion.identity);

            grid.Push(p);
        }
    }


    public ProductOnSale GetItem()
    {
        return grid.Peek();
    }

    public ProductOnSale GetItemOut()
    {
        return grid.Pop();
    }

    public bool IsEmpty()
    {
        return grid.Count == 0;
    }


    //BUTTONS

    //--------------------------------------Button 01 - Open/Close------------------------------------------\\
    public string GetButtonTitle01()
    {
        return !opened ? "Open" : "Close";
    }
    public bool GetButtonState01()
    {
        return pickedUp;
    }
    public void OnClick_Button01()
    {
        if (!opened)
            OpenAsync().Forget();
        else
            CloseAsync().Forget();
    }

    //--------------------------------------Button 02 - Place------------------------------------------\\
    public string GetButtonTitle02()
    {
        return "Place";
    }
    public bool GetButtonState02()
    {
        return opened && storage && grid.Count > 0;
    }
    public void OnClick_Button02()
    {
        PlaceItem();
    }

    //--------------------------------------Button 02 - Throw------------------------------------------\\
    public string GetButtonTitle03()
    {
        return "Throw";
    }
    public bool GetButtonState03()
    {
        return pickedUp;
    }
    public void OnClick_Button03()
    {
        Throw();
    }


    private void OnDrawGizmosSelected()
    {
        grid.OnDrawGizmo();

        Gizmos.DrawWireSphere(transform.position + handHoldPos, .5f);
    }
}
