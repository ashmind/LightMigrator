using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace LightMigrator.Engine.Internal {
    public class MigrationInfo {
        public MigrationInfo([NotNull] string version) {
            Version = Argument.NotNull("version", version);
        }

        [NotNull] public string Version { get; private set; }
        [CanBeNull] public string Name { get; set; }
    }
}
