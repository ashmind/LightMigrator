using JetBrains.Annotations;

namespace LightMigrator.Database.FluentInterface {
    public interface ITableSyntax : IObjectSyntax {
        [NotNull] ISchemaSyntax Schema { get; }
    }
}