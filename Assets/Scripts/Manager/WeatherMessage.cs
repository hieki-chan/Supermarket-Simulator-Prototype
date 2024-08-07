using Hieki.Pubsub;

public struct WeatherMessage : IMessage
{
    public RealTimeWeather.Weather weather;
}