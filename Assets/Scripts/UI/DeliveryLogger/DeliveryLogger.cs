using Cysharp.Threading.Tasks;
using DG.Tweening;
using Supermarket;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryLogger : MonoBehaviour
{
    public LogMessage messagePrefab;
    public int poolCount;

    MonoPool<LogMessage> messagePool = new MonoPool<LogMessage>();
    
    List<LogMessage> arrivedMessages = new List<LogMessage>();

    public Vector2 startPos;
    public float spacing = 100;

    [Header("Animation")]
    public float arrivedDuration = .3f;
    public float startX = -350;
    public float fadeDuration = .25f;

    private void Awake()
    {
        messagePool = new MonoPool<LogMessage>(messagePrefab, poolCount, m => m.transform.SetParent(transform, false));
    }

    public void Log()
    {
        LogMessage message = messagePool.GetOrCreate();
        message.rectTransform.anchoredPosition = startPos;

        for (int i = 0; i < arrivedMessages.Count; i++)
        {
            arrivedMessages[i].rectTransform.anchoredPosition += new Vector2(0, spacing);
        }

        arrivedMessages.Add(message);

        Arrive(message).Forget();
    }

    async UniTaskVoid Arrive(LogMessage message)
    {
        RectTransform rectTransform = message.aniRectTransform;
        rectTransform.anchoredPosition = new Vector2(startX, rectTransform.anchoredPosition.y);

        var token = this.GetCancellationTokenOnDestroy();
        await rectTransform.DOAnchorPosX(0, arrivedDuration).SetEase(Ease.InCubic).WithCancellation(token);

        await UniTask.Delay(5000); //hiding

        Color colorBgBeforeFade = message.background.color;
        Color colorTxtBeforeFade = message.messageText.color;

        await UniTask.WhenAll(message.background.DOFade(0, fadeDuration).WithCancellation(token), 
            message.messageText.DOFade(0, fadeDuration).WithCancellation(token));

        messagePool.Return(message);
        message.background.color = colorBgBeforeFade;
        message.messageText.color = colorTxtBeforeFade;
        
        arrivedMessages.RemoveAt(arrivedMessages.Count - 1);
    }
}
