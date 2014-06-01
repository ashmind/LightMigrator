using System;
using System.Collections.Generic;
using System.Linq;

// Requires AgentHeisenbug plugin for ReSharper

// ReSharper disable once CheckNamespace
namespace JetBrains.Annotations {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class ReadOnlyAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class ThreadSafeAttribute : Attribute {}
}
