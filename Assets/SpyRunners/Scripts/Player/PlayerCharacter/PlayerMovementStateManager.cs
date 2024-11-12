﻿using UnityEngine;

namespace SpyRunners.Player
{
    public class PlayerMovementStateManager : MonoBehaviour, IDependent
    {
        private PlayerMovementStates _previousState;
        public PlayerMovementStates PreviousState => _previousState;
        private PlayerMovementStates _currentState;

        public PlayerMovementStates CurrentState
        {
            get => _currentState;
            set
            {
                _previousState = _currentState;
                _currentState = value;
            }
        }

        public void RevertState()
        {
            CurrentState = _previousState;
        }

        private void Awake()
        {
            PlayerCharacter playerCharacter = GetComponent<PlayerCharacter>();
            playerCharacter.AddDependent(this);
        }

        public void Initialize() {}

        public void SubscribeToEvents() {}

        public void CleanUp() {}

        public void UnsubscribeFromEvents() {}

        public void Finish() {}
    }
}