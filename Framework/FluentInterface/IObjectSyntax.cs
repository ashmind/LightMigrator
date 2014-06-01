using JetBrains.Annotations;

namespace LightMigrator.Framework.FluentInterface {
    [PublicAPI]
    public interface IObjectSyntax {
        [NotNull] string Name { get; }
        [NotNull] string FullNameEscaped { get; }
        bool Exists();
    }
}