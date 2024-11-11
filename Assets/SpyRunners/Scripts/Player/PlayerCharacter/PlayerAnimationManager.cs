using UnityEngine;

namespace SpyRunners.Player
{
    public class PlayerAnimationManager : MonoBehaviour, IDependent
    {
        private PlayerCharacter _playerCharacter;
        private Animator _animator;
        private PlayerInputManager _playerInputManager;
        private PlayerMovement _playerMovement;
        
        private bool _isInitialized = false;
        private bool _isSubscribed = false;
        private bool _isCleanedUp = false;
        private bool _isFinished = false;
        
        private void Awake()
        {
            _playerCharacter = GetComponent<PlayerCharacter>();
            _playerCharacter.AddDependent(this);
            _animator = GetComponent<Animator>();
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
            
            _playerInputManager = _playerCharacter.PlayerManager.Dependencies[typeof(PlayerInputManager)] as PlayerInputManager;
            if (!_playerInputManager)
                throw new System.NullReferenceException("PlayerInputManager is null");
            _playerInputManager.JumpButton.Pressed += OnJump;
            _playerMovement = _playerCharacter.Dependencies[typeof(PlayerMovement)] as PlayerMovement;
            if (!_playerMovement)
                throw new System.NullReferenceException("PlayerMovement is null");

            _isSubscribed = true;
        }
        
        private void Update()
        {
            //_animator.SetBool("Slide", _playerInputManager.SlideButton.Held);
            //_animator.SetFloat("InputX", _playerInputManager.MoveInput.x);
            //_animator.SetFloat("InputY", _playerInputManager.MoveInput.y);
        }

        private void OnJump()
        {
            //_animator.SetTrigger("Jump");
        }

        public void CleanUp()
        {
            if (_isCleanedUp)
                return;
            
            

            _isCleanedUp = true;
        }

        public void UnsubscribeFromEvents()
        {
            if (_isSubscribed)
                return;

            _playerInputManager = null;
            _playerMovement = null;

            _isSubscribed = true;
        }

        public void Finish()
        {
            if (_isFinished)
                return;
            
            _isFinished = true;
        }
    }
}