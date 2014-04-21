using JetBrains.Annotations;

namespace LightMigrator.Database.FluentInterface {
    public interface ICollectionSyntax<T> {
        [NotNull] T this[[NotNull] string name] { get; }
    }
}