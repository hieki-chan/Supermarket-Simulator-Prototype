using System;
using System.Collections.Generic;
using SRandom = System.Random;
using URandom = UnityEngine.Random;

namespace Hieki.Utils
{
    public static class RandomUtils
    {
        public static bool Chance(float value01)
        {
            return URandom.Range(0.0f, 1.0f) <= value01;
        }

        public static bool PercentChance(float value0_100)
        {
            return URandom.Range(0.0f, 100.0f) <= value0_100;
        }

        public static T PickOne<T>(List<T> args)
        {
            return args[URandom.Range(0, args.Count)];
        }

        public static T PickOne<T>(T[] args)
        {
            return args[URandom.Range(0, args.Length)];
        }

        public static T PickRandom<T>(params T[] args)
        {
            return args[URandom.Range(0, args.Length)];
        }

        public static string RandomUserName()
        {
            return "User" + RandomString(10);
        }

        public static string RandomString(int length = 5)
        {
            return Guid.NewGuid().ToString().Substring(0, length);
        }

        public static string RandomID_Str(int length = 5)
        {
            SRandom random = new SRandom(DateTime.UtcNow.Second);
            string strNum = random.Next().ToString().Substring(0, length);
            //int randomNum = int.Parse(strNum);
            //return randomNum;
            return strNum;
        }

        public static int RandomID_Int(int length = 5)
        {
            int randomNum = int.Parse(RandomID_Str(length));
            return randomNum;
        }
    }
}