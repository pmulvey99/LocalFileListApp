using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalFileListApp
{
    public static class HelperExtensions
    {
        // Pinched from https://stackoverflow.com/questions/11830174/how-to-flatten-tree-via-linq/31881243#31881243
        public static IEnumerable<T> Expand<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> elementSelector)
        {
            var stack = new Stack<IEnumerator<T>>();
            var e = source.GetEnumerator();
            try
            {
                while (true)
                {
                    while (e.MoveNext())
                    {
                        var item = e.Current;
                        yield return item;

                        var elements = elementSelector(item);
                        if (elements == null)
                            continue;

                        stack.Push(e);
                        e = elements.GetEnumerator();
                    }

                    if (stack.Count == 0)
                        break;

                    e.Dispose();
                    e = stack.Pop();
                }
            }
            finally
            {
                e.Dispose();

                while (stack.Count != 0)
                    stack.Pop().Dispose();
            }
        }
    }

    internal static class HelperFunctions
    {
        internal static string ConvertBytesToString(in long bytes)
        {
            StringBuilder sb = new StringBuilder();

            if (bytes < 1024)
            {
                // Display bytes.
                sb.Append(bytes);
                sb.Append(" Bytes");
            }
            else if (bytes < 1024.0 * 1024.0)
            {
                // Display kb.
                double value = bytes / 1024.0;
                sb.Append(value.ToString("F1"));
                sb.Append(" KB");
            }
            else if (bytes < 1024.0 * 1024.0 * 1024.0)
            {
                // Display mb.
                double value = bytes / (1024.0 * 1024.0);
                sb.Append(value.ToString("F1"));
                sb.Append(" MB");
            }
            else if (bytes < 1024.0 * 1024.0 * 1024.0 * 1024.0)
            {
                // Display gb.
                double value = bytes / (1024.0 * 1024.0 * 1024.0);
                sb.Append(value.ToString("F1"));
                sb.Append(" GB");
            }
            else
            {
                // Display tb.
                double value = bytes / (1024.0 * 1024.0 * 1024.0 * 1024.0);
                sb.Append(value.ToString("F1"));
                sb.Append(" TB");
            }

            return sb.ToString();
        }
    }
}
