using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace LightMigrator.Running {
    public class MigrationNameConvention : IMigrationConvention {
        private readonly Regex _nameRegex;
        private readonly Regex _eventRegex;

        public MigrationNameConvention([NotNull] Regex nameRegex, [NotNull] Regex eventRegex) {
            _nameRegex = Argument.NotNull("nameRegex", nameRegex);
            _eventRegex = Argument.NotNull("eventRegex", eventRegex);
        }

        public string GetVersion(IMigration migration) {
            var match = _nameRegex.Match(migration.GetType().Name);
            if (!match.Success)
                return null;

            return match.Groups[1].Value;
        }

        public MigrationEvent? GetEvent(IMigration migration) {
            var match = _eventRegex.Match(migration.GetType().Name);
            if (!match.Success)
                return null;

            return (MigrationEvent)Enum.Parse(typeof(MigrationEvent), match.Groups[1].Value);
        }
    }
}
