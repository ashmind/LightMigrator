using JetBrains.Annotations;

namespace LightMigrator.Framework.FluentInterface {
    [PublicAPI]
    public interface ISchemaSyntax : IObjectSyntax {
        [NotNull] ISchemaSyntax Create();
        [NotNull] ICollectionSyntax<ITableSyntax> Tables { get; }
    }
}