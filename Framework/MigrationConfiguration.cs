using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using AshMind.Extensions;
using JetBrains.Annotations;
using LightMigrator.Framework.FluentInterface;
using LightMigrator.Framework.Internal;

namespace LightMigrator.Framework {
    public class MigrationConfiguration {
        [NotNull] private Func<Assembly, IEnumerable<IMigration>> _migrationProvider;
        [NotNull] private Func<IMigration, string> _versionProvider;
        [NotNull] private Func<IMigration, string> _nameProvider;
        [NotNull] private Func<IDatabaseSyntax, MigrationHistoryTableDefinition> _historyTableProvider;

        public MigrationConfiguration() {
            MigrationProvider = DefaultMigrationProvider;
            HistoryTableProvider = database => {
                // ReSharper disable once AssignNullToNotNullAttribute
                Argument.NotNull("database", database);
                return ((IDefaultHistoryTableDefinitionProvider)database).GetDefaultHistoryTableDefinition();
            };

            VersionProvider = migration => {
                // ReSharper disable once AssignNullToNotNullAttribute
                Argument.NotNull("migration", migration);
                var versionMatch = Regex.Match(migration.GetType().Name, @"\d+");
                return versionMatch.Success ? versionMatch.Value : null; // null will be handled/reported at caller
            };

            NameProvider = migration => {
                // ReSharper disable once AssignNullToNotNullAttribute
                Argument.NotNull("migration", migration);
                var typeName = migration.GetType().Name;
                var nameMatch = Regex.Match(typeName, @"\d+(.+)");
                return nameMatch.Success ? nameMatch.Groups[1].Value : typeName;
            };
        }

        protected static IEnumerable<IMigration> DefaultMigrationProvider(Assembly assembly) {
            if (assembly == null)
                throw new ArgumentNullException("assembly", "Assembly can only be null if a custom MigrationProvider is used.");

            // ReSharper disable once AssignNullToNotNullAttribute
            var types = assembly.GetTypes().Where(t => t.HasInterface<IMigration>());
            foreach (var type in types) {
                IMigration instance;
                try {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    instance = (IMigration)Activator.CreateInstance(type);
                }
                catch (Exception ex) {
                    // ReSharper disable once PossibleNullReferenceException
                    throw new MigrationException("Failed to create instance of " + type.FullName + ": " + ex.Message, ex);
                }

                yield return instance;
            }
        }

        [NotNull]
        public Func<IDatabaseSyntax, MigrationHistoryTableDefinition> HistoryTableProvider {
            get { return _historyTableProvider; }
            protected set { _historyTableProvider = Argument.NotNull("value", value); }
        }

        [NotNull]
        public Func<Assembly, IEnumerable<IMigration>> MigrationProvider {
            get { return _migrationProvider; }
            protected set { _migrationProvider = Argument.NotNull("value", value); }
        }

        [NotNull]
        public Func<IMigration, string> VersionProvider {
            get { return _versionProvider; }
            protected set { _versionProvider = Argument.NotNull("value", value); }
        }

        [NotNull]
        public Func<IMigration, string> NameProvider {
            get { return _nameProvider; }
            protected set { _nameProvider = Argument.NotNull("value", value); }
        }
    }
}
