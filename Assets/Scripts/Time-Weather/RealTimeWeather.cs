using UnityEngine;
using static Hieki.Utils.RandomUtils;

public class RealTimeWeather : MonoBehaviour
{
    [SerializeField, NonEditable] private Day day;
    [SerializeField, NonEditable] private DayParts part;
    [SerializeField, NonEditable] private Weather weather;

    [SerializeField] private float dayDuration;
    [SerializeField, NonEditable] private float dayTimer;


    private void Update()
    {
        dayTimer += Time.deltaTime;
        DayPart();

        if (dayTimer >= dayDuration)
        {
            NextDay();
        }
    }

    void DayPart()
    {
        float percent = dayTimer / dayDuration;

        part = percent switch
        {
            < .35f => DayParts.Morning,
            >= .35f and < .5f => DayParts.Noon,
            >= .5f and < .75f => DayParts.Afternoon,
            >= .75f and < 1f => DayParts.Evening,
            _ => DayParts.Night,
        };
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

        //determines the next weather

        weather = weather switch
        {
            Weather.Sunny => Chance(SUNNY_NEXT_SUNNY) ? Weather.Sunny : Chance(SUNNY_NEXT_RAINY) ? Weather.Rainy : Weather.Cloudy,
            Weather.Windy => Chance(WINDY_NEXT_CLOUNDY) ? Weather.Cloudy : Weather.Windy,
            Weather.Cloudy => Chance(CLOUNDY_NEXT_CLOUNDY) ? Weather.Cloudy : Chance(CLOUNDY_NEXT_STORMY) ? Weather.Stormy : Chance(CLOUNDY_NEXT_SUNNY) ? Weather.Sunny : Weather.Windy,
            Weather.Rainy => Chance(RAINY_NEXT_RAINY) ? Weather.Rainy : Chance(RAINY_NEXT_STORMY) ? Weather.Stormy : Weather.Sunny,
            Weather.Stormy => Chance(STORMY_NEXT_SUNNY) ? Weather.Sunny : Weather.Rainy,
            _ => Weather.Sunny,
        };
    }

    const float SUNNY_NEXT_SUNNY = .72f;
    const float SUNNY_NEXT_RAINY = .6f;

    const float WINDY_NEXT_CLOUNDY = .6f;

    const float CLOUNDY_NEXT_CLOUNDY = .6f;
    const float CLOUNDY_NEXT_STORMY = .25f;
    const float CLOUNDY_NEXT_SUNNY = .55f;

    const float RAINY_NEXT_RAINY = .6f;
    const float RAINY_NEXT_STORMY = .5f;

    const float STORMY_NEXT_SUNNY = .4f;


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
        Sunny,
        Windy,
        Cloudy,
        Rainy,
        Stormy
    }
}
