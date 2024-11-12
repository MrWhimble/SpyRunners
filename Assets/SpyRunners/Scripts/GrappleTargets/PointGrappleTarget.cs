using UnityEditor;
using UnityEngine;

namespace SpyRunners
{
    public class PointGrappleTarget : MonoBehaviour, IGrappleTarget
    {
        private Rigidbody _rigidbody;
        private bool _hasRigidbody;
        
        private void OnEnable()
        {
            IGrappleTarget.Targets.Add(this);
        }

        private void OnDisable()
        {
            IGrappleTarget.Targets.Remove(this);
        }
        
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _hasRigidbody = _rigidbody;
        }
        
        public Vector3 TransformPoint(Vector3 point) => transform.TransformPoint(point);
        public Vector3 InverseTransformPoint(Vector3 point) => transform.InverseTransformPoint(point);
        
        public bool GetGrapplePoint(Ray ray, out Vector3 closestPoint, out float rayDistance)
        {
            closestPoint = transform.position;
            rayDistance = Vector3.Project(closestPoint - ray.origin, ray.direction).magnitude;
            rayDistance *= Mathf.Sign(Vector3.Dot(closestPoint - ray.origin, ray.direction));
            
            return true;
        }

        public Vector3 GetPointVelocity(Vector3 worldPosition)
        {
            return _hasRigidbody ? _rigidbody.velocity : Vector3.zero;
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Handles.color = Color.white;
            Handles.DrawWireCube(transform.position, new Vector3(0.1f, 0.1f, 0.1f));
            Handles.DrawWireCube(transform.position, new Vector3(0.2f, 0.2f, 0.2f));
            Handles.DrawWireCube(transform.position, new Vector3(0.25f, 0.25f, 0.25f));
        }
#endif // UNITY_EDITOR
    }
}