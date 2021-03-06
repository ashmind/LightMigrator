﻿using System;
using JetBrains.Annotations;
using LightMigrator.Framework.FluentInterface;

namespace LightMigrator.SqlServer {
    public class SqlServerCollection<T> : ICollectionSyntax<T> {
        [NotNull] private readonly Func<string, T> _syntaxFactory;

        public SqlServerCollection([NotNull] Func<string, T> syntaxFactory) {
            _syntaxFactory = Argument.NotNull("syntaxFactory", syntaxFactory);
        }

        public T this[string name] {
            get {
                // ReSharper disable once AssignNullToNotNullAttribute
                return _syntaxFactory(name);
            }
        }
    }
}