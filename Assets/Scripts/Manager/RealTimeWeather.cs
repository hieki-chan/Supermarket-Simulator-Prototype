using Hieki.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Hieki.Utils.RandomUtils;

public class RealTimeWeather : MonoBehaviour
{
    [Header("Day Time And Weather")]
    [SerializeField, NonEditable] private Day day;
    [SerializeField, NonEditable] private DayParts part;
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
    public RainSound rainSound;
    public GameObject rainParticlePrefab;

    [NonEditable, SerializeField] 
    private GameObject rainParticleInstance;

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

        rainSound.Play(false);
    }

    void Update()
    {
        dayTimer += Time.deltaTime;
        dayNightFactor = dayTimer / dayDuration;

        DayPart();

        if (dayTimer >= dayDuration)
        {
            NextDay();
        }


        // Update Skybox
        RenderSettings.skybox = (part < DayParts.Evening) ? daySkybox : nightSkybox;

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

        if (part != this.part)
        {
            this.part = part;
            OnDayPartChange?.Invoke(part);
            WeatherForecast();
        }
    }

    public void EndDay()
    {

    }

    private void NextDay()
    {
        dayTimer = 0;
        day++;

        if (day > Day.Sunday)
        {
            day = Day.Monday;
        }

        part = DayParts.Morning;

        OnDayChange?.Invoke(day);
    }

    private void SetWeatherMatrix()
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
                    new NextWeather(Weather.Rainy, .5f),
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
                    new NextWeather(Weather.Rainy, .4f),
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
                    new NextWeather(Weather.Rainy, .5f),
                    new NextWeather(Weather.Cloudy, .4f),
                    new NextWeather(Weather.Sunny, .5f),
                }
            },
        };
    }

    /// <summary>
    /// determines the next weather
    /// </summary>
    private void WeatherForecast()
    {
        if (part != (DayParts.Morning | DayParts.Afternoon | DayParts.Evening))
            return;

        var matrix = weatherMatrix[weather];

        matrix.ForeachRandomStart((next) =>
        {
            if (Chance(next.chance))
            {
                weather = next.weather;
                OnWeatherChange?.Invoke(weather);
                if (weather == Weather.Rainy)
                    Rain();
                else
                    StopRain();
                return true;
            }
            return false;
        });
    }

    private void Rain()
    {
        rainParticleInstance.SetActive(true);
        rainSound.Play(true);
    }

    private void StopRain()
    {
        rainParticleInstance.SetActive(false);
        rainSound.Play(false);
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
