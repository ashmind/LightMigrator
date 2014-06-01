using System.Collections.Generic;
using JetBrains.Annotations;

namespace LightMigrator.Running {
    public interface IMigrationPlanner {
        [NotNull] IEnumerable<MigrationInfo> Plan([NotNull] IEnumerable<IMigration> migrations);
    }
}