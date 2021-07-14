using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Utilities
{
    public static class StringUtilities
    {
        public static bool EqualsIgnorecase(this string value, string otherValue)
        {
            return value.Equals(otherValue, StringComparison.OrdinalIgnoreCase);
        }
    }

    public static class TaskUtilities
    {
        public static Task WhenAll(this IEnumerable<Task> tasks)
        {
            return Task.WhenAll(tasks);
        }
    }

    public static class FileUtilities
    {
        public static void SafeDelete(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}