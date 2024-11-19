using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpyRunners.Player
{
    public class PlayerCharacter : MonoBehaviour, IDependent, IDependency
    {
        [HideInInspector]
        public PlayerManager PlayerManager;
        
        private Dictionary<Type, IDependent> _dependencies = new ();
        public IReadOnlyDictionary<Type, IDependent> Dependencies => _dependencies;

        private bool _isInitialized = false;
        private bool _isSubscribed = false;
        private bool _isCleanedUp = false;
        private bool _isFinished = false;
        
        public void AddDependent(IDependent dependent)
        {
            _dependencies.Add(dependent.GetType(), dependent);
        }

        public void RemoveDependent(IDependent dependent)
        {
            _dependencies.Remove(dependent.GetType());
        }

        public void Initialize()
        {
            if (_isInitialized)
                return;

            foreach (var kv in _dependencies)
            {
                Debug.Log(kv.Key.ToString());
            }
            
            IDependencyUtils.CallDependentStartMethods(_dependencies);

            _isInitialized = true;
        }

        public void SubscribeToEvents()
        {
            if (_isSubscribed)
                return;

            

            _isSubscribed = true;
        }

        public void CleanUp()
        {
            if (_isCleanedUp)
                return;
            
            IDependencyUtils.CallDependentFinalMethods(_dependencies);
            _dependencies.Clear();

            _isCleanedUp = true;
        }

        public void UnsubscribeFromEvents()
        {
            if (!_isSubscribed)
                return;



            _isSubscribed = false;
        }
        
        public void Finish()
        {
            if (_isFinished)
                return; 

            if (this && gameObject)
                Destroy(gameObject);

            _isFinished = true;
        }
    }
}