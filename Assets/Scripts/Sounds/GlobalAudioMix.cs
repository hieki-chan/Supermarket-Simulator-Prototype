using Cysharp.Threading.Tasks;
using Hieki.Pubsub;
using UnityEngine;
using UnityEngine.Audio;

[DefaultExecutionOrder(-11)]
public class GlobalAudioMix : MonoBehaviour
{
    const string MUSIC_VOLUME = "MusicVolume";
    const string SFX_VOLUME = "SFXVolume";

    public AudioMixer audioMixer;

    [Header("Weather Sounds")]
    public AudioClip sunnySound;
    public AudioClip clearSound;
    public AudioClip rainSound;

    AudioSource musicSource;

    Topic musicVolumeTopic = Topic.FromMessage<MusicVolumeMessage>();
    Topic sfxVolumeTopic = Topic.FromMessage<SFXVolumeMessage>();
    Topic weatherTopic = Topic.FromMessage<WeatherMessage>();

    ISubscriber subscriber = new Subscriber();

    private void Awake()
    {
        subscriber.Subscribe<MusicVolumeMessage>(musicVolumeTopic, ChangeMusicVolume);
        subscriber.Subscribe<SFXVolumeMessage>(sfxVolumeTopic, ChangeSFXVolume);
        subscriber.Subscribe<WeatherMessage>(weatherTopic, ChangeMusicSound);

        musicSource = GetComponent<AudioSource>();
    }


    private void ChangeMusicVolume(MusicVolumeMessage message)
    {
        audioMixer.SetFloat(MUSIC_VOLUME, Mathf.Log10(message.volume) * 20);
    }

    private void ChangeSFXVolume(SFXVolumeMessage message)
    {
        audioMixer.SetFloat(SFX_VOLUME, Mathf.Log10(message.volume) * 20);
    }

    private void ChangeMusicSound(WeatherMessage message)
    {
        AudioClip clip = message.weather switch
        {
            RealTimeWeather.Weather.Sunny => sunnySound,
            RealTimeWeather.Weather.Clear => clearSound,
            RealTimeWeather.Weather.Rainy => rainSound,
            _=> sunnySound,
        };

        FadeOut(2f, clip).Forget();
    }

    public async UniTaskVoid FadeOut(float FadeTime, AudioClip clip)
    {
        float startVolume = musicSource.volume;

        while (musicSource.volume > 0)
        {
            musicSource.volume -= startVolume * Time.deltaTime / FadeTime;

            await UniTask.NextFrame();
        }

        //musicSource.Stop();
        musicSource.volume = startVolume;

        musicSource.clip = clip;
        musicSource.Play();
    }
}
