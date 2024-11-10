using System;
using System.Collections.Generic;

namespace SpyRunners
{
    /// <summary>
    /// An object that relies on <see cref="IDependency"/>
    /// </summary>
    public interface IDependent
    {
        /// <summary>
        /// <para>
        /// Called during Start() before <see cref="SubscribeToEvents"/><br/>
        /// Should be used to call <see cref="Initialize"/>, and <see cref="SubscribeToEvents"/> on dependents, etc
        /// </para>
        /// </summary>
        void Initialize();
        
        /// <summary>
        /// <para>
        /// Called during Start()<br/>
        /// Should subscribe to any events in any dependencies
        /// </para>
        /// </summary>
        void SubscribeToEvents();
        
        /// <summary>
        /// <para>
        /// Is called before <see cref="UnsubscribeFromEvents"/> and <see cref="Finish"/><br/>
        /// Should be used to remove any created objects, call <see cref="CleanUp"/>, <see cref="UnsubscribeFromEvents"/>, and <see cref="Finish"/> on dependents, etc
        /// </para>
        /// </summary>
        void CleanUp();
        
        /// <summary>
        /// <para>
        /// Is called before <see cref="Finish"/><br/>
        /// Should unsubscribe from any events in any dependencies
        /// </para>
        /// </summary>
        void UnsubscribeFromEvents();
        
        /// <summary>
        /// <para>
        /// Is the last method called by dependencies
        /// </para>
        /// </summary>
        void Finish();
    }
}