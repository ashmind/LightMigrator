using System;
using System.Collections.Generic;
using System.Linq;
using AshMind.Extensions;
using JetBrains.Annotations;

namespace LightMigrator.Database.Internal {
    [PublicAPI]
    public class NaiveMultiDatabaseRollbackPlan : IDatabaseRollbackPlan {
        private readonly IEnumerable<IDatabaseRollbackPlan> _rollbackPlans;

        public NaiveMultiDatabaseRollbackPlan(IEnumerable<IDatabase> databases)
            : this(databases.Select(d => d.RollbackPlan))
        {
        }

        public NaiveMultiDatabaseRollbackPlan(IEnumerable<IDatabaseRollbackPlan> rollbackPlans) {
            _rollbackPlans = rollbackPlans.AsReadOnlyList();
        }
        
        public void Prepare() {
            foreach (var plan in _rollbackPlans) {
                plan.Prepare();
            }
        }

        public void Rollback() {
            var exceptions = new List<Exception>();
            foreach (var plan in _rollbackPlans) {
                try {
                    plan.Rollback();
                }
                catch (Exception ex) {
                    exceptions.Add(ex);
                }
            }
            if (exceptions.Any())
                throw new AggregateException(exceptions);
        }

        public void CommitFirstPhase() {
            foreach (var plan in _rollbackPlans) {
                plan.CommitFirstPhase();
            }
        }

        public void CommitLastPhase() {
            foreach (var plan in _rollbackPlans) {
                plan.CommitLastPhase();
            }
        }
    }
}