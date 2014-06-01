﻿using System.Collections.Generic;
using JetBrains.Annotations;
using LightMigrator.Framework.Conventions;

namespace LightMigrator.Running {
    public interface IVersionRepository {
        void Prepare();

        // ReSharper disable once ReturnTypeCanBeEnumerable.Global
        [NotNull] IReadOnlyCollection<string> GetAllVersions();
        void SaveVersion([NotNull] MigrationInfo migrationInfo);
    }
}