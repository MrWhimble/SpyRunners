using System;
using UnityEngine;

namespace SpyRunners.Player
{
    public class PlayerMovement : MonoBehaviour, IDependent
    {
        [SerializeField] private float _speed;
        
        private PlayerCharacter _playerCharacter;
        private Rigidbody _rigidbody;
        private PlayerInputManager _playerInputManager;
        
        private bool _subscribed = false;
        
        private void Awake()
        {
            _playerCharacter = GetComponent<PlayerCharacter>();
            _playerCharacter.AddDependent(this);
            _rigidbody = GetComponent<Rigidbody>();
        }
        
        public void Initialize()
        {
            
        }

        public void SubscribeToEvents()
        {
            if (_subscribed)
                return;
            
            _playerInputManager = _playerCharacter.PlayerManager.Dependencies[typeof(PlayerInputManager)] as PlayerInputManager;
            if (!_playerInputManager)
                throw new System.NullReferenceException("PlayerInputManager is null");
            _playerInputManager.JumpButton.Pressed += OnJumpPressed;

            _subscribed = true;
        }

        private void FixedUpdate()
        {
            transform.Translate(new Vector3(_playerInputManager.MoveInput.x, 0, _playerInputManager.MoveInput.y) * _speed * Time.fixedDeltaTime);
        }

        private void OnJumpPressed()
        {
            
        }

        public void CleanUp()
        {
            
        }

        public void UnsubscribeFromEvents()
        {
            if (!_subscribed)
                return;
            
            _playerInputManager.JumpButton.Pressed -= OnJumpPressed;
            _playerInputManager = null;
            
            _subscribed = false;
        }

        public void Finish()
        {
            
        }
    }
}