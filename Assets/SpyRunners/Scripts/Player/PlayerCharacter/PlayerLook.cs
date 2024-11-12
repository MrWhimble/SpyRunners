using SpyRunners.Managers;
using UnityEngine;

namespace SpyRunners.Player
{
    public class PlayerLook : MonoBehaviour, IDependent
    {
        [SerializeField] private CameraData _cameraData;
        [SerializeField] private int _cameraPriority;
        [Space] 
        [SerializeField] private Transform _yawTransform;
        [SerializeField] private Transform _pitchTransform;
        [SerializeField] private float _minPitch;
        [SerializeField] private float _maxPitch;
        [Space]
        [SerializeField] private float _yawSensitivity = 0.2f;
        [SerializeField] private float _pitchSensitivity = 0.2f;
        
        private PlayerCharacter _playerCharacter;
        private PlayerInputManager _playerInputManager;

        private int _cameraId;

        private float _cameraPitch;
        private float _cameraYaw;

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
            
            _cameraId = IdManager.GetId();
            CameraManager.Cameras.Add(_cameraId, _cameraPriority, _cameraData);

            _isInitialized = true;
        }

        public void SubscribeToEvents()
        {
            if (_isSubscribed)
                return;
            
            _playerInputManager = _playerCharacter.PlayerManager.Dependencies[typeof(PlayerInputManager)] as PlayerInputManager;
            if (!_playerInputManager)
                throw new System.NullReferenceException("PlayerInputManager is null");

            _isSubscribed = true;
        }

        private void Update()
        {
            _cameraYaw += _playerInputManager.LookInput.x * _yawSensitivity;
            
            _cameraPitch -= _playerInputManager.LookInput.y * _pitchSensitivity;
            _cameraPitch = Mathf.Clamp(_cameraPitch, _minPitch, _maxPitch);
            
            _yawTransform.localEulerAngles = new Vector3(0, _cameraYaw, 0);
            _pitchTransform.localEulerAngles = new Vector3(_cameraPitch, 0, 0);
        }

        public void CleanUp()
        {
            if (_isCleanedUp)
                return;
            
            if (CameraManager.Cameras != null)
                CameraManager.Cameras.Remove(_cameraId);

            _isCleanedUp = true;
        }

        public void UnsubscribeFromEvents()
        {
            if (!_isSubscribed)
                return;

            _playerInputManager = null;

            _isSubscribed = false;
        }

        public void Finish()
        {
            
        }
    }
}