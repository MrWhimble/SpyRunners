using UnityEngine;
using UnityEngine.InputSystem;

namespace SpyRunners.Player
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputManager : MonoBehaviour, IDependent
    {
        private PlayerManager _playerManager;
        private PlayerInput _playerInput;

        private const string MoveId = "Move";
        public Vector2 MoveInput { get; private set; }
        private const string LookId = "Look";
        public Vector2 LookInput { get; private set; }
        public InputButton JumpButton { get; } = new("Jump");
        public InputButton SlideButton { get; } = new("Slide");
        public InputButton GrappleButton { get; }= new("Grapple");

        private bool _isSubscribed = false;
        
        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            
            _playerManager = GetComponent<PlayerManager>();
            _playerManager.AddDependent(this);
        }

        public void Initialize()
        {
            
        }

        public void SubscribeToEvents()
        {
            if (_isSubscribed)
                return;
            
            _playerInput.actions[MoveId].started += OnMove;
            _playerInput.actions[MoveId].performed += OnMove;
            _playerInput.actions[MoveId].canceled += OnMove;
            
            _playerInput.actions[LookId].started += OnLook;
            _playerInput.actions[LookId].performed += OnLook;
            _playerInput.actions[LookId].canceled += OnLook;
            
            JumpButton.SubscribeToInputs(_playerInput);
            SlideButton.SubscribeToInputs(_playerInput);
            GrappleButton.SubscribeToInputs(_playerInput);

            _isSubscribed = true;
        }
        
        private void OnMove(InputAction.CallbackContext context) => MoveInput = context.ReadValue<Vector2>();
        private void OnLook(InputAction.CallbackContext context) => LookInput = context.ReadValue<Vector2>();

        public void CleanUp()
        {
            
        }

        public void UnsubscribeFromEvents()
        {
            if (!_isSubscribed)
                return;
            
            _playerInput.actions[MoveId].started -= OnMove;
            _playerInput.actions[MoveId].performed -= OnMove;
            _playerInput.actions[MoveId].canceled -= OnMove;
            
            _playerInput.actions[LookId].started -= OnLook;
            _playerInput.actions[LookId].performed -= OnLook;
            _playerInput.actions[LookId].canceled -= OnLook;
            
            JumpButton.UnsubscribeFromInputs();
            SlideButton.UnsubscribeFromInputs();
            GrappleButton.UnsubscribeFromInputs();

            _isSubscribed = false;
        }
        
        public void Finish()
        {
            
        }
    }
}

