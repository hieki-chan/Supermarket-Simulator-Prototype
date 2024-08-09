using UnityEngine;
using Hieki.Utils;
using System;
using Hieki.Pubsub;

[RequireComponent(typeof(AudioSource))]
public class RainSound : MonoBehaviour
{
    public Vector3 zone;
    public Transform target;
    public Window[] windows;

    [Serializable]
    public struct Window
    {
        public Vector3 center;
        [Min(0.1f)]
        public float size;
    }


    [Header("Audio Settings")]
    [Range(0, 1)] public float insideVolume = 0.2f;
    [Range(0, 1)] public float outsideVolume = 1;
    public float audioLerpSpeed;

    AudioSource audioSource;

    Topic weatherTopic = Topic.FromMessage<WeatherMessage>();
    ISubscriber subscriber;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 0; // 2D sound
        audioSource.loop = true;
        Play(false);

        subscriber = new Subscriber();
        subscriber.Subscribe<WeatherMessage>(weatherTopic, (weatherMessage) =>
        {
            Play(weatherMessage.weather == RealTimeWeather.Weather.Rainy);
        });
    }

    private void Update()
    {
        Vector3 targetPos = target.position;
        if (IsInsideBox(target.position, transform.position, zone))
        {
            bool nearWindow = false;

            for (int i = 0; i < windows.Length; i++)
            {
                Window window = windows[i];
                float dist = (window.center - targetPos).magnitude;
                float ratio = dist / window.size;
                if (ratio <= 1)
                {
                    audioSource.volume = Mathf.Lerp(audioSource.volume, Mathf.Clamp(insideVolume * (3 - ratio), insideVolume, 1), audioLerpSpeed * Time.deltaTime);
                    nearWindow = true;
#if UNITY_EDITOR
                    Debug.DrawLine(targetPos, window.center);
#endif
                }
            }
            if (nearWindow == false)
                audioSource.volume = Mathf.Lerp(audioSource.volume, insideVolume, audioLerpSpeed * Time.deltaTime);
        }
        else
        {
            audioSource.volume = Mathf.Lerp(audioSource.volume, outsideVolume, audioLerpSpeed * Time.deltaTime);
        }

    }

    bool IsInsideBox(Vector3 targetPos, Vector3 boxCenter, Vector3 boxSize)
    {
        Vector3 halfSize = boxSize / 2;

        bool isInsideX = targetPos.x >= (boxCenter.x - halfSize.x) && targetPos.x <= (boxCenter.x + halfSize.x);
        //bool isInsideY = playerPosition.y >= (boxCenter.y - halfSize.y) && playerPosition.y <= (boxCenter.y + halfSize.y);
        bool isInsideZ = targetPos.z >= (boxCenter.z - halfSize.z) && targetPos.z <= (boxCenter.z + halfSize.z);

        return isInsideX /*&& isInsideY*/ && isInsideZ;
    }

    public void Play(bool state)
    {
        audioSource.enabled = state;
        this.enabled = state;
    }

#if UNITY_EDITOR

    public Color gizmoColor = Color.green;

    private void OnDrawGizmosSelected()
    {
        SceneDrawer.DrawWireCubeHandles(transform.position, zone, Quaternion.identity, gizmoColor, 3);

        Gizmos.color = gizmoColor;
        for (int i = 0; i < windows.Length; i++)
        {
            Gizmos.DrawWireSphere(windows[i].center, windows[i].size);
        }
    }
#endif
}
