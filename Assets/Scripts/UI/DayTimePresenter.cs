using Supermarket.MVP;
using System;
using Day = RealTimeWeather.Day;
using Daypart = RealTimeWeather.DayParts;
using Weather = RealTimeWeather.Weather;

[Serializable]
public class DayTimePresenter : Presenter<RealTimeWeather, RealTimeWeatherView>
{
    public override void Initialize()
    {
        model.OnDayChange += OnDayChange;
        model.OnDayPartChange += OnDayPartChange;
        model.OnWeatherChange += OnWeatherChange;
    }

    private void OnDayChange(Day day)
    {
        view.DayText.text = day.ToString();
    }

    private void OnDayPartChange(Daypart part)
    {
        view.DaypartText.text = $"| {part}";
    }

    private void OnWeatherChange(Weather weather)
    {
        view.WeatherText.text = $"| {weather}";
    }
}