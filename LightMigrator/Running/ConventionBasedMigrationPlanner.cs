using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using AshMind.Extensions;

namespace LightMigrator.Running {
    public class ConventionBasedMigrationPlanner: IMigrationPlanner {
        private readonly IDiscovery _discovery;

        public ConventionBasedMigrationPlanner(IDiscovery discovery) {
            _discovery = discovery;
        }

        public IEnumerable<PlannedMigration> Plan(IEnumerable<IMigration> migrations) {
            var migrationsByEvent = new Dictionary<MigrationEvent, IList<IMigration>>();
            var migrationsByVersion = new SortedDictionary<string, IMigration>();
            ClassifyMigrations(migrations, migrationsByVersion, migrationsByEvent);

            var results = new List<PlannedMigration>();
            results.AddRange(migrationsByEvent.GetValueOrDefault(MigrationEvent.BeforeAll).EmptyIfNull().Select(m => new PlannedMigration(m)));
            results.AddRange(migrationsByVersion.Select(versioned => new PlannedMigration(versioned.Value, versioned.Key)));
            results.AddRange(migrationsByEvent.GetValueOrDefault(MigrationEvent.AfterAll).EmptyIfNull().Select(m => new PlannedMigration(m)));

            return results;
        }

        private void ClassifyMigrations(IEnumerable<IMigration> migrations, IDictionary<string, IMigration> migrationsByVersion, IDictionary<MigrationEvent, IList<IMigration>> migrationsByEvent) {
            var conventionCache = new Dictionary<Assembly, IMigrationConvention>();
            foreach (var migration in migrations) {
                var convention = conventionCache.GetOrAdd(migration.GetType().Assembly, a => _discovery.Discover(a, CreateDefaultConvention));

                var version = convention.GetVersion(migration);
                if (version != null) {
                    migrationsByVersion.Add(version, migration);
                    continue;
                }

                var @event = convention.GetEvent(migration);
                if (@event != null) {
                    migrationsByEvent.GetOrAdd(@event.Value, e => new List<IMigration>());
                    continue;
                }

                throw new MigrationException("Cannot identify either version or event for migration " + migration + ".");
            }
        }

        protected virtual IMigrationConvention CreateDefaultConvention() {
            return new MigrationNameConvention(
                new Regex(@"Migration_?(\d+)"),
                new Regex("((?:Before|After)All)(?![A-Za-z])")
            );
        }
    }
}
