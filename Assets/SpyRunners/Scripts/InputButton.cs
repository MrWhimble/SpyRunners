using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SpyRunners
{
    public class InputButton
    {
        public bool Held { get; private set; }
        public event Action Pressed;
        public event Action Released;

        private PlayerInput _playerInput;
        private string _inputName;
        private bool _subscribed = false;

        public InputButton(string inputName)
        {
            _inputName = inputName;
        }
        
        public void SubscribeToInputs(PlayerInput playerInput)
        {
            if (_subscribed)
                return;

            _playerInput = playerInput;
            
            if (_playerInput == null)
                throw new NullReferenceException("PlayerInput is null");
            if (_playerInput.actions == null)
                throw new NullReferenceException("PlayerInput.actions is null");
            _playerInput.actions.FindAction(_inputName, true);
            
            _playerInput.actions[_inputName].started += OnPressed;
            _playerInput.actions[_inputName].performed += OnHeld;
            _playerInput.actions[_inputName].canceled += OnReleased;
            
            _subscribed = true;
        }

        public void UnsubscribeFromInputs()
        {
            if (!_subscribed)
                return;
            
            _playerInput.actions[_inputName].started -= OnPressed;
            _playerInput.actions[_inputName].performed -= OnHeld;
            _playerInput.actions[_inputName].canceled -= OnReleased;

            _playerInput = null;
            _inputName = string.Empty;
            _subscribed = false;
        }
        
        private void OnPressed(InputAction.CallbackContext context)
        {
            Debug.Log($"Pressed: {_inputName}");
            Pressed?.Invoke();
            Held = true;
        }

        private void OnHeld(InputAction.CallbackContext context) => Held = true;

        private void OnReleased(InputAction.CallbackContext context)
        {
            Held = false;
            Released?.Invoke();
        }
    }
}