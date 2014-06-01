﻿using System.Collections.Generic;
using JetBrains.Annotations;
using LightMigrator.Framework;
using LightMigrator.Framework.Conventions;

namespace LightMigrator.Running {
    public interface IMigrationPlanner {
        [Pure] [NotNull] IEnumerable<MigrationInfo> Plan([NotNull] IEnumerable<IMigration> migrations);
    }
}