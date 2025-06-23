#nullable enable
using System;
using System.ComponentModel;
using System.Text;

namespace BorderlessGaming.Logic.NekoBoiNick
{
    /// <summary>
    /// Extension methods for <see cref="Exception" />.
    /// </summary>
    internal static class ExceptionExtensions
    {
        /// <summary>
        /// Gets the fully qualified <see cref="Exception" /> message from the specified <see cref="Exception" />.
        /// </summary>
        /// <param name="exception">The <see cref="Exception" /> to get the message of.</param>
        /// <returns>
        /// A <see langword="string" /> of the <see cref="Exception" /> message; otherwise <see langword="null" /> if the
        /// provided <see cref="Exception" /> is <see langword="null" />.
        /// </returns>
        public static string? GetFullyQualifiedExceptionMessage(this Exception? exception)
        {
            if (exception is null) { 
                return null;
            }
            var sb = new StringBuilder()
                .Append('[').Append(exception.GetType().Name).Append("] ").Append(exception.Message);
            if (exception is Win32Exception win32Exception)
            {
                sb.Append(" | ").Append("ErrorCode: ").Append(win32Exception.ErrorCode)
                  .Append(" | ").Append("NativeErrorCode: ").Append(win32Exception.NativeErrorCode)
                  .Append(" | ").Append("HResult: ").Append(win32Exception.HResult);
                if (win32Exception.Source is string source)
                {
                    sb.Append(" | ").Append("Source: ").Append(source);
                }
                if (win32Exception.TargetSite is System.Reflection.MethodBase targetSite)
                {
                    sb.Append(" | ").Append("TargetSite: ").Append(targetSite.Name);
                }
                sb.AppendLine();
            }
            if (exception.HelpLink is string helpLink)
            {
                sb.Append("Get Help At: ").Append(helpLink).AppendLine();
            }
            if (exception.StackTrace is string stackTrace)
            {
                sb.Append(stackTrace).AppendLine();
            }
            if (exception.InnerException is Exception innerException)
            {
                sb.Append(innerException.GetFullyQualifiedExceptionMessage()).AppendLine();
            }
            if (exception is Win32Exception win32Exception2)
            {
                sb.Append("--- Win32Exception Data ---").AppendLine();
                for (int i = 0; i < win32Exception2.Data.Count; i++)
                {
                    sb.Append(win32Exception2.Data.Keys.GetAtIndex(i))
                      .Append(": ").Append(win32Exception2.Data.Values.GetAtIndex(i)).AppendLine();
                }
            }
            return sb.ToString();
        }
    }
}
