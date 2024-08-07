using Hieki.Pubsub;

namespace Supermarket.Player
{
    public readonly struct CamSensitivityMessage : IMessage
    {
        public readonly float camSensitivity;

        public CamSensitivityMessage(float camSensitivity)
        {
            this.camSensitivity = camSensitivity;
        }
    }

    public readonly struct MoveStateMessage : IMessage
    {
        public readonly bool state;

        public MoveStateMessage(bool state)
        {
            this.state = state;
        }
    }

    public readonly struct LookStateMessage : IMessage
    {
        public readonly bool state;

        public LookStateMessage(bool state)
        {
            this.state = state;
        }
    }

    public readonly struct ControlStateMessage : IMessage
    {
        public readonly bool state;

        public ControlStateMessage(bool state)
        {
            this.state = state;
        }
    }

    public readonly struct InteractMessage : IMessage
    {
        public readonly Interactable interaction;

        public InteractMessage(Interactable interaction)
        {
            this.interaction = interaction;
        }
    }
}