using UnityEngine;

namespace SpyRunners
{
    public static class RayExtensions
    {
        // based on https://www.crewes.org/Documents/ResearchReports/2010/CRR201032.pdf
        public static bool FindClosestPointOnLineSegment(this Ray ray, Vector3 segmentA, Vector3 segmentB,
            out Vector3 closestPointOnLineSegment, out float distanceAlongRay)
        {
            closestPointOnLineSegment = Vector3.zero;
            distanceAlongRay = 0;
            
            Vector3 lineSegmentDirection = (segmentB - segmentA).normalized;

            if (Mathf.Abs(Vector3.Dot(ray.direction, lineSegmentDirection)) >= 1f)
                return false;
            
            Vector3 rayOriginToSegmentA = (segmentA - ray.origin);
            Vector3 m = Vector3.Cross(lineSegmentDirection, ray.direction);
            float m2 = Vector3.Dot(m, m);
            Vector3 r = Vector3.Cross(rayOriginToSegmentA, m / m2);
            distanceAlongRay = Vector3.Dot(r, lineSegmentDirection);
            float t2 = Vector3.Dot(r, ray.direction);
            t2 = Mathf.Clamp(t2, 0, (segmentB - segmentA).magnitude);
            closestPointOnLineSegment = segmentA + lineSegmentDirection * t2;
            return true;
        }
    }
}