using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LightMigrator.Framework.FluentInterface;

namespace LightMigrator.Engine {
    public class MigrationScope : IMigrationScope {
        [NotNull] private readonly IRollbackPlan _overallRollbackPlan;
        private bool _completed;

        public MigrationScope([NotNull] IDatabase database)
            : this(database.Name, new[] { database }, database.RollbackPlan) {
        }

        public MigrationScope([NotNull] string primaryDatabaseName, [NotNull] IEnumerable<IDatabase> databases, [NotNull] IRollbackPlan overallRollbackPlan) {
            PrimaryDatabaseName = Argument.NotNull("primaryDatabaseName", primaryDatabaseName);
            Databases = Argument.NotNull("databases", databases).ToDictionary(
                // ReSharper disable once PossibleNullReferenceException
                d => d.Name,
                d => (IDatabaseSyntax)d
            );
            _overallRollbackPlan = overallRollbackPlan;

            _overallRollbackPlan.Prepare();
        }

        public string PrimaryDatabaseName { get; private set; }
        public IReadOnlyDictionary<string, IDatabaseSyntax> Databases { get; private set; }
        
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
