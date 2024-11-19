﻿using System;
using UnityEngine; 
namespace SpyRunners.Player
{
    public class PlayerMovement : MonoBehaviour, IDependent
    {
        [Header("Force Settings")]
        [SerializeField] private float _baseSpeed = 4;
        [SerializeField] private float _maxSpeed = 16;
        [SerializeField] private float _speedDecreaseRate = -0.5f;

        [Header("Acceleration Settings")]
        [SerializeField] private float _acceleration = 50;
        [SerializeField] private AnimationCurve _accelerationFactorFromDot = new AnimationCurve(new Keyframe(-1, 2), new Keyframe(0, 1), new Keyframe(1, 1));
        [SerializeField] private float _accelerationFactorOnGround = 1;
        [SerializeField] private float _accelerationFactorOnWall = 1;
        [SerializeField] private float _accelerationFactorInAir = 0.25f;
        
        [Header("Max Acceleration Settings")]
        [SerializeField] private float _maxAccelerationForce = 50;
        [SerializeField] private float _maxAccelerationForceFactorOnGround = 1;
        [SerializeField] private float _maxAccelerationForceFactorOnWall = 1;
        [SerializeField] private float _maxAccelerationForceFactorInAir = 0.1f;
        [SerializeField] private AnimationCurve _maxAccelerationForceFactorFromDot = new AnimationCurve(new Keyframe(-1, 2), new Keyframe(0, 1), new Keyframe(1, 1));
        [SerializeField] private AnimationCurve _maxAccelerationForceFactorOnWallFromDot = new AnimationCurve(new Keyframe(-1, 1), new Keyframe(0, 0), new Keyframe(1, 0));
        [SerializeField] private AnimationCurve _maxAccelerationForceFactorInAirFromDot = new AnimationCurve(new Keyframe(-1, 1), new Keyframe(0, 0), new Keyframe(1, 0));

        [Header("Ground Check Settings")]
        [SerializeField] private LayerMask _groundCheckLayers = 1;
        [SerializeField] private float _groundCheckStartHeight = 0.6f;
        [SerializeField] private float _groundCheckDistance = 0.26f;
        [SerializeField] private float _groundCheckRadius = 0.4f;
        
        [Header("Wall Check Settings")]
        [SerializeField] private LayerMask _wallCheckLayers = 1;
        [SerializeField] private float _wallCheckStartHeight = 0.6f;
        [SerializeField] private float _wallCheckDistance = 0.2f;
        [SerializeField] private float _wallCheckRadius = 0.2f;
        
        [Header("Wall Riding Settings")]
        [SerializeField] private float _minSpeedToWallRide = 6f;
        [SerializeField] private float _maxVerticalSpeedToWallRide = 4f;
        [SerializeField] private float _wallNormalPushingForce = 0;
        [SerializeField] private float _wallAntiGravityForceMultiplier = 0.9f;
        [SerializeField] private float _wallJumpingNormalForce = 10;
        
        [Header("Misc Settings")]
        [SerializeField] private float _jumpForce = 4;
        [SerializeField] private Vector3 _forceScale = new Vector3(1, 0, 1);
        
        private PlayerCharacter _playerCharacter;
        private Rigidbody _rigidbody;
        private PlayerMovementStateManager _playerMovementStateManager;
        private PlayerInputManager _playerInputManager;
        
        private RaycastHit[] _hitCache;
        private Collider[] _colliderCache = new Collider[4];
        private Rigidbody _groundedRigidbody;

        private Vector3 _goalVelocity;

        private bool _jumpBuffer;

        private float _speed;
        
        private bool _subscribed = false;
        
        private void Awake()
        {
            _hitCache = new RaycastHit[4];
            _playerCharacter = GetComponent<PlayerCharacter>();
            _playerCharacter.AddDependent(this);
            _rigidbody = GetComponent<Rigidbody>();

            _speed = _baseSpeed;
        }
        
        public void Initialize()
        {
            
        }

