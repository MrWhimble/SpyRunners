using UnityEngine;

namespace SpyRunners
{
    public class LineGrappleTarget : MonoBehaviour, IGrappleTarget
    {
        [SerializeField] private float _length = 10;

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

        public Vector3 TransformPoint(Vector3 point) => transform.TransformPoint(point);
        public Vector3 InverseTransformPoint(Vector3 point) => transform.InverseTransformPoint(point);

        public bool GetGrapplePoint(Ray ray, out Vector3 closestPoint, out float rayDistance)
        {
            bool isParallel = ray.FindClosestPointOnLineSegment(
                transform.position - transform.right * _length * 0.5f,
                transform.position + transform.right * _length * 0.5f,
                out closestPoint, out rayDistance);

            return isParallel;
        }
        
        public Vector3 GetPointVelocity(Vector3 worldPosition)
        {
            return _hasRigidbody ? _rigidbody.GetPointVelocity(worldPosition) : Vector3.zero;
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Debug.DrawLine(transform.position - transform.right * _length * 0.5f, transform.position + transform.right * _length * 0.5f, Color.white);
        }
#endif // UNITY_EDITOR
    }
}