using System.Collections.Generic;
using JetBrains.Annotations;
using LightMigrator.Framework;

namespace LightMigrator.Engine {
    public class MigrationGroup {
        private readonly MigrationConfiguration _configuration;
        private readonly IReadOnlyList<IMigration> _migrations;

        public MigrationGroup([NotNull] MigrationConfiguration configuration, [NotNull] IReadOnlyList<IMigration> migrations) {
            _configuration = Argument.NotNull("configuration", configuration);
            _migrations = Argument.NotNull("migrations", migrations);
        }

        public MigrationConfiguration Configuration {
            get { return _configuration; }
        }

        public IReadOnlyList<IMigration> Migrations {
            get { return _migrations; }
        }
    }
}