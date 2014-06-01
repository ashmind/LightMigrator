using System;
using System.Collections.Generic;
using System.Data;
using JetBrains.Annotations;

namespace LightMigrator.Framework.FluentInterface {
    [PublicAPI]
    public interface IDatabaseSyntax {
        [NotNull] string Name { get; }
        bool Exists();

        [NotNull] ICollectionSyntax<ISchemaSyntax> Schemas { get; }
        [NotNull] ICollectionSyntax<ITableSyntax> Tables { get; }

        [NotNull] IDatabaseSyntax Create();
        [NotNull] IDatabaseSyntax CreateFromScript([NotNull] string script);

        [NotNull] IDatabaseSyntax ExecuteScript([NotNull] string script);
        [NotNull] IDatabaseSyntax ExecuteNonQuery([NotNull] string sql, [CanBeNull] object arguments = null);
        [NotNull] IEnumerable<T> ExecuteReader<T>([NotNull] string sql, [CanBeNull] object arguments, [NotNull] Func<IDataReader, T> transform);
        [NotNull] IEnumerable<T> ExecuteReader<T>([NotNull] string sql, [NotNull] Func<IDataReader, T> transform);

        [NotNull] IDatabaseSyntax Drop();
    }
}