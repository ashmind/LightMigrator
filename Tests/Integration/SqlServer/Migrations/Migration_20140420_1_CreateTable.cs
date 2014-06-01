using System;
using System.Collections.Generic;
using System.Linq;
using LightMigrator.Framework;

namespace LightMigrator.Tests.Integration.SqlServer.Migrations {
    public class Migration_20140420_1_CreateTable : Migration {
        protected override void Migrate() {
            Database.ExecuteScript(@"
                CREATE TABLE X1 (
                    Id int NOT NULL PRIMARY KEY
                )
            ");

            /*Databases.Create("test")
                     .Tables.Create("Y");

            Schemas.Create("test")
                   .Tables.Create("X", c => new {
                       Id = c.Int.NotNull
                   });

            Tables.Create("Y", new {

            });*/
        }
    }
}
