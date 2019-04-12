namespace DancingLinks.Tests
{
    using System;
    using Newtonsoft.Json;

    public static class ObjectDumperExtensions
    {
        /// <summary>
        /// This is a simple (and lazy, read: effective) solution. Simply send your
        /// object to Newtonsoft serialize method, with the indented formatting, and
        /// you have your own Dump() extension method.
        /// </summary>
        /// <typeparam name="T">The object Type</typeparam>
        /// <param name="anObject">The object to dump</param>
        /// <param name="print">Method to print a string</param>
        /// <param name="aTitle">Optional, will print this before the dump.</param>
        /// <returns>The object as you passed it</returns>
        public static T Dump<T>(this T anObject, Action<string> print = null, string aTitle = "")
        {
            if (print == null)
            {
                print = Console.Out.WriteLine;
            }

            var prettyJson = string.Empty;
            if (!string.IsNullOrEmpty(aTitle))
            {
                prettyJson = $"{aTitle}: ";
            }

            var rawJson = JsonConvert.SerializeObject(anObject, Formatting.None);

            prettyJson += new JsonFormatter(rawJson).Format();

            print(prettyJson);

            return anObject;
        }
    }
}