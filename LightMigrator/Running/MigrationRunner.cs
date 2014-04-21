using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AshMind.Extensions;
using JetBrains.Annotations;
using Serilog;

namespace LightMigrator.Running {
    public class MigrationRunner : IMigrationRunner {
        [NotNull] private readonly IDiscovery _discovery;
        [NotNull] private readonly IMigrationPlanner _planner;
        [NotNull] private readonly Func<IMigrationScope> _scopeFactory;
        [NotNull] private readonly IVersionRepository _runRepository;
        [NotNull] private readonly ILogger _logger;

        public MigrationRunner([NotNull] IDiscovery discovery, [NotNull] IMigrationPlanner planner, [NotNull] Func<IMigrationScope> scopeFactory, [NotNull] IVersionRepository runRepository, [NotNull] ILogger logger) {
            _discovery = Argument.NotNull("discovery", discovery);
            _planner = Argument.NotNull("planner", planner);
            _scopeFactory = Argument.NotNull("scopeFactory", scopeFactory);
            _runRepository = Argument.NotNull("runRepository", runRepository);
            _logger = Argument.NotNull("logger", logger);
        }

        public void RunAll(params Assembly[] assemblies) {
            // ReSharper disable once AssignNullToNotNullAttribute
            RunAll(assemblies.EmptyIfNull().SelectMany(a => _discovery.DiscoverAll<IMigration>(a)));
        }

        public void RunAll([NotNull] IEnumerable<IMigration> migrations) {
            _runRepository.Prepare();

            var alreadyRun = _runRepository.GetAllVersions().ToSet();
            var planned = _planner.Plan(migrations);

            using (var scope = _scopeFactory()) {
                foreach (var migration in planned) {
                    // ReSharper disable once PossibleNullReferenceException
                    if (alreadyRun.Contains(migration.Version)) {
                        _logger.Information("Migration {$migration} skipped (already run).", migration);
                        continue;
                    }

                    _logger.Debug("Migration {$migration} started.", migration);
                    try {
                        migration.Migration.Migrate(scope);
                    }
                    catch (Exception ex) {
                        _logger.Error(ex, "Migration {$migration} failed.", migration);
                        throw new MigrationException("Migration " + migration + " failed: " + ex + ".", ex);
                    }

                    _runRepository.SaveVersion(migration);
                    _logger.Information("Migration {$migration} completed.", migration);
                }

                // ReSharper disable once PossibleNullReferenceException
                scope.Complete();
            }
        }
    }
}
