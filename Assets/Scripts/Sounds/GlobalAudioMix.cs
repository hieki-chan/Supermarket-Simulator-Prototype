using Hieki.Pubsub;
using UnityEngine;
using UnityEngine.Audio;

public class GlobalAudioMix : MonoBehaviour
{
    const string MUSIC_VOLUME = "MusicVolume";
    const string SFX_VOLUME = "SFXVolume";

    public AudioMixer audioMixer;

    Topic musicVolumeTopic = Topic.FromMessage<MusicVolumeMessage>();
    Topic sfxVolumeTopic = Topic.FromMessage<SFXVolumeMessage>();

    ISubscriber subscriber = new Subscriber();

    private void Awake()
    {
        subscriber.Subscribe<MusicVolumeMessage>(musicVolumeTopic, ChangeMusicVolume);
        subscriber.Subscribe<SFXVolumeMessage>(sfxVolumeTopic, ChangeSFXVolume);
    }


    public void ChangeMusicVolume(MusicVolumeMessage message)
    {
        audioMixer.SetFloat(MUSIC_VOLUME, Mathf.Log10(message.volume) * 20);
    }

    public void ChangeSFXVolume(SFXVolumeMessage message)
    {
        audioMixer.SetFloat(SFX_VOLUME, Mathf.Log10(message.volume) * 20);
    }
}
