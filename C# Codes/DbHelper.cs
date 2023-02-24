using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCRPaperlessSystem.Helper
{
    public static class DbHelper
    {
        // The connection string should be stored in a secure configuration file.
        // Consider using encryption or other security measures to protect the credentials.
        private const string ConnectionString = "Data Source=172.29.2.96;Initial Catalog=db_NCRSystem;User ID=sa;Password=qa1server*";

        // Executes a SELECT query and returns a DataTable object containing the results.
        // The query should use parameterized queries to prevent SQL injection attacks.
        // The parameters argument is optional and can be null if the query has no parameters.
        public static async Task<DataTable> GetDataAsync(string query, IEnumerable<SqlParameter> parameters = null)
        {
            // Create a new DataTable object to hold the results.
            var dt = new DataTable();

            // Create a new SqlConnection object using the connection string.
            using var connection = new SqlConnection(ConnectionString);

            // Open the connection asynchronously.
            await connection.OpenAsync();

            // Create a new SqlCommand object using the query and the SqlConnection object.
            using var command = new SqlCommand(query, connection);

            // If parameters were provided, add them to the SqlCommand object.
            if (parameters != null)
            {
                command.Parameters.AddRange(parameters.ToArray());
            }

            // Create a new SqlDataAdapter object using the SqlCommand object.
            // The SqlDataAdapter object is used to fill the DataTable object with the results.
            using var dataAdapter = new SqlDataAdapter(command);

            // Fill the DataTable object with the results.
            dataAdapter.Fill(dt);

            // Return the DataTable object.
            return dt;
        }

        // Executes an INSERT query and returns the number of affected rows.
        // The query should use parameterized queries to prevent SQL injection attacks.
        public static async Task<int> InsertDataAsync(string tableName, IEnumerable<SqlParameter> parameters)
        {
            // Create a StringBuilder object to build the query string.
            var queryBuilder = new StringBuilder($"INSERT INTO {tableName} (");
            var valueBuilder = new StringBuilder("VALUES (");

            // Loop through each SqlParameter object and add its name to the query string.
            // Also, add its value to the values string and add a parameter to the SqlCommand object.
            foreach (var param in parameters)
            {
                queryBuilder.Append($"{param.ParameterName.Substring(1)}, ");
                valueBuilder.Append($"{param.ParameterName}, ");
            }

            // Remove the trailing comma and space from the query and values strings.
            queryBuilder.Remove(queryBuilder.Length - 2, 2).Append(") ");
            valueBuilder.Remove(valueBuilder.Length - 2, 2).Append(")");

            // Combine the query and values strings to form the complete query.
            var query = queryBuilder.ToString() + valueBuilder;

            // Create a new SqlConnection object using the connection string.
            using var connection = new SqlConnection(ConnectionString);

            // Open the connection asynchronously.
            await connection.OpenAsync();

            // Create a new SqlCommand object using the query and the SqlConnection object.
            using var command = new SqlCommand(query, connection);

            // Add the parameters to the SqlCommand object.
            command.Parameters.AddRange(parameters.ToArray());

            // Execute the query and return the number of affected rows.
            var affectedRows = await command.ExecuteNonQueryAsync();
            return affectedRows;
        }

        // Executes an UPDATE query and returns the number of affected rows.
        // The query should use parameterized queries to prevent SQL injection attacks.
        public static async Task<int> UpdateDataAsync(string tableName, IEnumerable<SqlParameter> setParameters, IEnumerable<SqlParameter> whereParameters)
        {
            // Build the initial part of the query string to update the given table
            var queryBuilder = new StringBuilder($"UPDATE {tableName} SET ");

            // Loop through the set parameters and append them to the query string
            foreach (var param in setParameters)
            {
                queryBuilder.Append($"{param.ParameterName.Substring(1)} = {param.ParameterName}, ");
            }

            // Remove the last comma and space from the query string and append the WHERE clause
            queryBuilder.Remove(queryBuilder.Length - 2, 2).Append(" WHERE ");

            // Loop through the where parameters and append them to the query string
            foreach (var param in whereParameters)
            {
                queryBuilder.Append($"{param.ParameterName.Substring(1)} = {param.ParameterName} AND ");
            }

            // Remove the last AND and space from the query string
            queryBuilder.Remove(queryBuilder.Length - 5, 5);

            // Get the final query string
            var query = queryBuilder.ToString();

            // Create a new SqlConnection object with the connection string
            using var connection = new SqlConnection(ConnectionString);

            // Open the connection asynchronously
            await connection.OpenAsync();

            // Create a new SqlCommand object with the query string and SqlConnection object
            using var command = new SqlCommand(query, connection);

            // Add all set and where parameters to the SqlCommand object's Parameters collection
            command.Parameters.AddRange(setParameters.Union(whereParameters).ToArray());

            // Execute the command asynchronously and return the number of affected rows
            var affectedRows = await command.ExecuteNonQueryAsync();
            return affectedRows;
        }

    }
}
