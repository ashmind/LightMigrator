using System.Collections.Generic;
using System.Reflection;

namespace LightMigrator.Running {
    public interface IMigrationRunner {
        void RunAll(params Assembly[] assemblies);
        void RunAll(IEnumerable<IMigration> migrations);
    }
}