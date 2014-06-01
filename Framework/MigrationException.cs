using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace LightMigrator.Framework {
    [PublicAPI]
    [Serializable]
    public class MigrationException : Exception {
        public MigrationException() {}
        public MigrationException(string message) : base(message) {}
        public MigrationException(string message, string sql, Exception inner) : base(message, inner) {
            this.Sql = sql;
        }

        public MigrationException(string message, Exception inner) : base(message, inner) {}
        protected MigrationException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context) {}

        public string Sql { get; private set; }
    }
}