using System;
using System.Collections.Generic;
using System.Linq;
using LightMigrator.Database.Internal;

namespace LightMigrator.Database.SqlServer {
    public class DefaultSqlServerVersionTableDefinition : IDatabaseVersionTableDefinition {
        public string SchemaName {
            get { return "dbo"; }
        }

        public string TableName {
            get { return "Version"; }
        }

        public string VersionColumnName {
            get { return "Version"; }
        }

        public string NameColumnName {
            get { return "Name"; }
        }

        public string DateColumnName {
            get { return "DateUtc"; }
        }

        public string UserColumnName {
            get { return "User"; }
        }

        public string CreateScript {
            get { return @"
                CREATE TABLE dbo.Version (
                    Id      int          NOT NULL IDENTITY(1,1),
                    Version varchar(32)  NOT NULL,
                    Name    varchar(128) NOT NULL,
                    DateUtc datetime     NOT NULL,
                    [User]  varchar(128) NOT NULL,

                    CONSTRAINT PK_Version PRIMARY KEY (Id)
                )
            "; }
        }
    }
}
