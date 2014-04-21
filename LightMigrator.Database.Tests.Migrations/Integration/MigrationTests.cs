using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Serilog;
using Xunit;
using LightMigrator.Database.Internal;
using LightMigrator.Database.SqlServer;
using LightMigrator.Running;

namespace LightMigrator.Database.Tests.Integration {
    public class MigrationTests {
        [Fact]
        public void Test() {
            var discovery = new ReflectionDiscovery();
            var database = new SqlServerDatabase(ConfigurationManager.ConnectionStrings["SqlServer"].ConnectionString, d => new TransactionScopeDatabaseRollbackPlan());

            if (database.Exists())
                database.Drop();
            database.Create();

            var runner = new MigrationRunner(
                discovery,
                new ConventionBasedMigrationPlanner(discovery),
                () => new DatabaseMigrationScope(database),
                new SqlServerVersionRepository(database, new DefaultSqlServerVersionTableDefinition()),
                Log.ForContext(GetType())
            );

            runner.RunAll(GetType().Assembly);

            Assert.True(database.Tables["X1"].Exists());
        }
    }
}
