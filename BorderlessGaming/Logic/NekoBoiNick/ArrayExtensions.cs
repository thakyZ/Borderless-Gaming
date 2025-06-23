#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using BorderlessGaming.Logic.Models;
using BorderlessGaming.Logic.Models.Json;
using FavoriteEqualsTuple =  (BorderlessGaming.Logic.Models.Json.JsonFavorite? Item1, BorderlessGaming.Logic.Models.Favorite? Item2, bool Length, int? Index1, int? Index2);

namespace BorderlessGaming.Logic.NekoBoiNick
{
    /// <summary>
    /// Extension methods for <see cref="Array" />, <see cref="IEnumerable{T}" />, <see cref="Enumerable" />,
    /// <see cref="IDictionary{TKey, TValue}" />, <see cref="IDictionary" />, and <see cref="IEnumerable{JsonFavroites}" />.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Converts a <see cref="IEnumerable{JsonFavroites}" /> into a <see cref="Favorite[]" />.
        /// </summary>
        /// <param name="jsonFavorites">The specified <see cref="IEnumerable{JsonFavroites}" />.</param>
        /// <returns>A converted <see cref="Favorite[]" />.</returns>
        /// <exception cref="SequenceMismatchException{IEnumerable{JsonFavorite},JsonFavorite,IEnumerable{Favorite},Favorite}">
        /// Thrown when we check if the conversion successfully took place, and the two sequences are not eual.
        /// </exception>
        public static Favorite[] ToBebopArray(this IEnumerable<JsonFavorite> jsonFavorites)
        {
            var output = new List<Favorite>();
            foreach (var item in jsonFavorites)
            {
                output.Add(new()
                {
                    DelayBorderless = item.DelayBorderless,
                    HideMouseCursor = item.HideMouseCursor,
                    HideWindowsTaskbar = item.HideWindowsTaskbar,
                    IsRunning = item.IsRunning,
                    MuteInBackground = item.MuteInBackground,
                    OffsetBottom = item.OffsetBottom,
                    OffsetLeft = item.OffsetLeft,
                    OffsetRight = item.OffsetRight,
                    OffsetTop = item.OffsetTop,
                    PositionHeight = item.PositionHeight,
                    PositionWidth = item.PositionWidth,
                    PositionX = item.PositionX,
                    PositionY = item.PositionY,
                    RemoveMenus = item.RemoveMenus,
                    RunningId = item.RunningId,
                    Screen = new()
                    {
                        Height = item.Screen.Height,
                        Width = item.Screen.Width,
                        X = item.Screen.X,
                        Y = item.Screen.Y,
                    },
                    SearchText = item.SearchText,
                    ShouldMaximize = item.ShouldMaximize,
                    Size = item.Size,
                    TopMost = item.TopMost,
                    Type = item.Type,
                });
            }

            // Check if the two sequences are equal and if not throw.
            if (!jsonFavorites.FavoriteEquals(output, out FavoriteEqualsTuple tuple))
            {
                throw new SequenceMismatchException<IEnumerable<JsonFavorite>,JsonFavorite,IEnumerable<Favorite>,Favorite>(null,
                    tuple.Length, tuple.Index1, tuple.Item1, tuple.Index2, tuple.Item2);
            }

            return [..output];
        }

