using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imato.MsSql.ExternalData
{
    public class DataProcess<T> : CommandProcess
    {
        public DataProcess(string[] args) : base(args)
        {
        }

        public override async Task RunAsync()
        {
            try
            {
                ConsoleOutput.LogDebug("Get data");
                var data = (await CreateDataAsync())
                    .Union(CreateData());
                ConsoleOutput.LogDebug("Save data");
                await Context.SaveAsync(data);
            }
            catch (Exception ex)
            {
                Console.Error.Write(ex.ToString());
            }

            ConsoleOutput.LogDebug("Done");
        }

        protected override void PrintHelp()
        {
            base.PrintHelp();
            Console.WriteLine("\tConnectionString=, Table= (for DataProcess)");
        }

        /// <summary>
        /// Override and create data output
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected virtual Task<IEnumerable<T>> CreateDataAsync()
        {
            return Task.FromResult(Enumerable.Empty<T>());
        }

        /// <summary>
        /// Override and create data output
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected virtual IEnumerable<T> CreateData()
        {
            return Enumerable.Empty<T>();
        }
    }

    public class DataProcess : DataProcess<IDictionary<string, object>>
    {
        public DataProcess(string[] args) : base(args)
        {
        }
    }
}