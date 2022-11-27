namespace Imato.MsSql.ExternalData
{
    public class Process<T>
    {
        protected readonly Dictionary<string, string> Parameters;
        private readonly DbContext context;

        public Process(string[] args)
        {
            Parameters = Parse(args);
            context = new DbContext(GetParameter("Table"), GetParameter("ConnectionString"));
        }

        public async Task RunAsync()
        {
            try
            {
                var data = (await CreateDataAsync(Parameters))
                .Union(CreateData(Parameters));
                await context.SaveAsync(data);
            }
            catch (Exception ex)
            {
                Console.Error.Write(ex.ToString());
            }
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

        private Dictionary<string, string> Parse(string[] args)
        {
            var result = new Dictionary<string, string>();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("ConnectionString="))
                {
                    result.Add("ConnectionString", args[i].Replace("ConnectionString=", ""));
                }
                else
                {
                    var ss = args[i].Split("=");
                    if (ss.Length == 2)
                    {
                        result.Add(ss[0], ss[1]);
                    }
                }
            }
            return result;
        }

        protected string? GetParameter(string name)
        {
            return Parameters.ContainsKey(name) ? Parameters[name] : null;
        }
    }
}