using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using JetBrains.Annotations;
using LightMigrator.Database.FluentInterface;

namespace LightMigrator.Database.Internal {
    public interface IDatabase : IDatabaseSyntax {
        [NotNull] string ConnectionString { get; }
        [NotNull] IDatabaseRollbackPlan RollbackPlan { get; }
        [NotNull] IDbConnection OpenConnection();
    }
}