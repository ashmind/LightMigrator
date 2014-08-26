using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using JetBrains.Annotations;
using LightMigrator.Engine.Internal;
using LightMigrator.Framework.FluentInterface;

namespace LightMigrator.Engine {
    public interface IDatabase : IDatabaseSyntax {
        [NotNull] string ConnectionString { get; }
        [NotNull] IRollbackPlan RollbackPlan { get; }
        [NotNull] IDbConnection OpenConnection();
        [NotNull] IMigrationHistoryRepository HistoryRepository { get; }
    }
}