using Hieki.Pubsub;
using Hieki.Utils;
using Supermarket;
using Supermarket.Customers;
using Supermarket.Player;
using Supermarket.Pricing;
using Supermarket.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class CheckoutDesk : Interactable, IInteractButton01
{
    UnitPool<float, Transform> MoneyPool;

    [SerializeField] private Vector3 seatOffset;
    [SerializeField] private Vector3 lookEuler;

    [SerializeField] private Vector3 productPos;
    [SerializeField] private Vector3 moneyChangePosFrom;
    [SerializeField] private Vector3 moneyChangePosTo;

    public List<CheckoutPoint> CheckoutLine => checkoutLine;

    [Header("Line"), Space]
    [SerializeField] private List<CheckoutPoint> checkoutLine;

    [Header("Packing"), Space]
    [SerializeField] private Vector3 packingPos;
    [SerializeField] private Vector3 packOffset;
    [SerializeField, NonEditable] private int packedCout;

    public Action OnPackedDone;

    [Header("Money"), Space]
    [NonEditable, SerializeField] private float totalCost;
    [SerializeField] private float moneyGivingTime = .25f;
    [SerializeField] private float yStepMove = 0.001f;
    [SerializeField, NonEditable] float currentMoneyY = 0;
    [SerializeField] private float range;
    const int poolSize = 6;
    public GameObject bankote1;
    public GameObject bankote5;
    public GameObject bankote10;
    public GameObject bankote20;
    public GameObject bankote50;
    [Space]
    public GameObject coin0_01;
    public GameObject coin0_05;
    public GameObject coin0_10;
    public GameObject coin0_20;
    public GameObject coin0_50;

    [Header("Screen")]
    public CheckoutScreen screen;

    PaymentObject currentPayment;

    List<MoneyPoolable> activeMoney;


    struct MoneyPoolable
    {
        public float keyValue;
        public Transform trans;
    }

    Transform playerTrans;

    protected override void Awake()
    {
        base.Awake();
        MoneyPool = new UnitPool<float, Transform>(3);

        MoneyPool.Create(1, GetMoneyPrefab(1), poolSize);
        MoneyPool.Create(5, GetMoneyPrefab(5), poolSize);
        MoneyPool.Create(10, GetMoneyPrefab(10), poolSize);
        MoneyPool.Create(20, GetMoneyPrefab(20), poolSize);
        MoneyPool.Create(50, GetMoneyPrefab(50), poolSize);

        MoneyPool.Create(0.01f, GetMoneyPrefab(0.01f), poolSize);
        MoneyPool.Create(0.02f, GetMoneyPrefab(0.05f), poolSize);
        MoneyPool.Create(0.10f, GetMoneyPrefab(0.10f), poolSize);
        MoneyPool.Create(0.20f, GetMoneyPrefab(0.20f), poolSize);
        MoneyPool.Create(0.50f, GetMoneyPrefab(0.50f), poolSize);

        activeMoney = new List<MoneyPoolable>(24);
        currentMoneyY = Position.Offset(transform, moneyChangePosTo).y;
    }

    public override void OnInteract(Transform playerTrans, Transform cameraTrans)
    {
        this.playerTrans = playerTrans;
        this.Publish(PlayerTopics.charCtrllerTopic, new CharCtrllerStateMessage(false));

        playerTrans.LeanMove(Offset(seatOffset), .35f);
        cameraTrans.LeanRotate(lookEuler, .35f);

        this.Publish(PlayerTopics.controlTopic, new ControlStateMessage(false));
        this.Publish(PlayerTopics.interactTopic, new InteractMessage(this));
    }

    public override void OnInteractOther(Interactable other)
    {
        if (other is ProductOnSale product)
        {
            product.transform.LeanMove(Offset(productPos), .3f).setOnComplete(() =>
            {
                ProductOnSale.Pool.Return(product.ProductInfo.name, product);
            });

            packedCout--;
            ItemPricing price = SupermarketManager.Mine.GetItemPricing(product.ProductInfo);
            totalCost += price.price;
            screen.DisplayCost(totalCost);

            if (packedCout == 0)
            {
                OnPackedDone?.Invoke();
                OnPackedDone = null;
            }

            return;
        }

        NotifyPayment(other);
    }

    private void NotifyPayment(Interactable other)
    {
        if (other is PaymentObject obj)
        {
            currentPayment = obj;
            currentPayment.SetValue(totalCost);
            Customer.PaymentObjectPool.Return(obj.PaymentType, obj);

            switch (obj.PaymentType)
            {
                case PaymentType.CreditCard:
                    screen.Display(totalCost);
                    this.Publish(CardPayTopic, new CardPayNotify(currentPayment.value, OnPayCorrect, OnPayIncorrect));
                    break;
                case PaymentType.Cash:
                    screen.Display(obj.value, totalCost, obj.value - totalCost, unit.zero);
                    this.Publish(CashPayTopic, new CashPayNotify(obj.value - totalCost, GetMoney, OnResetMoney, OnGiveCorrect, OnPayIncorrect));
                    break;
                default:
                    break;
            }
        }
    }

    public void LeaveCheckkout()
    {
        this.Publish(PlayerTopics.charCtrllerTopic, new CharCtrllerStateMessage(true));

        playerTrans = null;

        this.Publish(PlayerTopics.controlTopic, new ControlStateMessage(true));
        this.Publish(PlayerTopics.interactTopic, new InteractMessage(null));
    }

    public (CheckoutPoint point, int index) GetEmptyPoint()
    {
        int count = checkoutLine.Count;
        for (int i = 0; i < count; i++)
        {
            CheckoutPoint point = checkoutLine[i];

            if (!point.isTaked && (i + 1 >= count || (i + 1 < count && !checkoutLine[i + 1].isTaked)))
            {
                return (point, i);
            }
        }

        return (null, -1);
    }

    //----------------------------------BUTTONS------------------------------------------\\

    public bool GetButtonState01()
    {
        return playerTrans;
    }

    public string GetButtonTitle01()
    {
        return "Leave Checkout";
    }

    public void OnClick_Button01()
    {
        LeaveCheckkout();
    }


    //--------------------------------Money Changing--------------------------------------\\
    public void ProductPacking(List<ProductOnSale> products)
    {
        var sortedProducts = products.OrderByDescending(p => p.GetWorldSize().magnitude).ToArray();

        Vector3 pos = Vector3.zero;
        for (int i = 0; i < sortedProducts.Length; i++)
        {
            ProductOnSale product = sortedProducts[i];

            product.EnableInteracttion();
            product.transform.parent = null;

            Vector2 size = product.GetWorldSize();
            if (i % 3 == 0)
            {
                pos.x += size.x;
            }

            pos += new Vector3(packOffset.x, 0, packOffset.y * (i / 3) * size.y);
            Vector3 worldPos = Offset(packingPos) + pos;

            product.transform.LeanMove(worldPos, .25f);
            product.transform.rotation = Quaternion.identity;
        }

        packedCout = products.Count;
    }

    private void GetMoney(float val, float totalGiving)
    {
        Vector3 from = Offset(moneyChangePosFrom);
        Vector3 to = Offset(moneyChangePosTo + new Vector3(UnityEngine.Random.Range(-range, range), 0, UnityEngine.Random.Range(-range, range)));
        to.y = currentMoneyY;
        currentMoneyY += yStepMove;

        var money = MoneyPool.Get(val, from, transform.rotation);
        if (!money)
        {
            MoneyPool.Create(val, GetMoneyPrefab(val), poolSize);
            money = MoneyPool.Get(val, from, transform.rotation);
        }

        money.LeanMove(to, moneyGivingTime);
        money.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360f), 0);
        activeMoney.Add(new MoneyPoolable()
        {
            keyValue = val,
            trans = money,
        });

        screen.DisplayGiving(totalGiving);
    }

    private void OnResetMoney()
    {
        ResetMoney(true);
        screen.DisplayGiving(unit.zero);
    }

    private void OnGiveCorrect()
    {
        ResetMoney(false);
        OnPayCorrect();
    }

    private void ResetMoney(bool playAni)
    {
        Vector3 from = Position.Offset(transform, moneyChangePosFrom);

        foreach (var m in activeMoney)
        {
            if (playAni)
                m.trans.LeanMove(from, moneyGivingTime).setOnComplete(() =>
                {
                    MoneyPool.Return(m.keyValue, m.trans);
                });
            else
                MoneyPool.Return(m.keyValue, m.trans);
        }

        currentMoneyY = Position.Offset(transform, moneyChangePosTo).y;
        activeMoney.Clear();
    }

    void OnPayCorrect()
    {
        currentPayment.OnPayCorrect();
        currentPayment = null;
        checkoutLine[0].isTaked = false;
        SupermarketManager.Mine.Store(totalCost);
        totalCost = 0;
    }

    void OnPayIncorrect()
    {

    }

    private Transform GetMoneyPrefab(float val)
    {
        return val switch
        {
            1 => bankote1.transform,
            5 => bankote5.transform,
            10 => bankote10.transform,
            20 => bankote20.transform,
            50 => bankote50.transform,

            0.01f => coin0_01.transform,
            0.05f => coin0_05.transform,
            0.10f => coin0_10.transform,
            0.20f => coin0_20.transform,
            0.50f => coin0_50.transform,
            _ => null
        };
    }

    Vector3 Offset(Vector3 offset) => Position.Offset(transform, offset);

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector3 seatPos = Offset(seatOffset);
        Quaternion rot = Quaternion.Euler(lookEuler);
        SceneDrawer.DrawWireCubeGizmo(seatPos, Vector3.one * .5f, rot, Color.green);
        SceneDrawer.ArrowGizmo(seatPos, rot * Vector3.forward);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Offset(productPos), .25f);


        UnityEditor.Handles.color = Gizmos.color = Color.blue;
        Vector3 from = Offset(moneyChangePosFrom);
        Vector3 to = Offset(moneyChangePosTo);
        UnityEditor.Handles.DrawWireDisc(from, Vector3.up, range);
        UnityEditor.Handles.DrawWireDisc(to, Vector3.up, range);
        SceneDrawer.ArrowGizmo(from, (to - from), (to - from).magnitude / 5);

        // SceneDrawer.DrawWireCubeHandles(Offset(packingPos), new Vector3(areaPacker.sizeX, 0, areaPacker.sizeY), transform.rotation, Color.white, 4);
    }
