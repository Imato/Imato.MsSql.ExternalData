using FastMember;
using System.Data.SqlClient;
using System.Text;

namespace Imato.MsSql.ExternalData
{
    internal class DbContext
    {
        private readonly string _connectionString = "Data Source=localhost; Initial Catalog=master; Trusted_Connection=True;";
        private readonly string? _tableName;

        public DbContext(string? tableName, string? connectionString = null)
        {
            _tableName = tableName;
            if (connectionString != null)
            {
                _connectionString = connectionString;
            }
        }

        public SqlBulkCopy CreateBulkCopy<T>(string[]? fields = null)
        {
            var bulkCopy = new SqlBulkCopy(_connectionString, SqlBulkCopyOptions.Default);
            bulkCopy.BatchSize = 1_000;
            bulkCopy.BulkCopyTimeout = 60_000;
            bulkCopy.DestinationTableName = _tableName;
            fields ??= typeof(T).GetProperties().Select(x => x.Name).ToArray();
            foreach (var field in fields)
            {
                bulkCopy.ColumnMappings.Add(field, field);
            }
            return bulkCopy;
        }

        public async Task SaveAsync<T>(IEnumerable<T> data)
        {
            if (_tableName != null)
            {
                using var bc = CreateBulkCopy<T>();
                using var reader = ObjectReader.Create(data);
                await bc.WriteToServerAsync(reader);
                return;
            }

            PrintData(data);
        }

        private void PrintData<T>(IEnumerable<T> data)
        {
            Console.WriteLine($"Specify destination table name in parameters (Table=dbo.test)");
            Console.WriteLine("");
            Console.WriteLine("Data:");
            var columns = string.Join(";", typeof(T).GetProperties().Select(x => x.Name));
            Console.WriteLine(columns);
            var sb = new StringBuilder();
            foreach (var d in data)
            {
                sb.Clear();
                foreach (var p in typeof(T).GetProperties())
                {
                    if (sb.Length > 0)
                        sb.Append(";");
                    sb.Append(p.GetValue(d)?.ToString() ?? "null");
                }
                Console.WriteLine(sb);
            }
        }
    }
}