using System;
using UnityEngine;

namespace SpyRunners.Player
{
    public class PlayerCheckpointManager : MonoBehaviour, IDependent
    {
        private PlayerCharacter _playerCharacter;
        private PlayerHealth _playerHealth;

        private Transform _latestCheckpoint;
        
        private bool _isInitialized = false;
        private bool _isSubscribed = false;
        private bool _isCleanedUp = false;
        private bool _isFinished = false;

        private void Awake()
        {
            _playerCharacter = GetComponent<PlayerCharacter>();
            _playerCharacter.AddDependent(this);
        }
        
        public void Initialize()
        {
            if (_isInitialized)
                return;
            
            

            _isInitialized = true;
        }

        public void SubscribeToEvents()
        {
            if (_isSubscribed)
                return;
            
            _playerHealth = _playerCharacter.Dependencies[typeof(PlayerHealth)] as PlayerHealth;
            if (!_playerHealth)
                throw new System.NullReferenceException("PlayerHealth is null");
            _playerHealth.Died += OnDied;

            _isSubscribed = true;
        }

        private void OnDied(PlayerCharacter playerCharacter)
        {
            if (playerCharacter != _playerCharacter)
                return;
            
            if (_latestCheckpoint == null)
                throw new System.NullReferenceException("_latestCheckpoint is null");
            
            transform.position = _latestCheckpoint.position;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Checkpoint"))
                return;
            
            _latestCheckpoint = other.transform;
        }

        public void CleanUp()
        {
            if (_isCleanedUp)
                return;
            
            

            _isCleanedUp = true;
        }

        public void UnsubscribeFromEvents()
        {
            if (!_isSubscribed)
                return;

            _playerHealth.Died -= OnDied;
            _playerHealth = null;

            _isSubscribed = false;
        }

        public void Finish()
        {
            if (_isFinished)
                return;



            _isFinished = true;
        }
    }
}