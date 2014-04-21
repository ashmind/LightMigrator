using System.Collections.Generic;
using JetBrains.Annotations;

namespace LightMigrator.Running {
    public interface IMigrationPlanner {
        [NotNull] IEnumerable<PlannedMigration> Plan([NotNull] IEnumerable<IMigration> migrations);
    }
}