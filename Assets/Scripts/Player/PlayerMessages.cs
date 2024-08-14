using Hieki.Pubsub;
using System.Runtime.CompilerServices;

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

    /// <summary>
    /// Set move and look state
    /// </summary>
    public readonly struct ControlStateMessage : IMessage
    {
        public readonly bool state;

        public ControlStateMessage(bool state)
        {
            this.state = state;
        }
    }

    /// <summary>
    /// Set character controller enable/disable
    /// </summary>
    public readonly struct CharCtrllerStateMessage : IMessage
    {
        public readonly bool state;

        public CharCtrllerStateMessage(bool state)
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

    public readonly struct CastingMessage : IMessage
    {
        public readonly bool state;

        public CastingMessage(bool state)
        {
            this.state = state;
        }
    }

    public static class PlayerTopics
    {
        /// <summary>
        /// Set target interacttion.
        /// </summary>
        public static Topic interactTopic { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _interactTopic; }
        private static readonly Topic _interactTopic = Topic.FromMessage<InteractMessage>();

        /// <summary>
        /// Enable/Disable move and look control.
        /// </summary>
        public static Topic controlTopic { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _controlTopic; }
        private static readonly Topic _controlTopic = Topic.FromMessage<ControlStateMessage>();

        /// <summary>
        /// Enable/Disable character controller.
        /// </summary>
        public static Topic charCtrllerTopic { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _controllerTopic; }
        private static readonly Topic _controllerTopic = Topic.FromMessage<CharCtrllerStateMessage>();

        /// <summary>
        /// Set camera sensitivity.
        /// </summary>
        public static Topic camSentvtTopic { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _camSentvtTopic; }
        private static readonly Topic _camSentvtTopic = Topic.FromMessage<CamSensitivityMessage>();

        /// <summary>
        /// Enable/Disable target casting.
        /// </summary>
        public static Topic castingTopic { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _castTopic; }
        private static readonly Topic _castTopic = Topic.FromMessage<CastingMessage>();
    }
}