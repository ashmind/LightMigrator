using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using LightMigrator.Framework;
using LightMigrator.Framework.Conventions;
using LightMigrator.Framework.Internal;

namespace LightMigrator.Running {
    [ThreadSafe]
    public class MigrationNameConvention : IMigrationConvention {
        public static class RegexGroupNames {
            public const string Version = "version";
            public const string Stage = "stage";
        }

        [NotNull] private readonly Regex _regex;
        private readonly bool _hasStageGroup;

        public MigrationNameConvention([NotNull] Regex regex) {
            Argument.NotNull("regex", regex);
            var groupNames = regex.GetGroupNames();
            if (!groupNames.Contains("version"))
                throw new ArgumentException(string.Format("Regex /{0}/ must define a capture named '{1}'.", regex, RegexGroupNames.Version));
            
            _regex = regex;
            _hasStageGroup = groupNames.Contains(RegexGroupNames.Stage);
        }

        public MigrationInfo GetInfo(IMigration migration) {
            Argument.NotNull("migration", migration);

            var name = migration.GetType().Name;
            var match = _regex.Match(name);
            if (!match.Success)
                throw new MigrationException(string.Format("Migration name '{0}' did not match convention regex /{1}/.", name, _regex));

            var versionGroup = match.Groups[RegexGroupNames.Version].NotNull();
            if (versionGroup.Success)
                return new MigrationInfo(migration, versionGroup.Value);

            if (_hasStageGroup) {
                var stageGroup = match.Groups[RegexGroupNames.Stage].NotNull();
                
                if (stageGroup.Success)
                    return new MigrationInfo(migration, (MigrationStage)Enum.Parse(typeof(MigrationStage), stageGroup.Value));
            }

            throw new MigrationException(string.Format(
                "Migration name '{0}' matched convention regex /{1}/, but neither '{2}' or '{3}' matched.",
                name, _regex, RegexGroupNames.Version, RegexGroupNames.Stage
            ));
        }
    }
}
