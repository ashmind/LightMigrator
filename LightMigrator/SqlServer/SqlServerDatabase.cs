using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using LightMigrator.Framework;
using LightMigrator.Framework.FluentInterface;
using LightMigrator.Running;

namespace LightMigrator.SqlServer {
    [PublicAPI]
    public class SqlServerDatabase : IDatabase {
        [NotNull] private readonly SqlServerDatabase _master;

        public SqlServerDatabase([NotNull] string connectionString, [NotNull] Func<IDatabase, IRollbackPlan> rollbackPlanFactory) {
            var name = new SqlConnectionStringBuilder(connectionString).InitialCatalog;
            if (name == null)
                throw new ArgumentException("Cannot identify database name (Initial Catalog) from the connection string.", "connectionString");

            Name = name;
            ConnectionString = Argument.NotNull("connectionString", connectionString);
            // ReSharper disable once AssignNullToNotNullAttribute
            RollbackPlan = Argument.NotNull("rollbackPlanFactory", rollbackPlanFactory).Invoke(this);

            _master = name == "master"
                    ? this
                    : new SqlServerDatabase(new SqlConnectionStringBuilder(ConnectionString) {InitialCatalog = "master"}.ToString(), rollbackPlanFactory);
        }

        public string Name { get; private set; }
        public string ConnectionString { get; private set; }

        public IRollbackPlan RollbackPlan { get; private set; }

        public bool Exists() {
            // ReSharper disable once PossibleNullReferenceException
            var result = _master.ExecuteReader("SELECT 1 FROM sys.databases WHERE name = @Name", new { Name }, r => (int?)r.GetInt32(0)).SingleOrDefault();
            return result == 1; 
        }

        public ICollectionSyntax<ISchemaSyntax> Schemas {
            get { return new SqlServerCollection<ISchemaSyntax>(SchemaFactory); }
        }

        public ICollectionSyntax<ITableSyntax> Tables {
            get { return Schemas["dbo"].Tables; }
        }

        [NotNull]
        protected virtual ISchemaSyntax SchemaFactory([NotNull] string name) {
            return new SqlServerSchema(name, this);
        }

        public IDbConnection OpenConnection() {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }

        public IDatabaseSyntax Create() {
            _master.ExecuteNonQuery("CREATE DATABASE [" + Name + "]");
            return this;
        }

        public IDatabaseSyntax CreateFromScript(string script) {
            _master.ExecuteScript(script);
            return this;
        }

        public IDatabaseSyntax Drop() {
            _master.ExecuteNonQuery("DROP DATABASE [" + Name + "]");
            return this;
        }
        
        public IDatabaseSyntax ExecuteScript(string script) {
            Argument.NotNullOrEmpty("script", script);

            using (var connection = OpenConnection())
            using (var command = connection.CreateCommand()) {
                command.CommandType = CommandType.Text;

                var parts = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                foreach (var part in parts) {
                    try {
                        command.CommandText = part;
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex) {
                        throw new MigrationException(ex.Message + Environment.NewLine + part, part, ex);
                    }
                }
            }

            return this;
        }

        public IDatabaseSyntax ExecuteNonQuery(string sql, object arguments = null) {
            Argument.NotNullOrEmpty("sql", sql);

            using (var connection = OpenConnection())
            using (var command = connection.CreateCommand()) {
                command.CommandType = CommandType.Text;
                command.CommandText = sql;
                AddArguments(command, arguments);

                command.ExecuteNonQuery();
            }
            return this;
        }

        public IEnumerable<T> ExecuteReader<T>(string sql, Func<IDataReader, T> transform) {
            return ExecuteReader(sql, null, transform);
        }

        public IEnumerable<T> ExecuteReader<T>(string sql, object arguments, Func<IDataReader, T> transform) {
            Argument.NotNullOrEmpty("sql", sql);
            Argument.NotNull("transform", transform);

            using (var connection = OpenConnection())
            using (var command = connection.CreateCommand()) {
                command.CommandType = CommandType.Text;
                command.CommandText = sql;
                AddArguments(command, arguments);

                using (var reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        yield return transform(reader);
                    }
                }
            }
        }

        private void AddArguments([NotNull] IDbCommand command, object arguments) {
            if (arguments == null)
                return;

            var properties = arguments.GetType().GetProperties();
            foreach (var property in properties) {
                object value;
                try {
                    // ReSharper disable once PossibleNullReferenceException
                    value = property.GetValue(arguments);
                }
                catch (Exception ex) {
                    // ReSharper disable once PossibleNullReferenceException
                    throw new MigrationException(string.Format("Failed to read property {0} from {1}: {2}", property.Name, arguments, ex), ex);
                }

                var parameter = command.CreateParameter();
                parameter.ParameterName = property.Name;
                parameter.Value = value ?? DBNull.Value;
                // ReSharper disable once PossibleNullReferenceException
                command.Parameters.Add(parameter);
            }
        }
    }
}
