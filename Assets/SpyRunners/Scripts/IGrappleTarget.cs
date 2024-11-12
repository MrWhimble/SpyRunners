using System.Collections.Generic;
using UnityEngine;

namespace SpyRunners
{
    public interface IGrappleTarget
    {
        /// <summary>
        /// Local to World Space
        /// </summary>
        /// <param name="point">Point in Local Space</param>
        /// <returns>Point in World Space</returns>
        Vector3 TransformPoint(Vector3 point);
        /// <summary>
        /// World to Local Space
        /// </summary>
        /// <param name="point">Point in World Space</param>
        /// <returns>Point in Local Space</returns>
        Vector3 InverseTransformPoint(Vector3 point);
        bool GetGrapplePoint(Ray ray, out Vector3 closestPoint, out float rayDistance);
        Vector3 GetPointVelocity(Vector3 worldPosition);

        public static List<IGrappleTarget> Targets { get; } = new();
    }
}