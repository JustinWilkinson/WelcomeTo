using System;
using System.Collections.Generic;
using System.Linq;

namespace WelcomeTo.Server.Extensions
{
    public static class LinqExtensions
    {
        /// <summary>
        /// Shuffles an IEnumerable using the System.Random class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable">Enumerable to shuffle</param>
        /// <param name="numberOfShuffles">Number of times to shuffle</param>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable, int numberOfShuffles = 5)
        {
            Random random = new Random();
            var array = enumerable.ToArray();

            for (var i = 0; i < numberOfShuffles; i++)
            {
                var unshuffledLength = array.Length;
                while (unshuffledLength > 1)
                {
                    var swapIndex = random.Next(unshuffledLength--);
                    var current = array[unshuffledLength];
                    array[unshuffledLength] = array[swapIndex];
                    array[swapIndex] = current;
                }
            }

            return array;
        }

        /// <summary>
        /// Distribute the contents of an IEnumerable evenly amongst the provided receiving collections.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable">Enumerable to distribute</param>
        /// <param name="receivers">Collections to add values from the Enumerable to.</param>
        public static void Distribute<T>(this IEnumerable<T> enumerable, params IEnumerable<T>[] receivers)
        {
            if (receivers == null)
            {
                throw new ArgumentNullException("Provided receivers cannot be null!");
            }
            else if (receivers.Length == 0)
            {
                throw new ArgumentException("At least one receiver must be provided!");
            }
            else if (receivers.Any(r => r == null))
            {
                throw new ArgumentNullException("Provided receivers cannot be null!");
            }
            else
            {
                var index = 0;
                var receiversCount = receivers.Length;
                foreach (var value in enumerable)
                {
                    var remainder = index++ % receiversCount;
                    receivers[remainder].Append(value);
                }
            }
        }
    }
}