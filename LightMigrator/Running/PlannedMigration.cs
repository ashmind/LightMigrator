using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace LightMigrator.Running {
    public class PlannedMigration {
        [NotNull] public IMigration Migration { get; private set; }
        [CanBeNull] public string Version { get; private set; }

        public PlannedMigration(IMigration migration, [CanBeNull] string version = null) {
            Migration = Argument.NotNull("migration", migration);
            Version = version;
        }

        public override string ToString() {
            if (Version != null)
                return Version + " " + Migration;

            return Migration.ToString();
        }
    }
}
