using JetBrains.Annotations;

namespace LightMigrator.Framework {
    public abstract class MigrationHistoryTableDefinition {
        [NotNull] private string _schemaName;
        [NotNull] private string _tableName;

        [NotNull]
        public string SchemaName {
            get { return _schemaName; }
            set { _schemaName = Argument.NotNull("value", value); }
        }

        [NotNull]
        public string TableName {
            get { return _tableName; }
            set { _tableName = Argument.NotNull("value", value); }
        }

        [NotNull] public string VersionColumnName { get; set; }
        [CanBeNull] public string NameColumnName { get; set; }
        [CanBeNull] public string DateColumnName { get; set; }
        [CanBeNull] public string UserColumnName { get; set; }

        // Temporary, while here is no fluent interface for CREATE
        [NotNull] public string CreateScript { get;set; }
    }
}