using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LightMigrator.Framework;
using LightMigrator.Framework.FluentInterface;

namespace LightMigrator.Engine.Internal {
    public abstract class MigrationHistoryRepositoryBase : IMigrationHistoryRepository {
        [NotNull] protected IDatabase Database { get; private set; }
        [NotNull] protected MigrationHistoryTableDefinition TableDefinition { get; private set; }
        [NotNull] protected ITableSyntax Table { get; private set; }

        protected MigrationHistoryRepositoryBase([NotNull] IDatabase database, [NotNull] MigrationHistoryTableDefinition tableDefinition) {
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

        public abstract IReadOnlyCollection<string> GetVersions();
        public abstract void Save(MigrationInfo info);
    }
}