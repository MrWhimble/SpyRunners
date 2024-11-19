using System.Collections;
using UnityEngine;

namespace SpyRunners.Player
{
    public class PlayerGrapple : MonoBehaviour, IDependent
    {
        [SerializeField] private GameObject _grappleVisualPrefab;
        [SerializeField] private Transform _grappleOrigin;
        [SerializeField] private float _maxDistanceFromGrapplePoint; // TODO Change to use angles
        [SerializeField] private float _maxDistanceFromOrigin;
        [SerializeField] private float _minDistanceFromOrigin;
        [SerializeField] private float _grappleThrowSpeed;
        [SerializeField] private float _pullingSpeed;
        [SerializeField] private float _grappleStrength;
        [SerializeField] private float _grappleDampening;
        
        private PlayerCharacter _playerCharacter;
        private Rigidbody _rigidbody;
        private PlayerMovementStateManager _playerMovementStateManager;
        private PlayerInputManager _playerInputManager;

        private GameObject _grappleVisual;
        
        private IGrappleTarget _grappleTarget;
        private Vector3 _localTargetPosition;
        
        private Coroutine _moveGrappleCoroutine;
        
        private float _distanceToTarget;
        
        private bool _isInitialized = false;
        private bool _isSubscribed = false;
        private bool _isCleanedUp = false;
        private bool _isFinished = false;
        
        private void Awake()
        {
            _playerCharacter = GetComponent<PlayerCharacter>();
            _playerCharacter.AddDependent(this);
            _rigidbody = GetComponent<Rigidbody>();
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
            
            _playerMovementStateManager = _playerCharacter.Dependencies[typeof(PlayerMovementStateManager)] as PlayerMovementStateManager;
            if (!_playerMovementStateManager)
                throw new System.NullReferenceException("PlayerMovementStateManager is null");
            _playerInputManager = _playerCharacter.PlayerManager.Dependencies[typeof(PlayerInputManager)] as PlayerInputManager;
            if (!_playerInputManager)
                throw new System.NullReferenceException("PlayerInputManager is null");
            _playerInputManager.GrappleButton.Pressed += OnStartGrapple;
            _playerInputManager.GrappleButton.Released += OnReleaseGrapple;
            

            _isSubscribed = true;
        }

        private void FixedUpdate()
        {
            if (_playerMovementStateManager.CurrentState is not PlayerMovementStates.Grappling)
                return;
            
            float deltaTime = Time.fixedDeltaTime;
            _distanceToTarget -= _pullingSpeed * deltaTime;
            Vector3 grappleWorldPositionTarget = _grappleTarget.TransformPoint(_localTargetPosition);
            if (Vector3.Distance(_grappleOrigin.position, grappleWorldPositionTarget) < _minDistanceFromOrigin)
            {
                OnReleaseGrapple();
                return;
            }
            
            Vector3 targetPositionInWorld = _grappleTarget.TransformPoint(_localTargetPosition);
            Vector3 direction = targetPositionInWorld - _rigidbody.worldCenterOfMass;
            float currentDistance = direction.magnitude;
            
            direction.Normalize();

            float directionVelocity = Vector3.Dot(direction, _rigidbody.velocity);
            float otherDirectionVelocity = Vector3.Dot(direction, _grappleTarget.GetPointVelocity(grappleWorldPositionTarget));

            float relativeVelocity = directionVelocity - otherDirectionVelocity;

            float distanceDelta = currentDistance - _distanceToTarget;

            float springForce = (distanceDelta * _grappleStrength) - (relativeVelocity * _grappleDampening);
			
            _rigidbody.AddForce(direction * springForce, ForceMode.Force);

            
        }

        private void OnStartGrapple()
        {
            bool hasTarget = GetBestTarget(out _grappleTarget, out Vector3 targetPositionInWorld);
            if (!hasTarget)
                return;
            _localTargetPosition = _grappleTarget.InverseTransformPoint(targetPositionInWorld);

            if (!_grappleVisual)
                _grappleVisual = Instantiate(_grappleVisualPrefab);
            
            if (_moveGrappleCoroutine != null)
                StopCoroutine(_moveGrappleCoroutine);
            _moveGrappleCoroutine = StartCoroutine(MoveGrappleToTarget());
        }

        private IEnumerator MoveGrappleToTarget()
        {
            WaitForFixedUpdate wait = new WaitForFixedUpdate();
            _grappleVisual.transform.position = _grappleOrigin.position;
            _grappleVisual.transform.rotation = _grappleOrigin.rotation;
            while (Vector3.Distance(_grappleVisual.transform.position, _grappleTarget.TransformPoint(_localTargetPosition)) > 0)
            {
                float deltaTime = Time.fixedDeltaTime;
                _grappleVisual.transform.position = Vector3.MoveTowards(_grappleVisual.transform.position,
                    _grappleTarget.TransformPoint(_localTargetPosition), _grappleThrowSpeed * deltaTime);
                yield return wait;
            }
            _distanceToTarget = Vector3.Distance(_grappleTarget.TransformPoint(_localTargetPosition), _grappleOrigin.position);
            _playerMovementStateManager.CurrentState = PlayerMovementStates.Grappling;
        }

        private bool GetBestTarget(out IGrappleTarget target, out Vector3 targetPosition)
        {
            Ray ray = new Ray(_grappleOrigin.position, _grappleOrigin.forward);
            
            float bestDistance = float.MaxValue;
            target = null;
            targetPosition = Vector3.zero;
            for (var i = IGrappleTarget.Targets.Count - 1; i >= 0; i--)
            {
                var t = IGrappleTarget.Targets[i];
                if (t == null)
                {
                    IGrappleTarget.Targets.RemoveAt(i);
                    continue;
                }

                bool hasGrapplePoint = t.GetGrapplePoint(ray, out Vector3 closestPoint, out float rayDistance);
                Debug.DrawRay(ray.origin, ray.direction * _maxDistanceFromOrigin, Color.green, 2f);
                Debug.DrawLine(closestPoint, ray.GetPoint(rayDistance), Color.cyan, 2f);
                if (!hasGrapplePoint)
                    continue;
                float realDistanceToGrapplePoint = Vector3.Distance(closestPoint, ray.origin);
                if (realDistanceToGrapplePoint > _maxDistanceFromOrigin || realDistanceToGrapplePoint < _minDistanceFromOrigin)
                    continue;

                if (Vector3.Distance(ray.GetPoint(rayDistance), closestPoint) > _maxDistanceFromGrapplePoint)
                    continue;
                
                if (Vector3.Distance(closestPoint, ray.GetPoint(rayDistance)) > bestDistance)
                    continue;

                bestDistance = realDistanceToGrapplePoint;
                target = t;
                targetPosition = closestPoint;
            }
            
            

            return target != null;
        }

        private void OnReleaseGrapple()
        {
            if (_moveGrappleCoroutine != null)
                StopCoroutine(_moveGrappleCoroutine);
            Destroy(_grappleVisual);
            _grappleTarget = null;
            _playerMovementStateManager.CurrentState = PlayerMovementStates.Airborne;
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

            _playerInputManager.GrappleButton.Pressed -= OnStartGrapple;
            _playerInputManager.GrappleButton.Released -= OnReleaseGrapple;
            _playerInputManager = null;

            _isSubscribed = false;
        }

        public void Finish()
        {
            
        }
    }
}