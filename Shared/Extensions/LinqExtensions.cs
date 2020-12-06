using System;
using System.Collections.Generic;
using System.Linq;

namespace WelcomeTo.Shared.Extensions
{
    /// <summary>
    /// Contains helpful extensions for IEnumerables not present in System.Linq
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// Checks if an IEnumerable is not null and has any elements.
        /// </summary>
        /// <typeparam name="T">Type of elements in the enumerable</typeparam>
        /// <param name="enumerable">Enumerable to check for content</param>
        /// <returns></returns>
        public static bool HasContent<T>(this IEnumerable<T> enumerable) => enumerable is not null && enumerable.Any();

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
        /// <param name="receivers">Collections to add values from the Enumerable to</param>
        public static void Distribute<T>(this IEnumerable<T> enumerable, params ICollection<T>[] receivers)
        {
            if (receivers is null)
            {
                throw new ArgumentNullException(nameof(receivers), "Provided receivers cannot be null!");
            }
            else if (receivers.Length == 0)
            {
                throw new ArgumentException("At least one receiver must be provided!");
            }
            else if (receivers.Any(r => r is null))
            {
                throw new ArgumentNullException(nameof(receivers), "Provided receivers cannot be null!");
            }
            else
            {
                var index = 0;
                var receiversCount = receivers.Length;
                foreach (var value in enumerable)
                {
                    var remainder = index++ % receiversCount;
                    receivers[remainder].Add(value);
                }
            }
        }

        /// <summary>
        /// Returns all elements in source that are not in exclude. If an element appears in source twice and exclude once, only the first instance is excluded.
        /// </summary>
        /// <typeparam name="T">Type of elements in enumerable</typeparam>
        /// <param name="source">Source enumerable</param>
        /// <param name="exclude">Enumerables to exclude (at most once)</param>
        /// <param name="equalityComparer">Optional equality comparer, if none is specified the default comparer is used</param>
        public static IEnumerable<T> Without<T>(this IEnumerable<T> source, IEnumerable<T> exclude, IEqualityComparer<T> equalityComparer = null)
        {
            equalityComparer ??= EqualityComparer<T>.Default;
            var exclusionList = exclude.ToList();

            foreach (var item in source)
            {
                var shouldReturn = true;

                for (int i = 0; i < exclusionList.Count; i++)
                {
                    if (equalityComparer.Equals(item, exclusionList[i]))
                    {
                        exclusionList.RemoveAt(i);
                        shouldReturn = false;
                        break;
                    }
                }

                if (shouldReturn)
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Adds a Pop method to a list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T Pop<T>(this IList<T> list)
        {
            if (list.HasContent())
            {
                var returnValue = list[0];
                list.RemoveAt(0);
                return returnValue;
            }

            return default;
        }

        /// <summary>
        /// Adds a Peek method to a list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T Peek<T>(this IList<T> list) => list.HasContent() ? list[0] : default;

        /// <summary>
        /// Performs an action for each element in an enumerable.
        /// </summary>
        /// <typeparam name="T">Type of elements in the enumerable.</typeparam>
        /// <param name="enumerable">Enumerable to iterate over.</param>
        /// <param name="action">Action to perform on element.</param>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var element in enumerable)
            {
                action(element);
            }
        }
    }
}