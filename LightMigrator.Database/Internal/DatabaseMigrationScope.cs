using System;
using System.Collections.Generic;
using System.Linq;
using LightMigrator.Running;

namespace LightMigrator.Database.Internal {
    public class DatabaseMigrationScope : IDatabaseMigrationContext, IMigrationScope {
        private readonly IDatabaseRollbackPlan _overallRollbackPlan;
        private bool _completed;

        public DatabaseMigrationScope(IDatabase database)
            : this(database.Name, new[] { database }, database.RollbackPlan) {
        }

        public DatabaseMigrationScope(string primaryDatabaseName, IEnumerable<IDatabase> databases, IDatabaseRollbackPlan overallRollbackPlan) {
            PrimaryDatabaseName = primaryDatabaseName;
            Databases = databases.ToDictionary(d => d.Name);
            _overallRollbackPlan = overallRollbackPlan;

            _overallRollbackPlan.Prepare();
        }

        public string PrimaryDatabaseName { get; private set; }
        public IReadOnlyDictionary<string, IDatabase> Databases { get; private set; }
        
        public void Complete() {
            _completed = true;
        }
        
        public void Dispose() {
            if (!_completed) {
                _overallRollbackPlan.Rollback();
                return;
            }

            try {
                _overallRollbackPlan.CommitFirstPhase();
            }
            catch (Exception commitException) {
                try {
                    _overallRollbackPlan.Rollback();
                }
                catch (AggregateException rollbackException) {
                    throw new AggregateException(new[] { commitException }.Concat(rollbackException.InnerExceptions));
                }
                catch (Exception rollbackException) {
                    throw new AggregateException(commitException, rollbackException);
                }
                throw;
            }

            _overallRollbackPlan.CommitLastPhase();
        }
    }
}
