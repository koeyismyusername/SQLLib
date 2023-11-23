using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace SQLLib.Utils
{
    public static class SQLHelper
    {
        public static bool TestConnection(string connectionString)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    return connection.State == ConnectionState.Open;
                }
            }
            catch
            {
                return false;
            }
        }

        public static List<T> ExecuteAll<T>(
            string connectionString,
            string commandText,
            SqlParameter[] parameters = null,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            CommandType commandType = CommandType.Text
        ) where T : new() => Execute
            (
                command =>
                {
                    using (var dataTable = new DataTable())
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                        return dataTable.Rows.CastToEntities<T>();
                    }
                },
                connectionString,
                commandText,
                parameters,
                isolationLevel,
                commandType
            );

        public static T ExecuteFirst<T>(
            string connectionString,
            string commandText,
            SqlParameter[] parameters = null,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            CommandType commandType = CommandType.Text
        ) where T : new() => Execute
            (
                command =>
                {
                    using (var dataTable = new DataTable())
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                        return dataTable.Rows[0].CastToEntity<T>();
                    }
                },
                connectionString,
                commandText,
                parameters,
                isolationLevel,
                commandType
            );

        public static int ExcecuteNone(
            string connectionString,
            string commandText,
            SqlParameter[] parameters = null,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            CommandType commandType = CommandType.Text
        ) => Execute
            (
                command => command.ExecuteNonQuery(),
                connectionString,
                commandText,
                parameters,
                isolationLevel,
                commandType
            );

        private static TResult Execute<TResult>(
            Func<SqlCommand, TResult> SqlCommandHandler,
            string connectionString,
            string commandText,
            SqlParameter[] parameters = null,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            CommandType commandType = CommandType.Text
        ) where TResult : new()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(isolationLevel))
                using (var command = new SqlCommand(commandText, connection, transaction))
                {
                    command.CommandType = commandType;

                    if (parameters != null && parameters.Length > 0)
                        command.Parameters.AddRange(parameters);
                    try
                    {
                        TResult result = SqlCommandHandler(command);
                        transaction.Commit();
                        return result;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }
    }
}
