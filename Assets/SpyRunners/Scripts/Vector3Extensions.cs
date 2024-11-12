using UnityEngine;

namespace SpyRunners
{
    public static class Vector3Extensions
    {
        public static Vector3 ClosestPointToLineSegment(this Vector3 p, Vector3 a, Vector3 b)
        {
            Vector3 ap = p - a;
            Vector3 ab = b - a;

            float abMag = ab.magnitude;

            float abapProduct = ab.x * ap.x + ab.y * ap.y + ab.z * ap.z;

            float distance = abapProduct / abMag;

            if (distance < 0)
            {
                return a;
            } else if (distance > 1)
            {
                return b;
            } else
            {
                return new Vector3(a.x + ab.x * distance, a.y + ab.y * distance, a.z + ab.z * distance);
            }
        }
        
        
    }
}