using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;

public class AutomaticDoor : MonoBehaviour
{
    public Transform doorLeft;
    public Transform doorRight;

    public float duration;

    public float move;
    [NonEditable] public float closePosZ;
    [NonEditable] public int enterCount;

    CancellationTokenSource tokenSource;

    private void Awake()
    {
        closePosZ = doorLeft.localPosition.z;
        tokenSource = new CancellationTokenSource();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & (1 << 9 | 1 << 8)) != 0)
        {
            enterCount++;
            if (enterCount == 1)
                OpenDoorAsync().Forget();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & (1 << 9 | 1 << 8)) != 0)
        {
            enterCount--;
            if (enterCount == 0)
                CloseDoorAsync().Forget();
        }
    }

    private void OnDestroy()
    {
        tokenSource.Cancel();
        tokenSource.Dispose();
    }

    private async UniTask OpenDoorAsync()
    {
        await UniTask.WhenAll(
            doorLeft.DOLocalMoveZ(+move, duration).SetEase(Ease.InSine).WithCancellation(tokenSource.Token),
            doorRight.DOLocalMoveZ(-move, duration).SetEase(Ease.InSine).WithCancellation(tokenSource.Token)
            );
    }

    private async UniTask CloseDoorAsync()
    {
        await UniTask.WhenAll(
            doorLeft.DOLocalMoveZ(+closePosZ, duration).SetEase(Ease.InSine).WithCancellation(tokenSource.Token),
            doorRight.DOLocalMoveZ(-closePosZ, duration).SetEase(Ease.InSine).WithCancellation(tokenSource.Token)
            );
    }
}