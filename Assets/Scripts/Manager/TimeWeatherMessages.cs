using Hieki.Pubsub;

public readonly struct WeatherMessage : IMessage
{
    public readonly RealTimeWeather.Weather weather;

    public WeatherMessage(RealTimeWeather.Weather weather)
    {
        this.weather = weather;
    }
}

public readonly struct DayPartMessage : IMessage
{
    public readonly RealTimeWeather.DayParts dayPart;

    public DayPartMessage(RealTimeWeather.DayParts dayPart)
    {
        this.dayPart = dayPart;
    }
}