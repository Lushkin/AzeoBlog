namespace NS.SpaceShooter.Services.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extensions class for the collections
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Split a collection into multiple partitions for the specified size.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="size">The size of each partition.</param>
        /// <returns>A collection containing the partitions</returns>
        public static IEnumerable<IEnumerable<TValue>> Split<TValue>(this IEnumerable<TValue> source, int size)
        {
            return Split(source, size, x => x);
        }

        /// <summary>
        /// Split a collection into multiple partitions for the specified size.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="size">The size of each partition.</param>
        /// <param name="resultSelector">The selector function for the creation of the partitions.</param>
        /// <returns>A collection containing the partitions</returns>
        public static IEnumerable<TResult> Split<TValue, TResult>(this IEnumerable<TValue> source, int size, Func<IEnumerable<TValue>, TResult> resultSelector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (size <= 0)
            {
                throw new ArgumentException($"{nameof(size)} must be over zero", nameof(size));
            }

            TValue[] bucket = null;
            var count = 0;
            foreach (var item in source)
            {
                if (bucket == null)
                {
                    bucket = new TValue[size];
                }

                bucket[count++] = item;

                if (count != size)
                {
                    continue;
                }

                yield return resultSelector(bucket.Select(x => x));
                bucket = null;
                count = 0;
            }

            if (bucket != null && count > 0)
            {
                yield return resultSelector(bucket.Take(count));
            }
        }
    }
}
