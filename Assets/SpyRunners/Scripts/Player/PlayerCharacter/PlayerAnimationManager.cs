using UnityEngine;

namespace SpyRunners.Player
{
    public class PlayerAnimationManager : MonoBehaviour, IDependent
    {
        private PlayerCharacter _playerCharacter;
        [SerializeField] private Animator _animator;
        private PlayerInputManager _playerInputManager;
        private PlayerMovement _playerMovement;
        private PlayerMovementStateManager _playerMovementStateManager;
        
        private bool _isInitialized = false;
        private bool _isSubscribed = false;
        private bool _isCleanedUp = false;
        private bool _isFinished = false;
        
        private void Awake()
        {
            _playerCharacter = GetComponent<PlayerCharacter>();
            _playerCharacter.AddDependent(this);
            //_animator = GetComponent<Animator>();
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
            _playerMovementStateManager = _playerCharacter.Dependencies[typeof(PlayerMovementStateManager)] as PlayerMovementStateManager;
            if (!_playerMovementStateManager)
                throw new System.NullReferenceException("PlayerMovementStateManager is null");
            _playerMovementStateManager.StateChanged += OnStateChanged;

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

        private void OnStateChanged(PlayerMovementStates previousState, PlayerMovementStates currentState)
        {
            if (previousState == currentState)
                return; 

            _animator.ResetTrigger(previousState.ToString());
            _animator.SetTrigger(currentState.ToString());
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

            _playerInputManager.JumpButton.Pressed -= OnJump;
            _playerInputManager = null;
            _playerMovement = null;
            _playerMovementStateManager.StateChanged -= OnStateChanged;
            _playerMovementStateManager = null;
            
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