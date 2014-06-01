using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using JetBrains.Annotations;

namespace LightMigrator.Running {
    public class TransactionScopeRollbackPlan : IRollbackPlan {
        [NotNull] private TransactionScope _scope;

        public void Prepare() {
            _scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions {
                IsolationLevel = IsolationLevel.ReadCommitted
            });
        }

        public void Rollback() {
            _scope.Dispose();
        }

        public void CommitFirstPhase() {
            // obviously that would not work so well for naive multi-database,
            // though if MSDTC is enabled it might be OK
        }

        public void CommitLastPhase() {
            try {
                _scope.Complete();
            }
            finally {
                _scope.Dispose();
            }
        }
    }
}
