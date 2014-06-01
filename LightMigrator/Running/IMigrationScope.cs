using System;
using LightMigrator.Framework;

namespace LightMigrator.Running {
    public interface IMigrationScope : IMigrationContext, IDisposable {
        void Complete();
    }
}