        public void SubscribeToEvents()
        {
            if (_subscribed)
                return;
            
            _playerMovementStateManager = _playerCharacter.Dependencies[typeof(PlayerMovementStateManager)] as PlayerMovementStateManager;
            if (!_playerMovementStateManager)
                throw new System.NullReferenceException("PlayerMovementStateManager is null");
            _playerInputManager = _playerCharacter.PlayerManager.Dependencies[typeof(PlayerInputManager)] as PlayerInputManager;
            if (!_playerInputManager)
                throw new System.NullReferenceException("PlayerInputManager is null");
            _playerInputManager.JumpButton.Pressed += OnJumpPressed;
            

            _subscribed = true;
        }

        public void AdjustSpeed(float amount )
        {
            _speed = Mathf.Clamp(_speed + amount, _baseSpeed, _maxSpeed);
        }

        private void FixedUpdate()
        {
            if (_playerMovementStateManager.CurrentState is PlayerMovementStates.Grappling)
                return;
            
            float deltaTime = Time.fixedDeltaTime;

            bool grounded = GetGroundHit(out RaycastHit groundHit);

            if (!grounded)
            {
                bool wallRiding = GetWallHit(out RaycastHit wallHit);
                if (wallRiding)
                {
                    if (HandleWallRide(wallHit, deltaTime))
                        return;
                }
            }
            
            _groundedRigidbody = groundHit.rigidbody;
            Vector3 groundVelocity = grounded && _groundedRigidbody
                ? _groundedRigidbody.GetPointVelocity(groundHit.point)
                : Vector3.zero;

            float upwardsForce = 0;
            if (_jumpBuffer && grounded)
            {
                upwardsForce = (_jumpForce / deltaTime) * _rigidbody.mass;
                grounded = false;
                _jumpBuffer = false;
                //_playerMovementStateManager.CurrentState = PlayerMovementStates.Airborne;
            }
            
            Vector3 inputGoal = new Vector3(_playerInputManager.MoveInput.x, 0, _playerInputManager.MoveInput.y);
            inputGoal = transform.TransformDirection(inputGoal);
            inputGoal = Vector3.ClampMagnitude(inputGoal, 1.0f);

            Vector3 velocityDirection = _goalVelocity.normalized;

            float velocityDot = Vector3.Dot(inputGoal, velocityDirection);
            
            float accelerationFactor = grounded ? _accelerationFactorOnGround : _accelerationFactorInAir;
            float acceleration = _acceleration 
                                 * _accelerationFactorFromDot.Evaluate(velocityDot) 
                                 * accelerationFactor;

            Vector3 targetVelocity = inputGoal * _maxSpeed;

            _goalVelocity = Vector3.MoveTowards(
                _goalVelocity, 
                targetVelocity + groundVelocity, 
                acceleration * deltaTime);

            Vector3 requiredAcceleration = (_goalVelocity - _rigidbody.velocity) / deltaTime;
            
            float maxAccelerationForceFactor = grounded 
                ? _maxAccelerationForceFactorOnGround
                : _maxAccelerationForceFactorInAir;
            float maxAcceleration = _maxAccelerationForce 
                                    * _maxAccelerationForceFactorFromDot.Evaluate(velocityDot)
                                    * maxAccelerationForceFactor;

            if (!grounded)
            {
                Vector3 horizontalVelocity = _rigidbody.velocity;
                horizontalVelocity.y = 0;
                if (horizontalVelocity.magnitude > _maxSpeed)
                {
                    maxAcceleration *= _maxAccelerationForceFactorInAirFromDot.Evaluate(Vector3.Dot(inputGoal, horizontalVelocity.normalized));
                }

                if (inputGoal.magnitude <= 0)
                {
                    maxAcceleration = 0;
                }
            }

            requiredAcceleration = Vector3.ClampMagnitude(requiredAcceleration, maxAcceleration);

            Vector3 actualAcceleration = Vector3.Scale(requiredAcceleration * _rigidbody.mass, _forceScale);
            Vector3 finalForce = actualAcceleration + new Vector3(0, upwardsForce, 0);
            _rigidbody.AddForce(finalForce);
            
            if (_rigidbody.velocity.magnitude > 0.1f && finalForce.magnitude > 0.1f && grounded)
                _playerMovementStateManager.CurrentState = PlayerMovementStates.Running;
            else if (!grounded)
                _playerMovementStateManager.CurrentState = PlayerMovementStates.Airborne;
            else
                _playerMovementStateManager.CurrentState = PlayerMovementStates.Idle;
            
            if (_playerMovementStateManager.CurrentState is PlayerMovementStates.Running)
                _speed = Mathf.Clamp(_speed + _speedDecreaseRate * deltaTime, _baseSpeed, _maxSpeed);
            else if (_playerMovementStateManager.CurrentState is PlayerMovementStates.Idle)
                _speed = _baseSpeed;
        }

