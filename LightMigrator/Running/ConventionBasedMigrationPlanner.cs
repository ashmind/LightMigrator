using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using AshMind.Extensions;
using JetBrains.Annotations;
using LightMigrator.Framework;
using LightMigrator.Framework.Conventions;

namespace LightMigrator.Running {
    public class ConventionBasedMigrationPlanner: IMigrationPlanner {
        [NotNull] private readonly IDiscovery _discovery;

        public ConventionBasedMigrationPlanner([NotNull] IDiscovery discovery) {
            _discovery = Argument.NotNull("discovery", discovery);
        }

        public IEnumerable<MigrationInfo> Plan(IEnumerable<IMigration> migrations) {
            var resolved = Resolve(migrations);
            var ordered = resolved.OrderBy(m => {
                if (m.Stage == MigrationStage.BeforeAll)
                    return -1;

                if (m.Stage == MigrationStage.AfterAll)
                    return 1;

                return 0;
            }).ThenBy(m => m.Version);

            return ordered;
        }

        [NotNull]
        private IEnumerable<MigrationInfo> Resolve([NotNull] IEnumerable<IMigration> migrations) {
            var conventionCache = new Dictionary<Assembly, IMigrationConvention>();
            foreach (var migration in migrations) {
                var convention = conventionCache.GetOrAdd(migration.GetType().Assembly, a => _discovery.Discover(a, CreateDefaultConvention));
                yield return convention.GetInfo(migration);
            }
        }

        protected virtual IMigrationConvention CreateDefaultConvention() {
            return new MigrationNameConvention(
                new Regex(@"(?:Migration_?(?<version>\d+)|(?<stage>(?:Before|After)All)(?![A-Za-z]))")
            );
        }
    }
}
