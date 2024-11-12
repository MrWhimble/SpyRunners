using SpyRunners.Managers;
using UnityEngine;

namespace SpyRunners.Player
{
    public class PlayerCursorManager : MonoBehaviour, IDependent
    {
        [SerializeField] private CursorData _cursorData;
        [SerializeField] private int _cursorPriority;

        private PlayerCharacter _playerCharacter;
        
        private int _cursorId;
        
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
            
            _cursorId = IdManager.GetId();
            CursorManager.Cursors.Add(_cursorId, _cursorPriority, _cursorData);

            _isInitialized = true;
        }

        public void SubscribeToEvents()
        {
            if (_isSubscribed)
                return;
            
            

            _isSubscribed = true;
        }

        public void CleanUp()
        {
            if (_isCleanedUp)
                return;
            
            if (CursorManager.Cursors != null)
                CursorManager.Cursors.Remove(_cursorId);

            _isCleanedUp = true;
        }

        public void UnsubscribeFromEvents()
        {
            if (_isSubscribed)
                return;

            _isSubscribed = true;
        }

        public void Finish()
        {
            
        }
    }
}