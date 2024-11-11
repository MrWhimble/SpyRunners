using System;
using System.Collections.Generic;

namespace SpyRunners
{
    public static class IDependencyUtils
    {
        public static void AddDependencyAndCallMethods(IDependent dependent, Dictionary<Type, IDependent> dependencies)
        {
            if (dependencies.ContainsKey(dependent.GetType()))
                return;
            
            dependencies.Add(dependent.GetType(), dependent);
            dependent.Initialize();
            dependent.SubscribeToEvents();
        }

        public static void RemoveDependencyAndCallMethods(IDependent dependent, Dictionary<Type, IDependent> dependencies)
        {
            if (dependencies.ContainsKey(dependent.GetType()))
                return;
            
            dependent.CleanUp();
            dependent.UnsubscribeFromEvents();
            dependent.Finish();
            dependencies.Remove(dependent.GetType());
        }

        /// <summary>
        /// Shortcut for called all start methods in dependents
        /// </summary>
        /// <example>
        /// <code>
        /// foreach (var dependent in dependencies.Values)<br/>
        ///     dependent.Initialize();<br/>
        /// foreach (var dependent in dependencies.Values)<br/>
        ///     dependent.SubscribeToEvents();
        /// </code>
        /// </example>
        public static void CallDependentStartMethods(Dictionary<Type, IDependent> dependencies)
        {
            foreach (var dependent in dependencies.Values) 
                dependent.Initialize();
            foreach (var dependent in dependencies.Values) 
                dependent.SubscribeToEvents();
        }

        /// <summary>
        /// Shortcut for called all final methods in dependents
        /// </summary>
        /// <example>
        /// <code>
        /// foreach (var dependent in dependencies.Values) 
        ///     dependent.CleanUp();
        /// foreach (var dependent in dependencies.Values) 
        ///     dependent.UnsubscribeFromEvents();
        /// foreach (var dependent in dependencies.Values) 
        ///     dependent.Finish();
        /// </code>
        /// </example>
        public static void CallDependentFinalMethods(Dictionary<Type, IDependent> dependencies)
        {
            foreach (var dependent in dependencies.Values) 
                dependent.CleanUp();
            foreach (var dependent in dependencies.Values) 
                dependent.UnsubscribeFromEvents();
            foreach (var dependent in dependencies.Values) 
                dependent.Finish();
        }
    }
}