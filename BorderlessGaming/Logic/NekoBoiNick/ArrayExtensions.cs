using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BorderlessGaming.Logic.Models.Json;

using BorderlessGaming.Logic.Models;

#nullable enable

namespace BorderlessGaming.Logic.NekoBoiNick {
    public static class ArrayExtensions {
        public static Favorite[] ToBebopArray(this List<JsonFavorite> jsonFavorites) {
            var output = new List<Favorite>();
            foreach (var item in jsonFavorites) {
                output.Add(new() {
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
                    Screen = new() {
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

            if (!jsonFavorites.FavoriteEquals(output, out FavoriteEqualsTuple tuple)) {
                throw new ArrayMismatchException<IEnumerable<JsonFavorite>, JsonFavorite, IEnumerable<Favorite>, Favorite>(null, tuple.length, tuple.index1, tuple.item1, tuple.index2, tuple.item2);
            }

            return output.ToArray();
        }

        public struct FavoriteEqualsTuple
        {
            internal bool length;
            internal int? index1;
            internal int? index2;
            internal JsonFavorite? item1;
            internal Favorite? item2;

            public FavoriteEqualsTuple(bool length = false, int? index1 = null, int? index2 = null, JsonFavorite? item1 = null, Favorite? item2 = null)
            {
                this.length = length;
                this.index1 = index1;
                this.index2 = index2;
                this.item1 = item1;
                this.item2 = item2;
            }
        }

        public static bool FavoriteEquals(this IEnumerable<JsonFavorite> jsonFavorites, IEnumerable<Favorite> favorites, out FavoriteEqualsTuple returned)
        {
            try
            {
                if (jsonFavorites.Count() != favorites.Count())
                {
                    returned = new(true, jsonFavorites.Count(), favorites.Count());
                    return false;
                }
                foreach ((int index1, JsonFavorite item1) in jsonFavorites.Enumerate())
                {
                    foreach ((int index2, Favorite item2) in favorites.Enumerate())
                    {
                        if (index1 == index2 && !item1.Equals(item2))
                        {
                            returned = new(false, index1, index2, item1, item2);
                            return false;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.Write($"Error when equating sequences.\n{exception.Message}\n{exception.StackTrace}\n");
                Console.Write($"Error when equating sequences.\n{exception.Message}\n{exception.StackTrace}\n");
                returned = new(false, null, null, null, null);
                return false;
            }
            returned = new(false, null, null, null, null);
            return true;
        }

        public static bool FavoriteEquals(this IEnumerable<JsonFavorite> jsonFavorites, IEnumerable<Favorite> favorites) {
            try {
                if (jsonFavorites.Count() != favorites.Count()) {
                    return false;
                }
                foreach ((int index1, JsonFavorite item1) in jsonFavorites.Enumerate()) {
                    foreach ((int index2, Favorite item2) in favorites.Enumerate()) {
                        if (index1 == index2 && !item1.Equals(item2)) {
                            return false;
                        }
                    }
                }
            } catch (Exception exception) {
                Debug.Write($"Error when equating sequences.\n{exception.Message}\n{exception.StackTrace}\n");
                Console.Write($"Error when equating sequences.\n{exception.Message}\n{exception.StackTrace}\n");
                return false;
            }
            return true;
        }

        public static IEnumerable<(int index, T item)> Enumerate<T>(this IEnumerable<T> array) where T : notnull {
            return array.Select((item, index) => (index: index, item: item));
        }

        public static List<T> Clone<T>(this List<T> list) where T : class
        {
            return new List<T>(list);
        }

        /// <summary>
        /// Custom Enumerable.SequenceEqual with generic types
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool SequenceEqual<T1, T2>(this IEnumerable<T1> first, IEnumerable<T2> second) where T1 : notnull where T2 : notnull {
            try {
                if (first.Count() != second.Count()) {
                    return false;
                }
                foreach ((int index1, T1 item1) in first.Enumerate()) {
                    foreach ((int index2, T2 item2) in second.Enumerate()) {
                        if (index1 == index2 && !item1.Equals(item2)) {
                            return false;
                        }
                    }
                }
            } catch (Exception exception) {
                Debug.Write($"Error when equating sequences.\n{exception.Message}\n{exception.StackTrace}\n");
                Console.Write($"Error when equating sequences.\n{exception.Message}\n{exception.StackTrace}\n");
            }
            return false;
        }
    }
}
