using System;
using System.Collections.Generic;
using System.Linq;
using LightMigrator.Framework;

namespace LightMigrator.SqlServer {
    public class SqlServerHistoryTableDefinition : MigrationHistoryTableDefinition {
        public SqlServerHistoryTableDefinition() {
            SchemaName = "dbo";
            TableName = "MigrationHistory";
            VersionColumnName = "Version";
            NameColumnName = "Name";
            DateColumnName = "DateUtc";
            UserColumnName = "User";

            CreateScript = @"
                CREATE TABLE dbo.MigrationHistory (
                    Id      int          NOT NULL IDENTITY(1,1),
                    Version varchar(32)  NOT NULL,
                    Name    varchar(128) NOT NULL,
                    DateUtc datetime     NOT NULL,
                    [User]  varchar(128) NOT NULL,

                    CONSTRAINT PK_Version PRIMARY KEY (Id)
                )
            ";
        }
    }
}
