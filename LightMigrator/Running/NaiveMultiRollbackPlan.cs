using System;
using System.Collections.Generic;
using System.Linq;
using AshMind.Extensions;
using JetBrains.Annotations;
using LightMigrator.Running;

namespace LightMigrator.Database.Internal {
    [PublicAPI]
    public class NaiveMultiRollbackPlan : IRollbackPlan {
        private readonly IEnumerable<IRollbackPlan> _rollbackPlans;

        public NaiveMultiRollbackPlan(IEnumerable<IDatabase> databases)
            : this(databases.Select(d => d.RollbackPlan))
        {
        }

        public NaiveMultiRollbackPlan(IEnumerable<IRollbackPlan> rollbackPlans) {
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