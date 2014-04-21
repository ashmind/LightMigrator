using System;
using System.Linq;
using JetBrains.Annotations;
using LightMigrator.Database.FluentInterface;
using LightMigrator.Database.Internal;

namespace LightMigrator.Database.SqlServer {
    public class SqlServerSchema : ISchemaSyntax {
        [NotNull] private readonly IDatabase _database;

        public SqlServerSchema([NotNull] string name, [NotNull] IDatabase database) {
            Name = Argument.NotNull("name", name);
            _database = Argument.NotNull("database", database);
        }

        public string Name { get; private set; }
        public string FullNameEscaped {
            get { return "[" + Name + "]"; }
        }

        public bool Exists() {
            var result = _database.ExecuteReader("SELECT 1 FROM sys.schemas WHERE name = @Name", new {Name}, r => (int?)r.GetInt32(0)).SingleOrDefault();
            return result == 1;
        }

        public ISchemaSyntax Create() {
            _database.ExecuteNonQuery("CREATE SCHEMA [" + Name + "]");
            return this;
        }

        public ICollectionSyntax<ITableSyntax> Tables {
            get { return new SqlServerCollection<ITableSyntax>(TableFactory); }
        }

        protected virtual ITableSyntax TableFactory(string name) {
            return new SqlServerTable(name, this, _database);
        }
    }
}