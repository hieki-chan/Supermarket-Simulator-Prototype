using Supermarket;
using Supermarket.Player;
using Supermarket.Customers;
using Supermarket.Products;
using UnityEngine;

public class Cartons : Interactable, IInteractButton01, IInteractButton02
{
    public static SimplePool<Cartons> Pool;
#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init()  //domain reloading
    {
        Pool = null;
    }
#endif

    public Furniture furniture;

    public ArrangementGrid grid;

    [Header("Pick Up")]
    [SerializeField] private Vector3 handHoldPos;


    private const float PICK_UP_SPEED = 2;
    private const float THROW_FORCE = 500;
    private const float THROW_FORCE_UP = 500;

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
    }

    private void OnEnable()
    {
        furniture = null;
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
    }

    public void Throw()
    {
        transform.parent = null;
        col.enabled = true;
        rb.isKinematic = false;
        rb.AddRelativeForce(Vector3.forward * (THROW_FORCE * Time.deltaTime), ForceMode.Impulse);
        rb.AddRelativeForce(Vector3.up * (THROW_FORCE_UP * Time.deltaTime), ForceMode.Impulse);

        player.currentInteraction = null;
    }

    public void PlaceItem()
    {
        ProductOnSale item = GetItem();

        if(storage.TryArrangeProduct(item, tier))
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

    public void PackFurniture(Furniture furniture)
    {
        this.furniture = furniture;
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
        return grid.Count == 0 && furniture == null;
    }


    //BUTTONS

    public string GetButtonTitle01()
    {
        return "Place";
    }
    public bool GetButtonState01()
    {
        return storage && grid.Count > 0 || furniture != null;
    }
    public void OnClick_Button01()
    {
        if(!furniture)
            PlaceItem();
        else
        {
            Furniture f = Furniture.Pool.GetOrCreate(furniture.GetType(), furniture, transform.position, Quaternion.identity);
            f.OnGet(player);
            f.OnInteractEnter();
            f.OnInteract(player);
            furniture = null;
            Throw();
        }
    }


    public string GetButtonTitle02()
    {
        return "Throw";
    }
    public bool GetButtonState02()
    {
        return true;
    }
    public void OnClick_Button02()
    {
        Throw();
    }


    private void OnDrawGizmosSelected()
    {
        grid.OnDrawGizmo();

        Gizmos.DrawWireSphere(transform.position + handHoldPos, .5f);
    }
}
