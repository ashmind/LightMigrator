using System;
using System.Linq;
using JetBrains.Annotations;
using LightMigrator.Database.FluentInterface;
using LightMigrator.Database.Internal;

namespace LightMigrator.Database.SqlServer {
    public class SqlServerTable : ITableSyntax {
        [NotNull] private readonly IDatabase _database;

        public SqlServerTable([NotNull] string name, [NotNull] ISchemaSyntax schema, [NotNull] IDatabase database) {
            Name = Argument.NotNull("name", name);
            Schema = Argument.NotNull("schema", schema);
            _database = Argument.NotNull("database", database);
        }

        public string Name { get; private set; }
        public string FullNameEscaped {
            get { return Schema.FullNameEscaped + ".[" + Name + "]"; }
        }
        public ISchemaSyntax Schema { get; private set; }

        public bool Exists() {
            var result = _database.ExecuteReader(
                "SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = @SchemaName AND TABLE_NAME = @TableName",
                new { SchemaName = Schema.Name, TableName = Name },
                r => (int?)r.GetInt32(0)
            ).SingleOrDefault();
            return result == 1;
        }
    }
}