        private bool HandleWallRide(RaycastHit wallHit, float deltaTime)
        {
            if (Mathf.Abs(_rigidbody.velocity.y) > _maxVerticalSpeedToWallRide)
                return false;
            Vector3 horizontalVelocity = _rigidbody.velocity;
            horizontalVelocity.y = 0;
            if (horizontalVelocity.magnitude < _minSpeedToWallRide)
                return false;
            
            float mass = _rigidbody.mass;
            
            if (_jumpBuffer)
            {
                float upwardsForce = (_jumpForce / deltaTime) * mass;
                _jumpBuffer = false;
                _playerMovementStateManager.CurrentState = PlayerMovementStates.WallJump; // should be for a single frame
                Vector3 pushOffForce = (_wallJumpingNormalForce / deltaTime) * mass * wallHit.normal;
                _rigidbody.AddForce(new Vector3(0, upwardsForce, 0) + pushOffForce);
                return true;
            }
            
            _playerMovementStateManager.CurrentState = PlayerMovementStates.WallRiding;
            
            transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, wallHit.normal), Vector3.up);
            
            Vector3 velocityOnWall = Vector3.ProjectOnPlane(_rigidbody.velocity, wallHit.normal);
            
            _rigidbody.AddForce(-_wallNormalPushingForce * mass * wallHit.normal);
            _rigidbody.AddForce(-_wallAntiGravityForceMultiplier * mass * Physics.gravity);
            
            Vector3 inputGoal = new Vector3(0, 0, _playerInputManager.MoveInput.y);
            inputGoal = transform.TransformDirection(inputGoal);
            inputGoal = Vector3.ClampMagnitude(inputGoal, 1.0f);
            
            float velocityDot = Vector3.Dot(inputGoal, velocityOnWall);
            
            float acceleration = _acceleration 
                                 * _accelerationFactorFromDot.Evaluate(velocityDot) 
                                 * _accelerationFactorOnWall;
            
            Vector3 targetVelocity = inputGoal * _maxSpeed;

            _goalVelocity = Vector3.MoveTowards(
                _goalVelocity, 
                targetVelocity,// + groundVelocity, 
                acceleration * deltaTime);
            
            Vector3 requiredAcceleration = (_goalVelocity - _rigidbody.velocity) / deltaTime;
            
            float maxAcceleration = _maxAccelerationForce 
                                    * _maxAccelerationForceFactorFromDot.Evaluate(velocityDot)
                                    * _maxAccelerationForceFactorOnWall;
            
            requiredAcceleration = Vector3.ClampMagnitude(requiredAcceleration, maxAcceleration);

            Vector3 actualAcceleration = Vector3.ProjectOnPlane(Vector3.Scale(requiredAcceleration * _rigidbody.mass, _forceScale), wallHit.normal);
            _rigidbody.AddForce(actualAcceleration);
            
