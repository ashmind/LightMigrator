using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LightMigrator.Framework.FluentInterface;

namespace LightMigrator.Framework {
    public abstract class Migration : IMigration {
        public void Migrate(IMigrationContext context) {
            Argument.NotNull("context", context);
            Database = context.Databases[context.PrimaryDatabaseName];
            Migrate();
        }
        
        protected abstract void Migrate();

        [NotNull] protected IDatabaseSyntax Database { get; private set; }
    }
}
