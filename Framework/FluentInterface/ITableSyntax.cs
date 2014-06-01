using JetBrains.Annotations;

namespace LightMigrator.Framework.FluentInterface {
    [PublicAPI]
    public interface ITableSyntax : IObjectSyntax {
        [NotNull] ISchemaSyntax Schema { get; }
    }
}