            return true;
        }

        private void OnJumpPressed()
        {
            if (_jumpBuffer)
                return;
            _jumpBuffer = true;
        }

        private bool GetGroundHit(out RaycastHit hit)
        {
            Ray ray = new Ray(_rigidbody.position + new Vector3(0, _groundCheckStartHeight, 0), Vector3.down);
            int hits = Physics.SphereCastNonAlloc(ray, _groundCheckRadius, _hitCache, _groundCheckDistance,
                _groundCheckLayers);
            
            return GetClosestHit(hits, _hitCache, out hit);
        }

        private bool GetWallHit(out RaycastHit hit)
        {
            // left side
            Ray ray = new Ray(_rigidbody.position + new Vector3(0, _wallCheckStartHeight, 0), -transform.right);
            int hits = Physics.SphereCastNonAlloc(ray, _wallCheckRadius, _hitCache, _wallCheckDistance,
                _wallCheckLayers);
            bool leftHasWall = GetClosestHit(hits, _hitCache, out RaycastHit leftHit);
            
            // right side
            ray = new Ray(_rigidbody.position + new Vector3(0, _wallCheckStartHeight, 0), transform.right);
            hits = Physics.SphereCastNonAlloc(ray, _wallCheckRadius, _hitCache, _wallCheckDistance,
                _wallCheckLayers);
            bool rightHasWall = GetClosestHit(hits, _hitCache, out RaycastHit rightHit);

            hit = default;
            if (!leftHasWall && !rightHasWall)
                return false;
            if (leftHasWall && !rightHasWall)
                hit = leftHit;
            else if (!leftHasWall) // and right
                hit = rightHit;
            else // left and right
            {
                Debug.Log($"Tight spot > Left: {leftHasWall} | Right: {rightHasWall}");// > Left: {leftHit.collider.gameObject.name} | Right: {rightHit.collider.gameObject.name}");
                hit = leftHit;
            }
            return true;
        }
        
        private bool GetClosestHit(int hitCount, RaycastHit[] hits, out RaycastHit hit)
        {
            if (hitCount <= 0)
            {
                hit = default;
                return false;
            }

            float closestDistance = Mathf.Infinity;
            int closestIndex = -1;
            for (int i = 0; i < hitCount; i++)
            {
                if (hits[i].rigidbody == _rigidbody)
                    continue;
                if (hits[i].distance < closestDistance)
                {
                    closestDistance = hits[i].distance;
                    closestIndex = i;
                }
            }

            if (closestIndex == -1)
            {
                hit = default;
                return false;
            }
            
            hit = hits[closestIndex];
            return true;
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
            _playerMovementStateManager = null;
            
            _subscribed = false;
        }

        public void Finish()
        {
            
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            //DebugUtils.DrawWireCapsule(transform.position + transform.up * (_standingColliderHeight - (_standingColliderHeight - _crouchColliderHeight) / 2f), transform.rotation, _ceilingCheckRadius, _standingColliderHeight - _crouchColliderHeight - 0.2f, Color.green);
            
            DebugUtils.DrawWireCapsule(
                transform.position + transform.up * (_groundCheckStartHeight - _groundCheckDistance * 0.5f),
                transform.rotation, _groundCheckRadius, _groundCheckDistance);
            
            DebugUtils.DrawWireCapsule(
                transform.position + transform.up * (_wallCheckStartHeight) + transform.right * (_wallCheckDistance * 0.5f),
                transform.rotation * Quaternion.Euler(0, 0, 90), _wallCheckRadius, _wallCheckDistance, Color.yellow);
            DebugUtils.DrawWireCapsule(
                transform.position + transform.up * (_wallCheckStartHeight) + transform.right * -(_wallCheckDistance * 0.5f),
                transform.rotation * Quaternion.Euler(0, 0, 90), _wallCheckRadius, _wallCheckDistance, Color.magenta);
            
            //float bottom = _climbCheckRadius + 0.1f;
            //float range = _standingColliderHeight - _climbCheckRadius * 2f - 0.2f;
            //Quaternion rot = Quaternion.LookRotation(-transform.up, transform.forward);
            //for (int i = 0; i < 3; i++)
            //    DebugUtils.DrawWireCapsule(transform.position + transform.up * (bottom + range * (i / 2f)) + transform.forward * _climbCheckDistance / 2f, rot, _climbCheckRadius, _climbCheckDistance, Color.cyan);
        }
#endif // UNITY_EDITOR
    }
}