using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LightMigrator.Framework.Conventions;
using LightMigrator.Framework.FluentInterface;

namespace LightMigrator.Engine {
    public abstract class VersionRepositoryBase : IVersionRepository {
        [NotNull] protected IDatabase Database { get; private set; }
        [NotNull] protected IVersionTableDefinition TableDefinition { get; private set; }
        [NotNull] protected ITableSyntax Table { get; private set; }

        protected VersionRepositoryBase([NotNull] IDatabase database, [NotNull] IVersionTableDefinition tableDefinition) {
            Database = Argument.NotNull("database", database);
            TableDefinition = Argument.NotNull("tableDefinition", tableDefinition);

            Table = Database.Schemas[TableDefinition.SchemaName].Tables[TableDefinition.TableName];
        }

        public void Prepare() {
            if (!Table.Schema.Exists())
                Table.Schema.Create();

            if (Table.Exists())
                return;

            // TODO: implement CREATE TABLE through fluent interface
            Database.ExecuteScript(TableDefinition.CreateScript);
        }

        public abstract IReadOnlyCollection<string> GetAllVersions();
        public abstract void SaveVersion(MigrationInfo migrationInfo);
    }
}