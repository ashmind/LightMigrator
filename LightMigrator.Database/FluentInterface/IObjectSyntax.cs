using JetBrains.Annotations;

namespace LightMigrator.Database.FluentInterface {
    public interface IObjectSyntax {
        [NotNull] string Name { get; }
        [NotNull] string FullNameEscaped { get; }
        bool Exists();
    }
}