using System.Collections;
using UnityEngine;

public class WalkSound : MonoBehaviour
{
    [Header("Requires")]
    [SerializeField] private AudioSource audioSource;
    [Header("Configs")]
    [SerializeField] private float spacingTime;
    [SerializeField] private AudioClip[] audioClips;
    //hide
    private int nowIndex;
    private bool isSpacingTime;
    public void PlaySound()
    {
        if (isSpacingTime)
        {
            return;
        }
        AudioClip audioClip = audioClips[nowIndex];
        audioSource.PlayOneShot(audioClip);

        isSpacingTime = true;
        StartCoroutine(SpacingTimeIE(audioClip.length));

        nowIndex++;
        if (nowIndex >= audioClips.Length)
        {
            nowIndex = 0;
        }

    }
    private IEnumerator SpacingTimeIE(float offset)
    {
        yield return new WaitForSeconds(spacingTime + offset);
        isSpacingTime = false;
    }
}
