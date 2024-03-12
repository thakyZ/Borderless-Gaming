using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace BorderlessGaming.Logic.NekoBoiNick
{
    public class ArrayMismatchException<T1, TK1, T2, TK2> : Exception where T1 : IEnumerable<TK1> where T2 : IEnumerable<TK2> where TK1 : class where TK2 : class
    {
        public ArrayMismatchException(string? message) : base(message) { }

        public ArrayMismatchException(string? message, T1 listOne, T2 listTwo) : base(ModifyMessage(message, listOne, listTwo)) { }

        public ArrayMismatchException(string? message, int? index1, TK1? item1, int? index2, TK2? item2) : base(ModifyMessage(message, index1, item1, index2, item2)) { }

        public ArrayMismatchException(string? message, bool length, int? index1, TK1? item1, int? index2, TK2? item2) : base(ModifyMessage(message, length, index1, item1, index2, item2)) { }

        private struct FindArrayMismatchTuple
        {
            public bool length;
            public int? index1;
            public int? index2;
            public TK1? item1;
            public TK2? item2;

            public FindArrayMismatchTuple(bool length = false, int? index1 = null, int? index2 = null, TK1? item1 = null, TK2? item2 = null)
            {
                this.length = length;
                this.index1 = index1;
                this.index2 = index2;
                this.item1 = item1;
                this.item2 = item2;
            }
        }

        private static FindArrayMismatchTuple FindArrayMismatch(IEnumerable<TK1> first, IEnumerable<TK2> second)
        {
            if (first.Count() != second.Count())
            {
              return new FindArrayMismatchTuple(true, first.Count(), second.Count(), default, default);
            }
            foreach ((int index1, TK1 item1) in first.Enumerate())
            {
                foreach ((int index2, TK2 item2) in second.Enumerate())
                {
                    if (index1 == index2 && !item1.Equals(item2))
                    {
                        return new FindArrayMismatchTuple(false, index1, index2, item1, item2);
                    }
                }
            }
            return new FindArrayMismatchTuple(false, null, null, null, null);
        }

        private static string ModifyMessage(string? message, int? index1, TK1? item1, int? index2, TK2? item2)
        {
            var innerMessage = "Found mismatch: ";
#pragma warning disable S2589 // Boolean expressions should not be gratuitous
            if (item1 is null && item2 is null)
            {
                innerMessage += $"List mismatch not found, Left: {index1} (null), Right: {index2} (null)";
            }
            else if (item1 is not null && item2 is null)
            {
                innerMessage += $"List mismatch at index, Left: {index1} ({item1.GetHashCode()}), Right: {index2} (null)";
            }
            else if (item1 is null && item2 is not null)
            {
                innerMessage += $"List mismatch at index, Left: {index1} (null), Right: {index2} ({item2.GetHashCode()})";
            }
            else if (item1 is not null && item2 is not null)
            {
                innerMessage += $"List mismatch at index, Left: {index1} ({item1.GetHashCode()}), Right: {index2} ({item2.GetHashCode()})";
            }
#pragma warning restore S2589 // Boolean expressions should not be gratuitous
            if (!string.IsNullOrEmpty(message))
            {
                return $"{message}\n{innerMessage}";
            }
            return innerMessage;
        }

        private static string ModifyMessage(string? message, bool length, int? index1, TK1? item1, int? index2, TK2? item2)
        {
            var innerMessage = "Found mismatch: ";
            if (length)
            {
                innerMessage += $"List count mismatch, Left: {index1}, Right: {index2}";
            }
            else
            {
                return ModifyMessage(message, index1, item1, index2, item2);
            }
            if (!string.IsNullOrEmpty(message))
            {
                return $"{message}\n{innerMessage}";
            }
            return innerMessage;
        }

        private static string ModifyMessage(string? message, T1 listOne, T2 listTwo)
        {
            var arrayMismatch = FindArrayMismatch(listOne, listTwo);
            return ModifyMessage(message, arrayMismatch.length, arrayMismatch.index1, arrayMismatch.item1, arrayMismatch.index2, arrayMismatch.item2);
        }
    }
}
