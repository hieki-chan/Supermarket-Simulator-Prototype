namespace Hieki.Utils
{
    public static class StringExtensions
    {
        public static float ReadTime(this string s)
        {
            float length = s.Length;
            float averageWordCount = length / 4;

            int minutes = (int)(averageWordCount / 200);
            float seconds = (averageWordCount / 200 - minutes) * 60f;
            return minutes * 60 + seconds;
        }
    }
}
