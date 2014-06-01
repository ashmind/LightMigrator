using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LightMigrator.Engine;
using LightMigrator.Framework.Conventions;

namespace LightMigrator.SqlServer {
    public class SqlServerVersionRepository : VersionRepositoryBase {
        public SqlServerVersionRepository([NotNull] IDatabase database, [NotNull] IVersionTableDefinition tableDefinition) : base(database, tableDefinition) {
        }

        public override IReadOnlyCollection<string> GetAllVersions() {
            // ReSharper disable once PossibleNullReferenceException
            return Database.ExecuteReader("SELECT " + TableDefinition.VersionColumnName + " FROM " + Table.FullNameEscaped, reader => reader.GetString(0)).ToList().AsReadOnly();
        }

        public override void SaveVersion(MigrationInfo migrationInfo) {
            Argument.NotNull("migrationInfo", migrationInfo);

            var columnsAndValues = new Dictionary<string, string> {{TableDefinition.VersionColumnName, "@Version"}};
            if (TableDefinition.NameColumnName != null)
                columnsAndValues.Add(TableDefinition.NameColumnName, "@Name");

            if (TableDefinition.DateColumnName != null)
                columnsAndValues.Add(TableDefinition.DateColumnName, "GETUTCDATE()");

            if (TableDefinition.UserColumnName != null)
                columnsAndValues.Add(TableDefinition.UserColumnName, "SYSTEM_USER");

            var columnsSql = string.Join(", ", columnsAndValues.Keys.Select(k => "[" + k + "]"));
            var valuesSql = string.Join(", ", columnsAndValues.Values);
            var sql = "INSERT INTO " + Table.FullNameEscaped + " ( " + columnsSql + " ) VALUES ( " + valuesSql + " )";
            
            Database.ExecuteNonQuery(sql, new { migrationInfo.GetType().Name, migrationInfo.Version });
        }
    }
}
