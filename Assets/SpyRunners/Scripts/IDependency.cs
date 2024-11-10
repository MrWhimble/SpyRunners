using System;
using System.Collections.Generic;

namespace SpyRunners
{
    /// <summary>
    /// An object that an <see cref="IDependent"/> relies on
    /// </summary>
    public interface IDependency
    { 
        void AddDependent(IDependent dependent);
        void RemoveDependent(IDependent dependent);
    }
}