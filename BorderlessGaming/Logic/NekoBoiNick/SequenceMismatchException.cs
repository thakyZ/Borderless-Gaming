#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace BorderlessGaming.Logic.NekoBoiNick
{
    /// <summary>
    /// A custom exception class to be thrown when two Sequences are not equal.
    /// </summary>
    /// <typeparam name="T1">The type of the first sequence.</typeparam>
    /// <typeparam name="TSource1">The inner type of the first sequence.</typeparam>
    /// <typeparam name="T2">The type of the second sequence.</typeparam>
    /// <typeparam name="TSource2">The inner type of the second sequence.</typeparam>
    public class SequenceMismatchException<T1, TSource1, T2, TSource2> : Exception
        where TSource1 : class
        where T1 : class, IEnumerable<TSource1>
        where TSource2 : class
        where T2 : class, IEnumerable<TSource2>
    {
        [Obsolete("Use a method populating the information of the sequences.")]
        public SequenceMismatchException(string? message) : base(message) { }

        /// <summary>
        /// Creates a new instance of <see cref="SequenceMismatchException{T1, TSource1, T2, TSource2}" />.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="listOne">The first sequence.</param>
        /// <param name="listTwo">The second sequence.</param>
        public SequenceMismatchException(string? message, T1 listOne, T2 listTwo)
            : this(message, false, null, null, null, null, listOne, listTwo) { }
        
        /// <summary>
        /// Creates a new instance of <see cref="SequenceMismatchException{T1, TSource1, T2, TSource2}" />.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="index1">The index of the first entry that doesn't match.</param>
        /// <param name="item1">The first entry that doesn't match.</param>
        /// <param name="index2">The index of the second entry that doesn't match.</param>
        /// <param name="item2">The second entry that doesn't match.</param>
        public SequenceMismatchException(
            string? message, int? index1, TSource1? item1, int? index2, TSource2? item2) : this(
                message, false, index1, item1, index2, item2) { }
        
        /// <summary>
        /// Creates a new instance of <see cref="SequenceMismatchException{T1, TSource1, T2, TSource2}" />.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="length">A <see langword="bool" /> that determines if the lengths match.</param>
        /// <param name="index1">The index of the first entry that doesn't match.</param>
        /// <param name="item1">The first entry that doesn't match.</param>
        /// <param name="index2">The index of the second entry that doesn't match.</param>
        /// <param name="item2">The second entry that doesn't match.</param>
        public SequenceMismatchException(
            string? message, bool length, int? index1, TSource1? item1, int? index2, TSource2? item2) : this(
                message, length, index1, item1, index2, item2, null, null) { }
        
        /// <summary>
        /// Creates a new instance of <see cref="SequenceMismatchException{T1, TSource1, T2, TSource2}" />.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="length">A <see langword="bool" /> that determines if the lengths match.</param>
        /// <param name="index1">The index of the first entry that doesn't match.</param>
        /// <param name="item1">The first entry that doesn't match.</param>
        /// <param name="index2">The index of the second entry that doesn't match.</param>
        /// <param name="item2">The second entry that doesn't match.</param>
        /// <param name="listOne">The first sequence.</param>
        /// <param name="listTwo">The second sequence.</param>
        public SequenceMismatchException(
            string? message, bool length, int? index1, TSource1? item1, int? index2, TSource2? item2, T1? listOne, T2? listTwo)
            : base(ModifyMessage(message, length, index1, item1, index2, item2, listOne, listTwo)) { }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        private static (TSource1? Item1, TSource2? Item2, bool Length, int? Index1, int? Index2) FindArrayMismatch(
            IEnumerable<TSource1> first, IEnumerable<TSource2> second)
        {
            if (first.Count() != second.Count())
            {
              return (null, null, true, first.Count(), second.Count());
            }
            foreach ((int index1, TSource1 item1) in first.Enumerate())
            {
                foreach ((int index2, TSource2 item2) in second.Enumerate())
                {
                    if (index1 == index2 && !item1.Equals(item2))
                    {
                        return (null, null, false, index1, index2);
                    }
                }
            }
            return (null, null, false, null, null);
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="message"></param>
        /// <param name="index1"></param>
        /// <param name="item1"></param>
        /// <param name="index2"></param>
        /// <param name="item2"></param>
        /// <returns></returns>
        private static string ModifyMessage(string? message, bool length, int? index1, TSource1? item1, int? index2, TSource2? item2, T1? listOne = null, T2? listTwo = null)
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(message))
            {
                sb.AppendLine(message);
            }
            if (listOne is not null && listTwo is not null && index1 is null && item1 is null && index2 is null && item2 is null && length is false)
            {
                var arrayMismatch = FindArrayMismatch(listOne, listTwo);
                length = arrayMismatch.Length;
                index1 = arrayMismatch.Index1;
                item1 = arrayMismatch.Item1;
                index2 = arrayMismatch.Index2;
                item2 = arrayMismatch.Item2;
            }
            sb.Append("Found mismatch: ");
            if (length)
            {
                sb.Append($"List count mismatch, Left: {index1}, Right: {index2}");
            }
            else
            {
                if (item1 is null && item2 is null)
                {
                    sb.Append($"List mismatch not found, Left: {index1} (null), Right: {index2} (null)");
                }
                else if (item1 is not null && item2 is null)
                {
                    sb.Append($"List mismatch at index, Left: {index1} ({item1.GetHashCode()}), Right: {index2} (null)");
                }
                else if (item1 is null && item2 is not null)
                {
                    sb.Append($"List mismatch at index, Left: {index1} (null), Right: {index2} ({item2.GetHashCode()})");
                }
                else if (item1 is not null && item2 is not null)
                {
                    sb.Append($"List mismatch at index, Left: {index1} ({item1.GetHashCode()}), Right: {index2} ({item2.GetHashCode()})");
                }
            }
            return sb.ToString();
        }
    }
}
