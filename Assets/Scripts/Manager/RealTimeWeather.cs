using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Hieki.Pubsub;
using Hieki.Utils;
using static Hieki.Utils.RandomUtils;
using UnityEditor.Experimental.GraphView;

public class RealTimeWeather : MonoBehaviour
{
    [Header("Day Time And Weather")]
    [SerializeField, NonEditable] private Day day;
    [SerializeField, NonEditable] private DayParts dayPart;
    [SerializeField, NonEditable] private Weather weather;

    [SerializeField] private float dayDuration;
    [SerializeField, NonEditable] private float dayTimer;

    Dictionary<Weather, List<NextWeather>> weatherMatrix;

    [Header("Day Night Effects")]
    public Light directionalLight;
    public Material daySkybox;
    public Material nightSkybox;
    public float dayIntensity = 1f;
    public float nightIntensity = 0.2f;
    public Color dayColor = Color.white;
    public Color nightColor = new Color(0.2f, 0.2f, 0.5f);
    public Color dayAmbient = Color.white;
    public Color nightAmbient = new Color(0.1f, 0.1f, 0.2f);

    [Range(0, 1)]
    public float dayNightFactor = 0; // 0 = day, 1 = night

    [Header("Rain Effects")]
    public GameObject rainParticlePrefab;

    [NonEditable, SerializeField] 
    private GameObject rainParticleInstance;

    private IPublisher publisher;

    private Topic dayPartTopic = Topic.FromString("daypart-change");
    private Topic weatherTopic = Topic.FromString("weather-change");

    CancellationToken token;

    #region EVENTS
    public UnityAction<Day> OnDayChange;
    public UnityAction<DayParts> OnDayPartChange;
    public UnityAction<Weather> OnWeatherChange;
    #endregion

    private void Awake()
    {
        SetWeatherMatrix();

        rainParticleInstance = Instantiate(rainParticlePrefab);
        rainParticleInstance.SetActive(false);

        publisher = new Publisher();

        //day changes
        DayChange();

        //day part changes
        DayPartChange();

        //weather changes
        WeatherChange();

        //update delayed
        token = this.GetCancellationTokenOnDestroy();
        PeriodicUpdate().Forget();
    }

    private async UniTaskVoid PeriodicUpdate()
    {

        while (true)
        {
            await UniTask.Delay(1000, false, PlayerLoopTiming.Update, token);
            DayElapse();
        }
    }

    void DayElapse()
    {
        dayTimer += 1;
        dayNightFactor = dayTimer / dayDuration;

        if (dayTimer >= dayDuration)
        {
            NextDay();
        }

        DayPart();


        // Update Skybox
        RenderSettings.skybox = (dayPart < DayParts.Evening) ? daySkybox : nightSkybox;

        // Update Directional Light
        directionalLight.intensity = Mathf.Lerp(dayIntensity, nightIntensity, dayNightFactor);
        directionalLight.color = Color.Lerp(dayColor, nightColor, dayNightFactor);

        // Update Ambient Light
        RenderSettings.ambientLight = Color.Lerp(dayAmbient, nightAmbient, dayNightFactor);
    }

    void DayPart()
    {
        DayParts part = dayNightFactor switch
        {
            < .35f => DayParts.Morning,
            >= .35f and < .5f => DayParts.Noon,
            >= .5f and < .75f => DayParts.Afternoon,
            >= .75f and < 1f => DayParts.Evening,
            _ => DayParts.Night,
        };

        if (part != dayPart)
        {
            dayPart = part;

            DayPartChange();

            WeatherForecast();
        }
    }

    void DayPartChange()
    {
        OnDayPartChange?.Invoke(dayPart);
        publisher.Publish(dayPartTopic, new DayPartMessage(dayPart));
    }

    public void EndDay()
    {

    }

    void NextDay()
    {
        dayTimer = 0;
        day++;

        if (day > Day.Sunday)
        {
            day = Day.Monday;
        }

        dayPart = DayParts.Morning;
        DayChange();
    }

    void DayChange()
    {
        OnDayChange?.Invoke(day);
    }

    void SetWeatherMatrix()
    {
        weatherMatrix = new Dictionary<Weather, List<NextWeather>>()
        {
            {
                Weather.Clear, new List<NextWeather>
                {
                    new NextWeather(Weather.Clear, .75f),
                    new NextWeather(Weather.Sunny, .5f),
                    new NextWeather(Weather.Windy, .3f),
                }
            },

            {
                Weather.Sunny, new List<NextWeather>
                {
                    new NextWeather(Weather.Sunny, .75f),
                    new NextWeather(Weather.Windy, .3f),
                    new NextWeather(Weather.Cloudy, .25f),
                    new NextWeather(Weather.Rainy, .7f),
                }
            },

            {
                Weather.Windy, new List<NextWeather>
                {
                    new NextWeather(Weather.Windy, .35f),
                    new NextWeather(Weather.Cloudy, .35f),
                    new NextWeather(Weather.Sunny, .35f),
                    new NextWeather(Weather.Clear, .35f),
                }
            },
            {
                Weather.Cloudy, new List<NextWeather>
                {
                    new NextWeather(Weather.Cloudy, .45f),
                    new NextWeather(Weather.Windy, .25f),
                    new NextWeather(Weather.Stormy, .25f),
                    new NextWeather(Weather.Rainy, .65f),
                    new NextWeather(Weather.Sunny, .25f),
                    new NextWeather(Weather.Clear, .5f),
                }
            },
            {
                Weather.Rainy, new List<NextWeather>
                {
                    new NextWeather(Weather.Rainy, .75f),
                    new NextWeather(Weather.Stormy, .35f),
                    new NextWeather(Weather.Sunny, .35f),
                    new NextWeather(Weather.Clear, .35f),
                }
            },
            {
                Weather.Stormy, new List<NextWeather>
                {
                    new NextWeather(Weather.Rainy, .677f),
                    new NextWeather(Weather.Cloudy, .4f),
                    new NextWeather(Weather.Sunny, .5f),
                }
            },
        };
    }

    /// <summary>
    /// determines the next weather
    /// </summary>
    void WeatherForecast()
    {
        if (dayPart != (DayParts.Morning | DayParts.Afternoon | DayParts.Evening))
            return;

        var matrix = weatherMatrix[weather];

        matrix.ForeachRandomStart((next) =>
        {
            if (Chance(next.chance))
            {
                weather = next.weather;
                WeatherChange();

                return true;
            }
            return false;
        });
    }

    void WeatherChange()
    {
        OnWeatherChange?.Invoke(weather);

        publisher.Publish(weatherTopic, new WeatherMessage(weather));

        if (weather == Weather.Rainy)
            Rain();
        else
            StopRain();
    }

    void Rain()
    {
        rainParticleInstance.SetActive(true);
    }

    void StopRain()
    {
        rainParticleInstance.SetActive(false);
    }

    public enum Day
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday
    }

    public enum DayParts
    {
        Morning,
        Noon,
        Afternoon,
        Evening,
        Night
    }

    public enum Weather
    {
        Clear,
        Sunny,
        Windy,
        Cloudy,
        Rainy,
        Stormy
    }
    readonly /*record*/ struct NextWeather  //c# 9.0
    {
        public readonly Weather weather;
        public readonly float chance;
        public NextWeather(Weather next, float chance)
        {
            this.weather = next;
            this.chance = chance;
        }
    }
}
