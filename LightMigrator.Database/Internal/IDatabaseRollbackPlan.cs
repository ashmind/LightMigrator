namespace LightMigrator.Database.Internal {
    public interface IDatabaseRollbackPlan {
        void Prepare();
        void Rollback();
        void CommitFirstPhase();
        void CommitLastPhase();
    }
}