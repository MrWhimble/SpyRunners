﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpyRunners.Player
{
    public class PlayerCharacter : MonoBehaviour, IDependent, IDependency
    {
        [HideInInspector]
        public PlayerManager PlayerManager;
        
        private Dictionary<Type, IDependent> _dependencies = new ();

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
            
            foreach (var dependent in _dependencies.Values) 
                dependent.Initialize();
            foreach (var dependent in _dependencies.Values) 
                dependent.SubscribeToEvents();

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
            
            foreach (var dependent in _dependencies.Values) 
                dependent.CleanUp();
            foreach (var dependent in _dependencies.Values) 
                dependent.UnsubscribeFromEvents();
            foreach (var dependent in _dependencies.Values) 
                dependent.Finish();
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
            
            Destroy(gameObject);

            _isFinished = true;
        }
    }
}