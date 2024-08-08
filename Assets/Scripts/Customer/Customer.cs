using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using Supermarket.Products;
using Hieki.AI;
using Hieki.Utils;

namespace Supermarket.Customers
{
    public class Customer : MonoBehaviour, ITreeComponent
    {
        public const int MAX_PRODUCTS = 10;

        public static readonly int IdlingHash = Animator.StringToHash("Idling");
        public static readonly int WalkingHash = Animator.StringToHash("Walking");
        public static readonly int ChoosingHash = Animator.StringToHash("Choosing");
        public static readonly int ChoosingSpeedHash = Animator.StringToHash("ChoosingSpeed");
        public static readonly int GiveHash = Animator.StringToHash("Give");
        public static readonly int UnGiveHash = Animator.StringToHash("UnGive");

        public static UnitPool<PaymentType, PaymentObject> PaymentObjectPool;

        public CustomerType type;
        [NonEditable] public CustomerMood mood;

        //public float MoveSpeed => moveSpeed;
        //[SerializeField] private float moveSpeed;

        //--------------------------BEHAVIOURS-----------------------------------

        [SerializeField] private CustomerSM_Model customerSM;


        [Header("Path Movement")]
        [NonSerialized]
        public MovementPath path;

        [NonEditable] public Vector3 targetPosition;

        [NonEditable]
        public int currentNode;
        public Action OnPathComplete;

        [Header("Shopping")]
        [NonEditable]
        public bool goingShopping;
        [NonSerialized]
        public Storage currentStorage;


        public int takeCount;
        public int takedCount;

        [NonEditable]
        public List<ProductOnSale> productsInBag;

        public Transform handHoldTarget;

        public PaymentObject cashObject;
        public PaymentObject creditCardObject;

        [NonSerialized]
        public Animator m_Animator;
        [NonSerialized]
        public NavMeshAgent m_Agent;

        [Space, SerializeField]
        private TextMeshPro textMesh;

        private void Awake()
        {
            m_Animator = GetComponentInChildren<Animator>();
            m_Agent = GetComponent<NavMeshAgent>();
            m_Agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;



            productsInBag = new List<ProductOnSale>(MAX_PRODUCTS);
            textMesh.gameObject.SetActive(false);

            if (PaymentObjectPool == null)
            {
                PaymentObjectPool = new UnitPool<PaymentType, PaymentObject>(1);

                PaymentObjectPool.Create(cashObject.PaymentType, cashObject, 1);
                PaymentObjectPool.Create(creditCardObject.PaymentType, creditCardObject, 1);
            }

            //customerSM.CreateStates(customerSM);
        }

        private void OnEnable()
        {
            mood = (CustomerMood)UnityEngine.Random.Range((int)CustomerMood.Normal, (int)CustomerMood.Anger + 1);
        }

        private void Start()
        {
            customerSM.Start();
        }

        public void OnUpdate()
        {
            customerSM.UpdateState();
        }

        public void Shop()
        {
            customerSM.isShopping = true;
        }

        public void MoveTowards(Vector3 targetPosition)
        {
            //transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * MoveSpeed);
            m_Agent.SetDestination(targetPosition);
        }

        public void Look(Vector3 direction, float smooth = 5)
        {
            direction.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction.normalized), Time.deltaTime * smooth);
        }

        public void Look(float rotateY, float smooth = 5)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, rotateY, 0), Time.deltaTime * smooth);
        }

        public bool Reached(Vector3 position)
        {
            Vector3 flatTarget = position;
            Vector3 flatPos = transform.position;

            flatPos.y = 0;
            flatPos.y = 0;

            return (flatTarget - flatPos).sqrMagnitude <= m_Agent.stoppingDistance * m_Agent.stoppingDistance;
        }

        public void CompletePath()
        {
            OnPathComplete?.Invoke();
        }

        public void Say(string content)
        {
            textMesh.text = content;
            Flag.Disable(textMesh.gameObject, content.ReadTime() + 1);
        }

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            PaymentObjectPool = null;
        }
#endif
    }
}