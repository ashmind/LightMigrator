using JetBrains.Annotations;

namespace LightMigrator.Database.FluentInterface {
    public interface ISchemaSyntax : IObjectSyntax {
        [NotNull] ISchemaSyntax Create();
        [NotNull] ICollectionSyntax<ITableSyntax> Tables { get; }
    }
}