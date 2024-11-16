using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpyRunners.Player
{
    public class PlayerPowerupManager : MonoBehaviour, IDependent
    {
        public PowerupEffect currentPowerup;
        private bool powerUpActive = false;
        private bool _isSubscribed = false;
        private bool _isCleanedUp = false;
        private bool _isInitialized = false;
        private PlayerInputManager _playerInputManager;
        private PlayerCharacter _playerCharacter;

        private void Awake()
        {
            _playerCharacter = GetComponent<PlayerCharacter>();
            _playerCharacter.AddDependent(this);
        }
        public void AddPowerup(PowerupEffect newpowerUp)
        { 
            if (newpowerUp == currentPowerup)
                return;
            
            currentPowerup = newpowerUp; 

            if (currentPowerup.instant == true)
            { 
                StartCoroutine(Duration(currentPowerup.duration)); 
                return;
            } 
            //UI UPDATE
        } 

        private void ActivatePowerup()
        {
            if (powerUpActive)
                return;

            if (currentPowerup == null)
                return;

            currentPowerup.Apply(_playerCharacter.gameObject);
            StartCoroutine(Duration(currentPowerup.duration));
        }

        public IEnumerator Duration(float duration)
        {
            powerUpActive = true;
            yield return new WaitForSeconds(duration);
            currentPowerup.Remove(gameObject);
            currentPowerup = null;
        }

        public void SubscribeToEvents()
        {
            if (_isSubscribed)
                return;

            _playerInputManager = _playerCharacter.PlayerManager.Dependencies[typeof(PlayerInputManager)] as PlayerInputManager;

            if (!_playerInputManager)
                throw new System.NullReferenceException("PlayerInputManager is null");
            _playerInputManager.PowerupButton.Pressed += ActivatePowerup; 

            _isSubscribed = true;
        }

        public void Initialize()
        {
            if (_isInitialized)
                return;

            _isInitialized = true;
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

            _playerInputManager.PowerupButton.Pressed -= ActivatePowerup;
         //   _playerInputManager.PowerupButton.Released -= ActivatePowerup;
            _playerInputManager = null;

            _isSubscribed = false;
        }

        public void Finish()
        {
           // throw new System.NotImplementedException();
        }
    }
}