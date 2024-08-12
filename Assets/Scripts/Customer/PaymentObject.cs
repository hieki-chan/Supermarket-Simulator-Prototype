using Supermarket.Pricing;
using UnityEngine.Events;
using UnityEngine;
using Supermarket.Customers;

namespace Supermarket
{
    public class PaymentObject : Interactable
    {
        public PaymentType PaymentType => m_PaymentType;
        [SerializeField] private PaymentType m_PaymentType;
        public unit value;
        public UnityAction OnPayCorrected;

        private void OnEnable()
        {
            outline.enabled = true;
        }

        public override void OnHoverExit()
        {
            return;
        }

        public void SetValue(float val)
        {
            value = val + (PaymentType == PaymentType.Cash ? Random.Range(0, 100f) : 0);
        }

        public void OnPayCorrect()
        {
            OnPayCorrected?.Invoke();
            OnPayCorrected = null;
        }
    }
}