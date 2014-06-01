using JetBrains.Annotations;

namespace LightMigrator.Framework.FluentInterface {
    [PublicAPI]
    public interface ICollectionSyntax<T> {
        [NotNull] T this[[NotNull] string name] { get; }
    }
}