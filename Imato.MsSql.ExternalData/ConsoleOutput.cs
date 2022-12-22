using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Imato.MsSql.ExternalData
{
    public static class ConsoleOutput
    {
        public static LogLevel LogLevel { get; set; } = LogLevel.Error;

        public static void WriteCsv<T>(IEnumerable<T> data, LogLevel level = LogLevel.Debug)
        {
            if (level > LogLevel)
                return;

            var dictionaryList = data as IEnumerable<IDictionary<string, object>>;
            if (dictionaryList != null)
            {
                WriteCsv(dictionaryList);
                return;
            }

            var columns = string.Join(";", typeof(T).GetProperties().Select(x => x.Name));
            Console.WriteLine(columns);
            var sb = new StringBuilder();
            var props = typeof(T).GetProperties();
            foreach (var d in data)
            {
                sb.Clear();
                foreach (var p in props)
                {
                    if (sb.Length > 0)
                        sb.Append(";");
                    sb.Append(p.GetValue(d)?.ToString() ?? "null");
                }
                Console.WriteLine(sb);
            }
        }

        private static void WriteCsv(IEnumerable<IDictionary<string, object>> data)
        {
            string? columns = null;
            var sb = new StringBuilder();
            foreach (var d in data)
            {
                if (columns == null)
                {
                    columns = string.Join(";", d.Keys);
                    Console.WriteLine(columns);
                }
                else
                {
                    sb.Clear();
                }

                foreach (var k in d.Keys)
                {
                    if (sb.Length > 0)
                        sb.Append(";");
                    sb.Append(d[k] ?? "null");
                }
                Console.WriteLine(sb);
            }
        }

        public static void WriteJson<T>(IEnumerable<T> data, LogLevel level = LogLevel.Debug)
        {
            if (level > LogLevel)
                return;

            foreach (var d in data)
            {
                WriteJson(d, level);
            }
        }

        public static void WriteJson<T>(T data, LogLevel level = LogLevel.Debug)
        {
            if (level > LogLevel)
                return;

            Console.WriteLine(JsonSerializer.Serialize(data, Constants.JsonOptions));
        }

        public static void Log(string message, LogLevel level)
        {
            if (level > LogLevel)
                return;
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")}: {message}");
        }

        public static void Log(object obj, LogLevel level)
        {
            if (level > LogLevel)
                return;
            WriteJson(obj);
        }

        public static void LogDebug(string message)
        {
            Log(message, LogLevel.Debug);
        }

        public static void LogDebug(object obj)
        {
            Log(obj, LogLevel.Debug);
        }

        public static void LogWarning(string message)
        {
            Log(message, LogLevel.Warning);
        }

        public static void LogWarning(object obj)
        {
            Log(obj, LogLevel.Warning);
        }

        public static void LogInformation(string message)
        {
            Log(message, LogLevel.Information);
        }

        public static void LogInformation(object obj)
        {
            Log(obj, LogLevel.Information);
        }

        public static void LogError(string message)
        {
            Log(message, LogLevel.Error);
        }

        public static void LogError(object obj)
        {
            Log(obj, LogLevel.Error);
        }
    }
}