using UnityEngine;

namespace SpyRunners.Managers
{
    [System.Serializable]
    public class CameraData
    {
        public Transform Target = null;
        public bool CustomFov = false;
        public float FieldOfView = 60;
        public float NearClippingPlane = 0.3f;
        public float FarClippingPlane = 1000f;

        public CameraData(Transform target)
        {
            Target = target;
            CustomFov = false;
            FieldOfView = 60;
            NearClippingPlane = 0.1f;
            FarClippingPlane = 1000f;
        }
        
        public CameraData(Transform target, float fieldOfView)
        {
            Target = target;
            CustomFov = true;
            FieldOfView = fieldOfView;
            NearClippingPlane = 0.1f;
            FarClippingPlane = 1000f;
        }

        public void SetFieldOfView(float fieldOfView)
        {
            CustomFov = true;
            FieldOfView = fieldOfView;
        }

        public void ClearFieldOfView()
        {
            CustomFov = false;
        }
    }
}