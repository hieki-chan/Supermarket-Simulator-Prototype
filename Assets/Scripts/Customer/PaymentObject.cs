using Supermarket.Pricing;
using UnityEngine.Events;
using UnityEngine;

namespace Supermarket
{
    public class PaymentObject : Interactable
    {
        public PaymentType PaymentType;
        public StandardCurrency value;
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
            value = val + Random.Range(0, 100f);
        }

        public void OnPayCorrect()
        {
            OnPayCorrected?.Invoke();
            OnPayCorrected = null;
        }
    }
}