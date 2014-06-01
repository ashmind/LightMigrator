using JetBrains.Annotations;

namespace LightMigrator.Running {
    public interface IVersionTableDefinition {
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