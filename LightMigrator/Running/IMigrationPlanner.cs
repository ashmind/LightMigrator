using System.Collections.Generic;
using JetBrains.Annotations;
using LightMigrator.Framework;

namespace LightMigrator.Running {
    public interface IMigrationPlanner {
        [Pure] [NotNull] IEnumerable<MigrationInfo> Plan([NotNull] IEnumerable<IMigration> migrations);
    }
}