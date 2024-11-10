using System;
using System.Collections;
using UnityEngine;

namespace SpyRunners.Managers
{
    public class CameraManager : MonoBehaviour
    {
        private static CameraManager _instance;
        
        private Camera _camera;
        public static Camera Camera => _instance ? _instance._camera : null;

        private PriorityList<CameraData> _cameras;
        public static PriorityList<CameraData> Cameras => _instance ? _instance._cameras : null;
        
        private Coroutine _moveCoroutine;

        private bool _isMoving;

        [SerializeField] private CameraData _defaultData;
        [SerializeField] private float _transferTime;

        private void Awake()
        {
            _instance = this;
            
            _camera = GetComponent<Camera>();

            _cameras = new(_defaultData);
            _cameras.NewValue += OnNewValue;
            SetCameraValues(_cameras.Value);
        }

        private void OnDestroy()
        {
            _cameras.NewValue -= OnNewValue;
            _cameras = null;

            _instance = null;
        }

        private void OnNewValue(CameraData newData)
        {
            SetTargetInternal(newData);
        }

        private void LateUpdate()
        {
            if (_isMoving)
                return;
            
            SetCameraValues(_cameras.Value);
        }

        private void SetCameraValues(CameraData data)
        {
            _camera.transform.position = data.Target.position;
            _camera.transform.rotation = data.Target.rotation;
            _camera.fieldOfView = data.FieldOfView;
            _camera.nearClipPlane = data.NearClippingPlane;
            _camera.farClipPlane = data.FarClippingPlane;
        }

        private void SetTargetInternal(CameraData newData)
        {
            if (_moveCoroutine != null)
                StopCoroutine(_moveCoroutine);

            _moveCoroutine = StartCoroutine(Move(newData));
        }

        private IEnumerator Move(CameraData newData)
        {
            _isMoving = true;

            Transform targetTransform = newData.Target;
            Transform cameraTransform = _camera.transform;
            
            if (!targetTransform || !cameraTransform)
                yield break;
            
            Vector3 originalPosition = cameraTransform.position;
            Quaternion originalRotation = cameraTransform.rotation;
            float originalFov = _camera.fieldOfView;
            float targetFov = newData.CustomFov ? newData.FieldOfView : _defaultData.FieldOfView;
            float originalNearClippingPlane = newData.NearClippingPlane;
            float originalFarClippingPlane = newData.FarClippingPlane;
            
            WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
            
            float ratio = 0f;
            while (ratio > 0f)
            {
                float deltaTime = Time.deltaTime;
                ratio += deltaTime / _transferTime;
                
                if (!cameraTransform)
                    yield break;
                Vector3 newPosition = Vector3.Lerp(originalPosition, targetTransform.position, ratio);
                cameraTransform.position = newPosition;
                cameraTransform.rotation = Quaternion.Lerp(originalRotation, targetTransform.rotation, ratio);
                
                float newFov = Mathf.Lerp(originalFov, targetFov, ratio);
                _camera.fieldOfView = newFov;

                _camera.nearClipPlane = Mathf.Lerp(originalNearClippingPlane, newData.NearClippingPlane, ratio);
                _camera.farClipPlane = Mathf.Lerp(originalFarClippingPlane, newData.FarClippingPlane, ratio);

                yield return waitForEndOfFrame;
            }

            if (!cameraTransform)
                yield break;
            cameraTransform.position = targetTransform.position;
            cameraTransform.rotation = targetTransform.rotation;
            _camera.fieldOfView = targetFov;
            _camera.nearClipPlane = newData.NearClippingPlane;
            _camera.farClipPlane = newData.FarClippingPlane;

            _isMoving = false;
            _moveCoroutine = null;
        }
    }
}