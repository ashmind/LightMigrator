using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace LightMigrator.Framework.Conventions {
    [ReadOnly]
    public class MigrationInfo {
        [NotNull] public IMigration Migration    { get; private set; }
        [CanBeNull] public string Version        { get; private set; }
        [CanBeNull] public MigrationStage? Stage { get; private set; }

        public MigrationInfo([NotNull] IMigration migration, [NotNull] MigrationStage? stage) {
            Migration = Argument.NotNull("migration", migration);
            // ReSharper disable once AssignNullToNotNullAttribute
            Stage = Argument.NotNull("stage", stage);
        }

        public MigrationInfo([NotNull] IMigration migration, [NotNull] string version) {
            Migration = Argument.NotNull("migration", migration);
            Version = Argument.NotNull("version", version);
        }

        public override string ToString() {
            if (Version != null)
                return Version + ": " + Migration;

            if (Stage != null)
                return Stage + ": " + Migration;

            return Migration.ToString();
        }
    }
}
