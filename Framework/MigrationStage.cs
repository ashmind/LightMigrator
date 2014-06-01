using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace LightMigrator.Framework {
    [PublicAPI]
    public enum MigrationStage {
        BeforeAll,
        AfterAll
    }
}
