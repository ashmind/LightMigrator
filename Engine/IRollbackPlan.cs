namespace LightMigrator.Engine {
    public interface IRollbackPlan {
        void Prepare();
        void Rollback();
        void CommitFirstPhase();
        void CommitLastPhase();
    }
}