        /// <summary>
        /// Custom <see cref="Enumerable.SequenceEqual{TSource}(IEnumerable{TSource}, IEnumerable{TSource})" /> with separate
        /// generic types.
        /// </summary>
        /// <typeparam name="T1">The type of the first <see cref="IEnumerable{T1}" />.</typeparam>
        /// <typeparam name="T2">The type of the second <see cref="IEnumerable{T2}" />.</typeparam>
        /// <param name="first">The first <see cref="IEnumerable{T1}" /></param>
        /// <param name="second">The second <see cref="IEnumerable{T2}" /></param>
        /// <returns>
        /// <see langword="true" /> if the two <see cref="IEnumerable{T}" />'s are equal; otherwise <see langword="false" />.
        /// </returns>
        public static bool SequenceEqual<T1, T2>(this IEnumerable<T1> first, IEnumerable<T2> second)
            where T1 : notnull
            where T2 : notnull
        {
            try
            {
                if (first.Count() != second.Count())
                {
                    return false;
                }
                foreach ((int index1, T1 item1) in first.Enumerate())
                {
                    foreach ((int index2, T2 item2) in second.Enumerate())
                    {
                        if (index1 == index2 && !item1.Equals(item2))
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                var error = $"Error when equating sequences.{Environment.NewLine}{exception.GetFullyQualifiedExceptionMessage()}";
                MessageBox.Show(error, "Exception Caught", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.Write(error);
                Console.Write(error);
                return false;
            }
            return true;
        }
        
        /// <summary>
        /// Custom <see cref="Enumerable.SequenceEqual{TSource}(IEnumerable{TSource}, IEnumerable{TSource})" /> with types for
        /// <see cref="JsonFavorite" /> and <see cref="Favorite" />.
        /// </summary>
        /// <param name="jsonFavorites">The <see cref="IEnumerable{JsonFavorite}" />.</param>
        /// <param name="favorites">The <see cref="IEnumerable{Favorite}" /></param>
        /// <returns><see langword="true" /> if the two <see cref="IEnumerable{JsonFavorite}" />
        /// and <see cref="IEnumerable{Favorite}" /> are equal; otherwise <see langword="false" />.</returns>
        public static bool SequenceEqual(this IEnumerable<JsonFavorite> jsonFavorites, IEnumerable<Favorite> favorites)
        {
            try
            {
                if (jsonFavorites.Count() != favorites.Count())
                {
                    return false;
                }
                foreach ((int index1, JsonFavorite item1) in jsonFavorites.Enumerate())
                {
                    foreach ((int index2, Favorite item2) in favorites.Enumerate())
                    {
                        if (index1 == index2 && !item1.Equals(item2))
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                var error = $"Error when equating sequences.{Environment.NewLine}{exception.GetFullyQualifiedExceptionMessage()}";
                MessageBox.Show(error, "Exception Caught", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.Write(error);
                Console.Write(error);
                return false;
            }
            return true;
        }
        
        /// <summary>
        /// Custom <see cref="Enumerable.SequenceEqual{TSource}(IEnumerable{TSource}, IEnumerable{TSource})" />
        /// with types for <see cref="JsonFavorite" /> and <see cref="Favorite" />.
        /// </summary>
        /// <param name="first">The <see cref="IEnumerable{JsonFavorite}" />.</param>
        /// <param name="second">The <see cref="IEnumerable{Favorite}" /></param>
        /// <param name="returned">A custom tuple to see which entries in the segments are not equal.</param>
        /// <returns>
        /// <see langword="true" /> if the two <see cref="IEnumerable{JsonFavorite}" /> and
        /// <see cref="IEnumerable{Favorite}" /> are equal; otherwise <see langword="false" />.
        /// </returns>
        public static bool FavoriteEquals(this IEnumerable<JsonFavorite> jsonFavorites, IEnumerable<Favorite> favorites,
                                          out FavoriteEqualsTuple returned)
        {
            try
            {
                if (jsonFavorites.Count() != favorites.Count())
                {
                    returned = new FavoriteEqualsTuple(null, null, true, jsonFavorites.Count(), favorites.Count());
                    return false;
                }
                foreach ((int index1, JsonFavorite item1) in jsonFavorites.Enumerate())
                {
                    foreach ((int index2, Favorite item2) in favorites.Enumerate())
                    {
                        if (index1 == index2 && !item1.Equals(item2))
                        {
                            returned = new FavoriteEqualsTuple(item1, item2, false, index1, index2);
                            return false;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.Write($"Error when equating sequences.\n{exception.Message}\n{exception.StackTrace}\n");
                Console.Write($"Error when equating sequences.\n{exception.Message}\n{exception.StackTrace}\n");
                returned = new FavoriteEqualsTuple(null, null, false, null, null);
                return false;
            }

            returned = new FavoriteEqualsTuple(null, null, false, null, null);
            return true;
        }

        /// <summary>
        /// Returns a <see cref="IEnumerable{T}" /> containing a <see cref="Tuple{int, T}" /> containing the index
        /// of the item and the item itself in the sequence.
        /// </summary>
        /// <typeparam name="T">The inner type of the <see cref="IEnumerable{T}" />.</typeparam>
        /// <param name="array">The specified <see cref="IEnumerable{T}" /> to enumerate.</param>
        /// <returns>
        /// A clone of the provided <see cref="IEnumerable{T}" /> with a <see cref="Tuple{int, T}" /> as it's inner type.
        /// </returns>
        public static IEnumerable<(int index, T item)> Enumerate<T>(this IEnumerable<T> array) where T : notnull
        {
            return array.Select((item, index) => (index: index, item: item));
        }

        /// <summary>
        /// Clones a <see cref="List{T}" /> into a new <see cref="List{T}" /> instance.
        /// </summary>
        /// <typeparam name="T">The inner type of the <see cref="List{T}" />.</typeparam>
        /// <param name="list">The <see cref="List{T}" /> to clone.</param>
        /// <returns>A new instance of the provided <see cref="List{T}" />.</returns>
        public static List<T> Clone<T>(this List<T> list) where T : class
        {
            return [.. list];
        }

        /// <summary>
        /// Gets an item at a specified index in a <see cref="ICollection" />.
        /// </summary>
        /// <param name="collection">The specified <see cref="ICollection" />.</param>
        /// <param name="index">The index to get the item at.</param>
        /// <returns>
        /// The <see langword="object" /> in the collection at the specified index; otherwise <see langword="null" />.
        /// </returns>
        public static object? GetAtIndex(this ICollection collection, int index)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(index, 0);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, collection.Count);
            int currentIndex = 0;
            foreach (var item in collection)
            {
                if (currentIndex == index)
                {
                    return item;
                }
                currentIndex++;
            }
            return null;
        }

        /// <summary>
        /// Tests if a key value pair exists in the specified <see cref="IDictionary" />.
        /// </summary>
        /// <param name="dictonary">The specified <see cref="IDictionary" /> to test.</param>
        /// <param name="predicate">A function to match the Key, with the Value.</param>
        /// <returns><see langword="true" /> if a match was found; otherwise <see langword="false" />.</returns>
        public static bool Contains(this IDictionary dictonary, Func<object?, object?, bool> predicate)
        {
            for (int i = 0; i < dictonary.Keys.Count; i++)
            {
                if (predicate(dictonary.Keys.GetAtIndex(i), dictonary.Values.GetAtIndex(i)))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Tests if a key value pair exists in the specified <see cref="IDictionary" />.
        /// </summary>
        /// <param name="dictonary">The specified <see cref="IDictionary" /> to test.</param>
        /// <param name="predicate">A function to match the Key, with the Value.</param>
        /// <returns><see langword="true" /> if a match was found; otherwise <see langword="false" />.</returns>
        public static bool Contains(this IDictionary dictonary, Func<string, string, bool> predicate)
        {
            return dictonary.Contains((object key, object value)
                => key is string keyString && value is string valueString && predicate(keyString, valueString));
        }

        /// <summary>
        /// Tests if a key value pair exists in the specified <see cref="IDictionary" />.
        /// </summary>
        /// <param name="dictonary">The specified <see cref="IDictionary" /> to test.</param>
        /// <param name="key">A <see langword="string" /> to match the Key.</param>
        /// <param name="value">A <see langword="string" /> to match the Value.</param>
        /// <param name="keyComparison">A <see cref="StringComparison" /> to compare the key against.</param>
        /// <param name="valueComparison">A <see cref="StringComparison" /> to compare the value against.</param>
        /// <returns><see langword="true" /> if a match was found; otherwise <see langword="false" />.</returns>
        public static bool Contains(this IDictionary dictonary, string key, string value,
                                    StringComparison keyComparison = StringComparison.Ordinal,
                                    StringComparison valueComparison = StringComparison.OrdinalIgnoreCase)
        {
            return dictonary.Contains((string _key, string _value)
                => _key.Equals(key, keyComparison) && _value.Equals(value, valueComparison));
        }
    }
}
