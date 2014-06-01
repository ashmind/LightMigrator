using System;
using LightMigrator.Framework;

namespace LightMigrator.Engine {
    public interface IMigrationScope : IMigrationContext, IDisposable {
        void Complete();
    }
}