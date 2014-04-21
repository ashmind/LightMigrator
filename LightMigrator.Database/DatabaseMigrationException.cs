using System;
using System.Runtime.Serialization;
using LightMigrator.Running;

namespace LightMigrator.Database {
    [Serializable]
    public class DatabaseMigrationException : MigrationException {
        public DatabaseMigrationException() {}
        public DatabaseMigrationException(string message) : base(message) {}

        public DatabaseMigrationException(string message, string sql, Exception inner) : base(message) {
            this.Sql = sql;
        }

        public DatabaseMigrationException(string message, Exception inner) : base(message, inner) {}
        protected DatabaseMigrationException(SerializationInfo info, StreamingContext context) : base(info, context) {}

        public string Sql { get; private set; }
    }
}