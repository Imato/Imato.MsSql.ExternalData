using FastMember;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Imato.MsSql.ExternalData
{
    public class DbContext
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

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public SqlBulkCopy CreateBulkCopy(IEnumerable<string> fields)
        {
            var bulkCopy = new SqlBulkCopy(_connectionString, SqlBulkCopyOptions.Default);
            bulkCopy.BatchSize = 1_000;
            bulkCopy.BulkCopyTimeout = 60_000;
            bulkCopy.DestinationTableName = _tableName;
            foreach (var field in fields)
            {
                bulkCopy.ColumnMappings.Add(field, field);
            }
            return bulkCopy;
        }

        public SqlBulkCopy CreateBulkCopy<T>(IEnumerable<string>? fields = null)
        {
            fields ??= typeof(T).GetProperties().Select(x => x.Name);
            return CreateBulkCopy(fields);
        }

        public async Task SaveAsync<T>(IEnumerable<T> data)
        {
            if (_tableName != null)
            {
                var dictionaryList = data as IEnumerable<IDictionary<string, object>>;
                if (dictionaryList != null)
                {
                    await SaveAsync(dictionaryList);
                    return;
                }

                using var bc = CreateBulkCopy<T>();
                using var reader = ObjectReader.Create(data);
                await bc.WriteToServerAsync(reader);
                return;
            }

            PrintData(data);
        }

        private async Task SaveAsync(IEnumerable<IDictionary<string, object>> data)
        {
            List<string> fields = new List<string>();
            var dataTable = new DataTable();
            foreach (var d in data)
            {
                if (fields.Count == 0)
                {
                    foreach (var key in d.Keys)
                    {
                        dataTable.Columns.Add(key, d[key].GetType());
                        fields.Add(key);
                    }
                }

                var row = dataTable.NewRow();
                foreach (var key in d.Keys)
                {
                    row[key] = d[key];
                }
                dataTable.Rows.Add(row);
            }

            using var bc = CreateBulkCopy(fields);
            await bc.WriteToServerAsync(dataTable);
        }

        private void PrintData<T>(IEnumerable<T> data)
        {
            Console.WriteLine($"Specify destination table name in parameters (Table=dbo.test)");
            Console.WriteLine("");
            Console.WriteLine("Output data:");
            ConsoleOutput.WriteCsv(data, LogLevel.Error);
        }
    }
}