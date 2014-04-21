﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LightMigrator.Database.FluentInterface;
using LightMigrator.Database.Internal;
using LightMigrator.Running;

namespace LightMigrator.Database.SqlServer {
    public class SqlServerVersionRepository : IVersionRepository {
        [NotNull] private readonly IDatabase _database;
        [NotNull] private readonly IDatabaseVersionTableDefinition _tableDefinition;
        [NotNull] private readonly ITableSyntax _table;

        public SqlServerVersionRepository([NotNull] IDatabase database, [NotNull] IDatabaseVersionTableDefinition tableDefinition) {
            _database = Argument.NotNull("database", database);
            _tableDefinition = Argument.NotNull("tableDefinition", tableDefinition);

            _table = _database.Schemas[_tableDefinition.SchemaName].Tables[_tableDefinition.TableName];
        }

        public void Prepare() {
            if (!_table.Schema.Exists())
                _table.Schema.Create();

            if (_table.Exists())
                return;

            // TODO: implement CREATE TABLE through fluent interface
            _database.ExecuteScript(_tableDefinition.CreateScript);
        }

        public IReadOnlyCollection<string> GetAllVersions() {
            // ReSharper disable once PossibleNullReferenceException
            return _database.ExecuteReader("SELECT " + _tableDefinition.VersionColumnName + " FROM " + _table.FullNameEscaped, reader => reader.GetString(0)).ToList().AsReadOnly();
        }

        public void SaveVersion(PlannedMigration migration) {
            Argument.NotNull("migration", migration);

            var columnsAndValues = new Dictionary<string, string> {{_tableDefinition.VersionColumnName, "@Version"}};
            if (_tableDefinition.NameColumnName != null)
                columnsAndValues.Add(_tableDefinition.NameColumnName, "@Name");

            if (_tableDefinition.DateColumnName != null)
                columnsAndValues.Add(_tableDefinition.DateColumnName, "GETUTCDATE()");

            if (_tableDefinition.UserColumnName != null)
                columnsAndValues.Add(_tableDefinition.UserColumnName, "SYSTEM_USER");

            var columnsSql = string.Join(", ", columnsAndValues.Keys.Select(k => "[" + k + "]"));
            var valuesSql = string.Join(", ", columnsAndValues.Values);
            var sql = "INSERT INTO " + _table.FullNameEscaped + " ( " + columnsSql + " ) VALUES ( " + valuesSql + " )";
            
            _database.ExecuteNonQuery(sql, new { migration.GetType().Name, migration.Version });
        }
    }
}