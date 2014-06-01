using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace LightMigrator.Framework {
    [UsedImplicitly]
    [PublicAPI]
    public interface IMigration {
        void Migrate([NotNull] IMigrationContext context);
    }
}
