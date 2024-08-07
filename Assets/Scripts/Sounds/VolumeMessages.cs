using Hieki.Pubsub;

public readonly struct MusicVolumeMessage : IMessage
{
    public readonly float volume;

    public MusicVolumeMessage(float volume)
    {
        this.volume = volume;
    }
}

public readonly struct SFXVolumeMessage : IMessage
{
    public readonly float volume;

    public SFXVolumeMessage(float volume)
    {
        this.volume = volume;
    }
}