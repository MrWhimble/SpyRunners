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
            IDependencyUtils.AddDependencyAndCallMethods(dependent, _dependencies);
        }

        public void RemoveDependent(IDependent dependent)
        {
            IDependencyUtils.RemoveDependencyAndCallMethods(dependent, _dependencies);
        }

        private void Start()
        {
            IDependencyUtils.CallDependentStartMethods(_dependencies);

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
            IDependencyUtils.CallDependentFinalMethods(_dependencies);
            _dependencies.Clear();
        }
    }
}