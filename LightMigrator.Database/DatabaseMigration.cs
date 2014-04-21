using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LightMigrator.Database.FluentInterface;

namespace LightMigrator.Database {
    public abstract class DatabaseMigration : IMigration {
        public void Migrate([NotNull] IDatabaseMigrationContext context) {
            // ReSharper disable once AssignNullToNotNullAttribute
            Database = context.Databases[context.PrimaryDatabaseName];
            Migrate();
        }

        protected abstract void Migrate();

        [NotNull] protected IDatabaseSyntax Database { get; private set; }

        void IMigration.Migrate(IMigrationContext context) {
            Migrate(Argument.NotNullAndCast<IDatabaseMigrationContext>("context", context));
        }
    }
}
