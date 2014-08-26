using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
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
        }

        [NotNull]
        public virtual string GetCreateScript() {
            var script = new StringBuilder();
            script.AppendFormat("CREATE TABLE [{0}].[{1}] (", SchemaName, TableName).AppendLine();
            script.AppendLine("    Id int NOT NULL IDENTITY(1,1) CONSTRAINT PK_Version PRIMARY KEY,");
            script.AppendFormat("    [{0}] nvarchar(32) NOT NULL", VersionColumnName).AppendLine();

            if (NameColumnName != null)
                script.Append(",").AppendLine().AppendFormat("    [{0}] nvarchar(32) NOT NULL", NameColumnName).AppendLine();

            if (DateColumnName != null)
                script.Append(",").AppendLine().AppendFormat("    [{0}] datetime NOT NULL", DateColumnName).AppendLine();

            if (UserColumnName != null)
                script.Append(",").AppendLine().AppendFormat("    [{0}] nvarchar(128) NOT NULL", UserColumnName).AppendLine();

            script.AppendLine().Append(")");

            return script.ToString();
        }
    }
}
