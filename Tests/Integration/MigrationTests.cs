using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using LightMigrator.Engine;
using LightMigrator.SqlServer;
using Serilog;
using Xunit;

namespace LightMigrator.Tests.Integration {
    public class MigrationTests {
        [Fact]
        public void Test() {
            var database = new SqlServerDatabase(
                ConfigurationManager.ConnectionStrings["SqlServer"].ConnectionString,
                d => new TransactionScopeRollbackPlan()
            );

            if (database.Exists())
                database.Drop();
            database.Create();

            var runner = new MigrationRunner(
                () => new MigrationScope(database),
                definition => new SqlServerHistoryRepository(database, definition), 
                Log.ForContext(GetType())
            );

            runner.Run(GetType().Assembly);

            Assert.True(database.Tables["X1"].Exists());
        }
    }
}
