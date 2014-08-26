using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LightMigrator.Engine;
using LightMigrator.Engine.Internal;
using LightMigrator.Framework;
using LightMigrator.Framework.FluentInterface;

namespace LightMigrator.SqlServer {
    public class SqlServerHistoryRepository : IMigrationHistoryRepository {
        [NotNull] private readonly IDatabase _database;

        public SqlServerHistoryRepository([NotNull] IDatabase database) {
            _database = Argument.NotNull("database", database);
        }
        
        public MigrationHistoryTableDefinition DefaultTableDefinition {
            get { return new SqlServerHistoryTableDefinition(); }
        }

        public void EnsureTable(MigrationHistoryTableDefinition table) {
            var actualTable = GetActualTable(table);
            if (!actualTable.Schema.Exists())
                actualTable.Schema.Create();

            if (actualTable.Exists())
                return;

            var sqlDefinition = (SqlServerHistoryTableDefinition)table;
            _database.ExecuteScript(sqlDefinition.GetCreateScript());
        }

        public IReadOnlyCollection<string> GetVersions(MigrationHistoryTableDefinition table) {
            // ReSharper disable once PossibleNullReferenceException
            var actualTable = GetActualTable(table);
            return _database.ExecuteReader("SELECT " + table.VersionColumnName + " FROM " + actualTable.FullNameEscaped, reader => reader.GetString(0))
                            .ToList().AsReadOnly();
        }

        public void Save(MigrationHistoryTableDefinition table, MigrationInfo info) {
            Argument.NotNull("info", info);

            var columnsAndValues = new Dictionary<string, string> { { table.VersionColumnName, "@Version" } };
            if (table.NameColumnName != null)
                columnsAndValues.Add(table.NameColumnName, "@Name");

            if (table.DateColumnName != null)
                columnsAndValues.Add(table.DateColumnName, "GETUTCDATE()");

            if (table.UserColumnName != null)
                columnsAndValues.Add(table.UserColumnName, "SYSTEM_USER");

            var columnsSql = string.Join(", ", columnsAndValues.Keys.Select(k => "[" + k + "]"));
            var valuesSql = string.Join(", ", columnsAndValues.Values);
            var actualTable = GetActualTable(table);
            var sql = "INSERT INTO " + actualTable.FullNameEscaped + " ( " + columnsSql + " ) VALUES ( " + valuesSql + " )";

            _database.ExecuteNonQuery(sql, new { info.Version, info.Name });
        }

        [NotNull]
        private ITableSyntax GetActualTable([NotNull] MigrationHistoryTableDefinition definition) {
            return _database.Schemas[definition.SchemaName].Tables[definition.TableName];
        }
    }
}
