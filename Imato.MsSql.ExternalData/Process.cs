namespace Imato.MsSql.ExternalData
{
    public class Process<T>
    {
        protected readonly Dictionary<string, string> Parameters = null!;
        protected readonly DbContext context = null!;
        protected bool debug;

        public Process(string[] args)
        {
            Parameters = Parse(args);
            context = new DbContext(GetParameter("Table"), GetParameter("ConnectionString"));
            debug = bool.Parse(GetParameter("Debug") ?? "false");
            PrintTime("Create process");
        }

        public async Task RunAsync()
        {
            try
            {
                PrintTime("Get data");
                var data = (await CreateDataAsync(Parameters))
                .Union(CreateData(Parameters));
                PrintTime("Save process");
                await context.SaveAsync(data);
            }
            catch (Exception ex)
            {
                Console.Error.Write(ex.ToString());
            }

            PrintTime("Done");
        }

        /// <summary>
        /// Override and create data output
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected virtual Task<IEnumerable<T>> CreateDataAsync(
            IDictionary<string, string> parameters)
        {
            return Task.FromResult(Enumerable.Empty<T>());
        }

        /// <summary>
        /// Override and create data output
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected virtual IEnumerable<T> CreateData(
            IDictionary<string, string> parameters)
        {
            return Enumerable.Empty<T>();
        }

        protected void PrintTime(string name)
        {
            if (debug)
                Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")}: {name}");
        }

        private Dictionary<string, string> Parse(string[] args)
        {
            var result = new Dictionary<string, string>();
            for (int i = 0; i < args.Length; i++)
            {
                var sp = args[i].IndexOf("=");
                if (sp <= 0)
                {
                    throw new ApplicationException("Parameter must as 'Key=Value'");
                }

                var key = args[i].Substring(0, sp);
                var value = args[i].Substring(sp + 1, args[i].Length - sp - 1);
                value = value.StartsWith("\"") && value.EndsWith("\"")
                    ? args[i].Substring(sp + 2, args[i].Length - sp - 3)
                    : value;
                result.Add(key, value);
            }
            return result;
        }

        public string? GetParameter(string name)
        {
            return Parameters.ContainsKey(name) ? Parameters[name] : null;
        }

        public string GetMandatoryParameter(string name)
        {
            return GetParameter(name) ?? throw new ArgumentNullException(name);
        }
    }
}