#endif

    [Serializable]
    public class CheckoutPoint
    {
        public Vector3 position;
        public float rotateY;
        public bool isTaked;
    }

    //--------------------------------------------------PubSub Events-----------------------------------------------\\
    public static Topic CardPayTopic => cardPayTopic;

    public static Topic CashPayTopic => cashPayTopic;


    private static readonly Topic cardPayTopic = Topic.FromMessage<CardPayNotify>();
    private static readonly Topic cashPayTopic = Topic.FromMessage<CashPayNotify>();


    public readonly struct CardPayNotify : IMessage
    {
        public readonly float value;
        public readonly Action OnCorrect;
        public readonly Action OnIncorrect;

        public CardPayNotify(float value, Action onCorrect, Action onIncorrect)
        {
            this.value = value;
            OnCorrect = onCorrect;
            OnIncorrect = onIncorrect;
        }
    }

    public readonly struct CashPayNotify : IMessage
    {
        public readonly float value;
        public readonly Action<float, float> OnClick;
        public readonly Action OnReset; 
        public readonly Action OnCorrect; 
        public readonly Action OnIncorrect;

        public CashPayNotify(float value, Action<float, float> onClick, Action onReset, Action onCorrect, Action onIncorrect)
        {
            this.value = value;
            OnClick = onClick;
            OnReset = onReset;
            OnCorrect = onCorrect;
            OnIncorrect = onIncorrect;
        }
    }
}

