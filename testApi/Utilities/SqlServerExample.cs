using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;

namespace testApi.Utilities
{
      public class SqlServerExample
    {
        private readonly string _connectionString;

        public SqlServerExample(string connectionString)
        {
            _connectionString = connectionString
                ?? throw new ArgumentNullException(nameof(connectionString));
        }

        /// <summary>
        /// Executes a SELECT query and returns data as a DataTable
        /// </summary>
        public async Task<DataTable> ExecuteQueryAsync(string query,
                                                       IEnumerable<SqlParameter>? parameters = null)
        {
            var dataTable = new DataTable();

            await using var connection = new SqlConnection(_connectionString);
            await using var command = new SqlCommand(query, connection)
            {
                CommandType = CommandType.Text
            };

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }

            await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync();
            dataTable.Load(reader);

            return dataTable;
        }

        public async Task<List<T>> ExecuteQueryAsync<T>(
        string query,
        IEnumerable<SqlParameter>? parameters = null) where T : new()
        {
            var result = new List<T>();

            await using var connection = new SqlConnection(_connectionString);
            await using var command = new SqlCommand(query, connection);

            if (parameters != null)
            {
                foreach (var p in parameters)
                    command.Parameters.Add(p);
            }

            await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            while (await reader.ReadAsync())
            {
                var obj = new T();

                foreach (var prop in properties)
                {
                    if (!reader.HasColumn(prop.Name) || reader[prop.Name] == DBNull.Value)
                        continue;

                    prop.SetValue(obj, Convert.ChangeType(reader[prop.Name], prop.PropertyType));
                }

                result.Add(obj);
            }

            return result;
        }

        /// <summary>
        /// Executes INSERT, UPDATE, DELETE and returns affected rows count
        /// </summary>
        public async Task<int> ExecuteNonQueryAsync(string query,
                                                    IEnumerable<SqlParameter>? parameters = null)
        {
            await using var connection = new SqlConnection(_connectionString);
            await using var command = new SqlCommand(query, connection)
            {
                CommandType = CommandType.Text
            };

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Executes a scalar query (e.g., COUNT, SUM)
        /// </summary>
        public async Task<T?> ExecuteScalarAsync<T>(string query,
                                                    IEnumerable<SqlParameter>? parameters = null)
        {
            await using var connection = new SqlConnection(_connectionString);
            await using var command = new SqlCommand(query, connection)
            {
                CommandType = CommandType.Text
            };

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();

            if (result == null || result == DBNull.Value)
                return default;

            return (T)Convert.ChangeType(result, typeof(T));
        }
    }

}
