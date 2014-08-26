using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AshMind.Extensions;
using JetBrains.Annotations;
using LightMigrator.Engine.Internal;
using LightMigrator.Framework;
using Serilog;

namespace LightMigrator.Engine {
    [PublicAPI]
    public class MigrationRunner : IMigrationRunner {
        [NotNull] private readonly Func<IMigrationScope> _scopeFactory;
        [NotNull] private readonly ILogger _logger;

        public MigrationRunner([NotNull] Func<IMigrationScope> scopeFactory, [NotNull] ILogger logger) {
            _scopeFactory = Argument.NotNull("scopeFactory", scopeFactory);
            _logger = Argument.NotNull("logger", logger);
        }

        public void Run(Assembly assembly) {
            Argument.NotNull("assembly", assembly);
            Run(GetConfiguration(assembly), assembly);
        }

        public void Run(MigrationConfiguration configuration, Assembly assembly) {
            Argument.NotNull("configuration", configuration);
            _logger.Information("Using configuration {configuration}.", configuration);

            var migrations = configuration.MigrationProvider(assembly);
            if (migrations == null)
                LogAndThrow("MigrationConfiguration.MigrationProvider returned null.", "Failed to get migration list.");

            Run(migrations, configuration);
        }

        private void Run([NotNull] IEnumerable<IMigration> migrations, [NotNull] MigrationConfiguration configuration) {
            using (var scope = _scopeFactory()) {
                // ReSharper disable once PossibleNullReferenceException
                var historyDatabase = scope.Databases[scope.PrimaryDatabaseName];
                var historyRepository = ((IDatabase)historyDatabase).HistoryRepository;
                var historyTable = configuration.HistoryTableOverride(historyRepository.DefaultTableDefinition);
                if (historyTable == null)
                    LogAndThrow("MigrationConfiguration.HistoryTableOverride returned null.", "Incorrect history table override (returned null).");

                historyRepository.EnsureTable(historyTable);

                var alreadyRun = historyRepository.GetVersions(historyTable).ToSet();

                foreach (var migration in migrations) {
                    if (migration == null) {
                        _logger.Warning("Migration skipped (null).");
                        continue;
                    }

                    var info = GetInfo(migration, configuration);

                    if (alreadyRun.Contains(info.Version)) {
                        _logger.Information("Migration {$migration} skipped (already run).", migration);
                        continue;
                    }

                    _logger.Information("Migration {$migration} started.", migration);
                    try {
                        // ReSharper disable once AssignNullToNotNullAttribute
                        migration.Migrate(scope);
                    }
                    catch (Exception ex) {
                        _logger.Error(ex, "Migration {$migration} failed.", migration);
                        throw new MigrationException("Migration " + migration + " failed: " + ex.Message, ex);
                    }

                    historyRepository.Save(historyTable, info);
                    _logger.Information("Migration {$migration} completed.", migration);
                }

                // ReSharper disable once PossibleNullReferenceException
                scope.Complete();
            }
        }

        [NotNull]
        protected virtual MigrationConfiguration GetConfiguration([NotNull] Assembly assembly) {
            var assemblyName = assembly.GetName().Name;
            // ReSharper disable once AssignNullToNotNullAttribute
            var types = assembly.GetTypes().Where(t => t.IsSubclassOf<MigrationConfiguration>()).ToArray();

            if (types.Length > 1) {
                var exceptionMessage = string.Format(
                    "Found more than one MigrationConfiguration type in '{0}':{1}{2}",
                    // ReSharper disable once PossibleNullReferenceException
                    assemblyName, Environment.NewLine, string.Join(Environment.NewLine, types.Select(t => t.FullName))
                );
                LogAndThrow(exceptionMessage, "Found more than one configuration in {$assembly}.", assemblyName);
            }

            if (types.Length == 0)
                return new MigrationConfiguration();

            try {
                // ReSharper disable once AssignNullToNotNullAttribute
                return (MigrationConfiguration)Activator.CreateInstance(types[0]);
            }
            catch (Exception ex) {
                _logger.Error(ex, "Failed to create a configuration for {$assembly}.", assemblyName);
                throw new MigrationException("Failed to create MigrationConfiguration for " + assemblyName + ": " + ex.Message, ex);
            }
        }

        [NotNull]
        protected virtual MigrationInfo GetInfo([NotNull] IMigration migration, [NotNull] MigrationConfiguration configuration) {
            var version = configuration.VersionProvider(migration);
            if (version == null) {
                LogAndThrow(
                    "MigrationConfiguration.VersionProvider returned null for " + migration + ".",
                    "Migration {$migration}: failed to get a version.", migration
                );
            }

            var name = configuration.NameProvider(migration);
            return new MigrationInfo(version) { Name = name };
        }

        [ContractAnnotation("=>halt")]
        private void LogAndThrow(string exceptionMesage, string logMessage, params object[] logPropertyValues) {
            var exception = new MigrationException(exceptionMesage);
            _logger.Error(exception, logMessage, logPropertyValues);
            throw exception;
        }
    }
}
