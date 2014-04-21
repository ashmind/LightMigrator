using JetBrains.Annotations;

namespace LightMigrator.Database.Internal {
    public interface IDatabaseVersionTableDefinition {
        [NotNull] string SchemaName { get; }
        [NotNull] string TableName { get; }

        [NotNull] string VersionColumnName { get; }
        [CanBeNull] string NameColumnName { get; }
        [CanBeNull] string DateColumnName { get; }
        [CanBeNull] string UserColumnName { get; }

        // Temporary, while here is no fluent interface for CREATE
        [NotNull] string CreateScript { get; }
        
    }
}