using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpyRunners.Player
{
    public class PlayerManager : MonoBehaviour, IDependency
    {
        private readonly Dictionary<Type, IDependent> _dependencies = new ();
        public IReadOnlyDictionary<Type, IDependent> Dependencies => _dependencies;
        
        [SerializeField] private PlayerCharacter _playerCharacterPrefab;

        public void AddDependent(IDependent dependent)
        {
            if (_dependencies.ContainsKey(dependent.GetType()))
                return;
            
            _dependencies.Add(dependent.GetType(), dependent);
            dependent.Initialize();
            dependent.SubscribeToEvents();
        }

        public void RemoveDependent(IDependent dependent)
        {
            if (!_dependencies.ContainsKey(dependent.GetType()))
                return;
            
            dependent.CleanUp();
            dependent.UnsubscribeFromEvents();
            dependent.Finish();
            _dependencies.Remove(dependent.GetType());
        }

        private void Start()
        {
            foreach (var dependent in _dependencies.Values) 
                dependent.Initialize();
            foreach (var dependent in _dependencies.Values) 
                dependent.SubscribeToEvents();

            SpawnPlayerCharacter();
        }

        private void SpawnPlayerCharacter()
        {
            PlayerCharacter playerCharacter = Instantiate(_playerCharacterPrefab);
            playerCharacter.PlayerManager = this;
            AddDependent(playerCharacter);
        }

        private void OnDestroy()
        {
            foreach (var dependent in _dependencies.Values) 
                dependent.CleanUp();
            foreach (var dependent in _dependencies.Values) 
                dependent.UnsubscribeFromEvents();
            foreach (var dependent in _dependencies.Values) 
                dependent.Finish();
            _dependencies.Clear();
        }
    }
}