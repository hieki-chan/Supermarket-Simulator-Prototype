// fuzzy-search by hieki-chan.
// give me a star: https://github.com/hieki-chan
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hieki
{
    public static class Fuzz
    {
        #region IMPLEMENT
        private const int GramSize = 2;

        /// <summary>
        /// Remove all spaces and convert to lowercase
        /// </summary>
        /// <example>
        /// "fuzzy search" -> "fuzzysearch"
        /// </example>
        private static string CleanString(string str)
        {
            // Replace all spaces with empty string
            str = str.Replace(" ", string.Empty);

            // Convert the string to lowercase
            str = str.ToLower();

            return str;
        }

        /// <summary>
        /// Get n-grams
        /// </summary>
        /// <example>
        /// "fuzzysearch" -> {"fu", "uz", "zz", "zy", "ys", "se", "ea", "ar", "rc", "ch"}
        /// </example>
        private static Dictionary<string, double> NGrams(string str)
        {
            var nGrams = new Dictionary<string, double>();
            var length = str.Length - GramSize + 1;

            for (int i = 0; i < length; i++)
            {
                // Get n-gram substring
                string s = str.Substring(i, GramSize);

                // Count the n-gram
                if (!nGrams.ContainsKey(s))
                {
                    nGrams[s] = 1.0;
                }
                else
                {
                    nGrams[s]++;
                }
            }

            return nGrams;
        }

        /// <summary>
        /// Get magnitude of the n-gram
        /// </summary>
        private static double Magnitude(Dictionary<string, double> ngram)
        {
            return ngram.Values.Sum(v => v * v) / 2;
        }

        /// <summary>
        /// Count number of occurrences of a substring in a string
        /// </summary>
        private static int Count(string str, string val)
        {
            int count = 0;
            int index = str.IndexOf(val, StringComparison.Ordinal);

            while (index != -1)
            {
                count++;
                index = str.IndexOf(val, index + 1, StringComparison.Ordinal);
            }

            return count;
        }

        /// <summary>
        /// Calculate the similarity ratio between two strings
        /// </summary>
        /// <returns>The similarity ratio in range [0, 1]</returns>
        private static double Ratio(string str1, string str2)
        {
            if (str1 == str2)
                return 1.0;

            int str1Size = str1.Length;
            int str2Size = str2.Length;

            if (str1Size == 0 || str2Size == 0)
            {
                return str1 == str2 ? 1.0 : 0.0;
            }

            // Both strings are too short (shorter than gramSize)
            if (str1Size < GramSize || str2Size < GramSize)
            {
                return str1Size > str2Size ?
                    (double)Count(str1, str2) / str1Size :
                    (double)Count(str2, str1) / str2Size;
            }

            var ngramsStr1 = NGrams(CleanString(str1));
            var ngramsStr2 = NGrams(CleanString(str2));

            double matchedCount = 0;

            foreach (var pair in ngramsStr1)
            {
                if (ngramsStr2.ContainsKey(pair.Key))
                {
                    // Calculate matched count
                    matchedCount += pair.Value * ngramsStr2[pair.Key];
                }
            }

            // The result is matched count / total magnitude of 2 n-gram strings
            return matchedCount / (Magnitude(ngramsStr1) + Magnitude(ngramsStr2));
        }
        #endregion

#if UNITY_EDITOR
        public static void SampleTest()
        {
            // TEST CASES
            UnityEngine.Debug.Log("ratio :fuzzy search & fuzi serxh -> " + Ratio("fuzzy search", "fuzi serxh"));
            UnityEngine.Debug.Log("ratio: ths is the test & this is the test -> " + Ratio("ths is the test", "this is the test", 3));
            UnityEngine.Debug.Log("ratio: apple & apkhj -> " + Ratio("apple", "apkhj"));
            UnityEngine.Debug.Log("ratio: ana & banana ->" + Ratio("ana", "banana"));
            UnityEngine.Debug.Log("ratio: coconutt & coconut -> " + Ratio("coconutt", "coconut"));
            UnityEngine.Debug.Log("ratio: something went wrong & smthing go wrog -> " + Ratio("something went wrong", "smthing go wrog"));
            UnityEngine.Debug.Log("ratio: a & ab -> " + Ratio("a", "ab"));
            UnityEngine.Debug.Log("ratio: a & b -> " + Ratio("a", "b"));
            UnityEngine.Debug.Log("ratio: a & a -> " + Ratio("a", "a"));
            UnityEngine.Debug.Log("ratio: \"\" & ab -> " + Ratio("", "ab"));
            UnityEngine.Debug.Log("ratio: \"\" & sting.Empty -> " + Ratio("", string.Empty));
        }
#endif


        /// <summary>
        /// Calculate the similarity between two strings.
        /// </summary>
        /// <returns> the similarity value in range [0,1]</returns>
        public static double Ratio(string str1, string str2, int digits = 4)
        {
            if (string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2))
                return 0;
            return Math.Round(Ratio(str1, str2), digits);
        }

        /// <summary>
        /// Calculate the similarity between two strings.
        /// </summary>
        public static void Ratio(string str1, string str2, out double outScore, int digits = 4)
        {
            if (string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2))
            {
                outScore = 0;
                return;
            }
            outScore = Math.Round(Ratio(str1, str2), digits);
        }

        /// <summary>
        /// Calculate the similarity between a given string and a collection of strings.
        /// </summary>
        /// <returns>an array of ratios</returns>
        public static double[] Ratio(string str, IEnumerable<string> enumerable, int digits = 4)
        {
            List<double> ratios = new List<double>();
            foreach (var item in enumerable)
            {
                ratios.Add(Ratio(str, item, digits));
            }

            return ratios.ToArray();
        }

        /// <summary>
        /// Calculating string similarity as a percentage.
        /// </summary>
        /// <returns> percentage value in range [0,100]</returns>
        public static double Percent(string str1, string str2)
        {
            if (string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2))
                return 0;
            double _ratio = Ratio(str1, str2);

            return Math.Round(_ratio * 100, 2);
        }

        public static SearchResult Match(string pattern, IEnumerable<string> source, int digits = 4)
        {
            SearchResult result = new SearchResult();
            double hightScore = 1;

            foreach (var item in source)
            {
                double r = Ratio(pattern, item, digits);

                if (r >= hightScore)
                {
                    result.str = item;
                    result.ratio = r;
                    hightScore = r;
                }
            }
            return result;
        }

        /// <summary>
        /// Search for strings
        /// </summary>
        /// <param name="pattern">String pattern to search</param>
        /// <param name="source">Collection</param>
        /// <param name="min">The minimum ratio</param>
        /// <param name="digits">Number of decimal</param>
        /// <returns>a list strings and a coressponding ratios list</returns>
        public static SearchResult[] Search(string pattern, IEnumerable<string> source, double min = 0.2, int digits = 4)
        {
            List<SearchResult> result = new List<SearchResult>();
            foreach (var item in source)
            {
                double r = Ratio(pattern, item, digits);
                if (r > min)
                {
                    result.Add(new SearchResult() { str = item, ratio = r });
                }
            }
            return result.ToArray();
        }

        public static void Search<T>(string pattern, IEnumerable<T> source, Func<T, string> ToString, Action<T, double> OnMatched, double min = 0.2, int digits = 4)
        {
            foreach (var item in source)
            {
                double r = Ratio(pattern, ToString(item), digits);
                if (r > min)
                {
                    OnMatched?.Invoke(item, r);
                }
            }
        }


        public static void Sort(string pattern, IEnumerable<string> source)
        {

        }
    }

    [Serializable]
    public struct SearchResult
    {
        public string str;
        public double ratio;
    }
}
