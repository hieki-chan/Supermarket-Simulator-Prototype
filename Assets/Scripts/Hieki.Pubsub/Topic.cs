
namespace Hieki.Pubsub
{
    public readonly struct Topic
    {
        public readonly int id;

        public Topic(int id)
        {
            this.id = id;
        }

        public override readonly int GetHashCode()
        {
            return id.GetHashCode();
        }

        public static Topic FromString(string str)
        {
            return new Topic(str.GetHashCode());
        }

        public static Topic FromMessage<T>() where T : IMessage
        {
            return Topic.FromString(typeof(T).FullName);
        }

        public static Topic FromType<T>()
        {
            return Topic.FromString(typeof(T).FullName);
        }
    }
}