#if UNITY_EDITOR

namespace Hieki.Pubsub.Sample
{
    internal interface IScene
    {
        public void OnAwake();
        public void OnLoadScene();
    }

    internal class SceneA : IScene
    {
        IPublisher publisher { get; set; }

        public void OnAwake()
        {

        }

        public void OnLoadScene()
        {
            MessagePayload payload = new MessagePayload(100, "Success");
            publisher.Publish(payload);
        }
    }


    internal class SceneB : IScene
    {
        ISubscriber subscriber { get; set; }

        public void OnAwake()
        {
            subscriber.Subscribe<MessagePayload>();
        }

        public void OnLoadScene()
        {

        }
    }

    internal class SceneManager
    {
        SceneA sceneA;
        SceneB sceneB;

        public void Awake()
        {
            sceneA = new SceneA();
            sceneA.OnAwake();   
            sceneB = new SceneB();
            sceneB.OnAwake();

            sceneA.OnLoadScene();
        }
    }

    internal readonly struct MessagePayload
    {
        public readonly float time;
        public readonly string state;

        public MessagePayload(float time, string state)
        {
            this.time = time;
            this.state = state;
        }
    }
}
#endif
