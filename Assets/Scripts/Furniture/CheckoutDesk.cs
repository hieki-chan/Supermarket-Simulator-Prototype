using Hieki.Utils;
using Supermarket;
using Supermarket.Customers;
using Supermarket.Player;
using Supermarket.Products;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public sealed class CheckoutDesk : Interactable, IInteractButton01
{
    Pool<float, Transform> MoneyPool;

    [SerializeField] private Vector3 seatOffset;
    [SerializeField] private Vector3 lookEuler;

    [SerializeField] private Vector3 productPos;
    [SerializeField] private Vector3 moneyChangePosFrom;
    [SerializeField] private Vector3 moneyChangePosTo;

    public List<CheckoutPoint> CheckoutLine => checkoutLine;
    [Header("Line"), Space]
    [SerializeField] private List<CheckoutPoint> checkoutLine;

    [Header("Packing"), Space]
    [SerializeField] private AreaPacking2D areaPacker;
    [SerializeField] private Vector3 packingPos;
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
    public TextMeshPro screenText;

    public PaymentTerminal paymentTerminal;
    public MoneyChanger moneyChangePannel;

    PaymentObject currentPayment;

    List<MoneyPoolable> activeMoney;


    struct MoneyPoolable
    {
        public float keyValue;
        public Transform trans;
    }

    PlayerController player;

    protected override void Awake()
    {
        base.Awake();
        MoneyPool = new Pool<float, Transform>(3);

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

        areaPacker.StartArea();
    }

    public override void OnInteract(PlayerController targetPlayer)
    {
        player = targetPlayer;
        player.disableMove = true;
        player.disableLook = true;
        player.m_Controller.enabled = false;

        player.transform.LeanMove(Offset(seatOffset), .35f);
        player.cameraTrans.LeanRotate(lookEuler, .35f);
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
            totalCost += product.ProductInfo.UnitCost;
            screenText.text = $"Total: {totalCost}";

            if (packedCout == 0)
            {
                areaPacker.StartArea();
                OnPackedDone?.Invoke();
            }

            return;
        }

        if (other is PaymentObject obj)
        {
            currentPayment = obj;
            Customer.PaymentObjectPool.Return(obj.PaymentType, obj);

            screenText.text = obj.PaymentType switch
            {
                PaymentType.Cash => $"Cash Payment \nReceived:{obj.value} \nTotal:{totalCost} \nChange:{obj.value - totalCost} \nGiving:{0}",
                PaymentType.CreditCard => $"Card Payment \nTotal:{totalCost}",
                _ => ""
            };


            switch (obj.PaymentType)
            {
                case PaymentType.CreditCard:
                    currentPayment.value = totalCost;
                    paymentTerminal.Check(obj.value, OnPayCorrect, OnPayIncorrect);
                    break;
                case PaymentType.Cash:
                    moneyChangePannel.Check(GetMoney, OnResetMoney, OnOK);
                    break;
                default:
                    break;
            }
        }
    }

    public void LeaveCheckkout()
    {
        player.m_Controller.enabled = true;
        player.disableMove = false;
        player.disableLook = false;

        player = null;

        InteractExit();
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

    //BUTTONS

    public bool GetButtonState01()
    {
        return player;
    }

    public string GetButtonTitle01()
    {
        return "Leave Checkout";
    }

    public void OnClick_Button01()
    {
        LeaveCheckkout();
    }


    //Money Change 
    public void ProductPackingPos(ProductOnSale product)
    {
        Vector3 size = product.GetWorldSize();
        AreaPacking2D.Item item = new AreaPacking2D.Item()
        {
            sizeX = size.x,
            sizeY = size.y,
            spacing = (size.x + size.y) / 3,
        };
        areaPacker.Pack(item, out Vector2 result);

        Vector3 worldPos = Offset(packingPos) + new Vector3(result.x, 0, result.y);

        product.transform.LeanMove(worldPos, .25f);
        product.transform.rotation = Quaternion.identity;

        packedCout++;
    }

    public void GetMoney(float val)
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
    }

    public void OnResetMoney()
    {
        ResetMoney(true);
    }

    public bool OnOK(float val)
    {
        if (Mathf.Abs(currentPayment.value - totalCost - val) < 0.01f)
        {
            ResetMoney(false);
            OnPayCorrect();
            return true;
        }
        OnPayIncorrect();
        return false;
    }

    public void ResetMoney(bool playAni)
    {
        Vector3 from = Position.Offset(transform, moneyChangePosFrom);

        foreach (var m in activeMoney)
        {
            if(playAni)
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

        SceneDrawer.DrawWireCubeHandles(Offset(packingPos), new Vector3(areaPacker.sizeX, 0, areaPacker.sizeY), transform.rotation, Color.white, 4);
    }
#endif

    [Serializable]
    public class CheckoutPoint
    {
        public Vector3 position;
        public float rotateY;
        public bool isTaked;
    }
}
