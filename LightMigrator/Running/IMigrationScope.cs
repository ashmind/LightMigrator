using System;

namespace LightMigrator.Running {
    public interface IMigrationScope : IMigrationContext, IDisposable {
        void Complete();
    }
}