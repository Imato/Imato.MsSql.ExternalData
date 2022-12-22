﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imato.MsSql.ExternalData
{
    public class CommandProcess
    {
        protected readonly Dictionary<string, string> Parameters = null!;
        protected readonly DbContext Context = null!;
        protected readonly string? Command;

        public CommandProcess(string[] args)
        {
            Parameters = ParseParameters(args);
            if (args.Contains("/?") || args.Contains("/h") || args.Contains("--help"))
            {
                PrintHelp();
            }
            Context = new DbContext(GetParameter("Table"), GetParameter("ConnectionString"));
            var logLevel = GetParameter("LogLevel");
            if (logLevel != null)
            {
                ConsoleOutput.LogLevel = (LogLevel)Enum.Parse(typeof(LogLevel), logLevel);
            }
            Command = GetParameter("Command");
            ConsoleOutput.LogDebug("Create process");
        }

        public virtual async Task RunAsync()
        {
            try
            {
                ConsoleOutput.LogDebug("Start command");
                await StartCommandAsync();
                StartCommand();
            }
            catch (Exception ex)
            {
                Console.Error.Write(ex.ToString());
            }

            ConsoleOutput.LogDebug("Done");
        }

        protected virtual void PrintHelp()
        {
            Console.WriteLine("Add parameters with value. Example Parameter=Value");
            Console.WriteLine("Used parameters:");
            Console.WriteLine("\tLogLevel=(Error|Warning|Information|Debug");
            Console.WriteLine("\tCommand= (for CommandProcess)");
        }

        /// <summary>
        /// Override to start Command
        /// </summary>
        /// <returns></returns>
        protected virtual Task StartCommandAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Override to start Command
        /// </summary>
        /// <returns></returns>
        protected virtual void StartCommand()
        {
        }

        private Dictionary<string, string> ParseParameters(string[] args)
        {
            var result = new Dictionary<string, string>();
            for (int i = 0; i < args.Length; i++)
            {
                var sp = args[i].IndexOf("=");
                if (sp > 0)
                {
                    var key = args[i].Substring(0, sp);
                    var value = args[i].Substring(sp + 1, args[i].Length - sp - 1);
                    value = value.StartsWith("\"") && value.EndsWith("\"")
                        ? args[i].Substring(sp + 2, args[i].Length - sp - 3)
                        : value;
                    result.Add(key, value);
                }
            }
            return result;
        }

        public string? GetParameter(string name)
        {
            return Parameters.ContainsKey(name) ? Parameters[name] : null;
        }

        public string GetMandatoryParameter(string name)
        {
            return GetParameter(name)
                ?? throw new ArgumentNullException($"Add parameter {name} (example: {name}=Value");
        }
    }
}