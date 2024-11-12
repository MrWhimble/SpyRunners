using UnityEditor;
using UnityEngine;

namespace SpyRunners
{
    public static class DebugUtils
    {
        #if UNITY_EDITOR
        // Taken from toomasoi @ https://forum.unity.com/threads/drawing-capsule-gizmo.354634/#post-4100557
        public static void DrawWireCapsule(Vector3 pos, Quaternion rot, float radius, float height, Color color = default(Color))
        {
            if (color != default(Color))
                Handles.color = color;
            Matrix4x4 angleMatrix = Matrix4x4.TRS(pos, rot, Handles.matrix.lossyScale);
            using (new Handles.DrawingScope(angleMatrix))
            {
                //var pointOffset = (height - (radius * 2)) / 2;
                float halfHeight = height / 2f;
 
                //draw sideways
                Handles.DrawWireArc(Vector3.up * halfHeight, Vector3.left, Vector3.back, -180, radius);
                Handles.DrawLine(new Vector3(0, halfHeight, -radius), new Vector3(0, -halfHeight, -radius));
                Handles.DrawLine(new Vector3(0, halfHeight, radius), new Vector3(0, -halfHeight, radius));
                Handles.DrawWireArc(Vector3.down * halfHeight, Vector3.left, Vector3.back, 180, radius);
                //draw frontways
                Handles.DrawWireArc(Vector3.up * halfHeight, Vector3.back, Vector3.left, 180, radius);
                Handles.DrawLine(new Vector3(-radius, halfHeight, 0), new Vector3(-radius, -halfHeight, 0));
                Handles.DrawLine(new Vector3(radius, halfHeight, 0), new Vector3(radius, -halfHeight, 0));
                Handles.DrawWireArc(Vector3.down * halfHeight, Vector3.back, Vector3.left, -180, radius);
                //draw center
                Handles.DrawWireDisc(Vector3.up * halfHeight, Vector3.up, radius);
                Handles.DrawWireDisc(Vector3.down * halfHeight, Vector3.up, radius);
 
            }
        }
        #endif // UNITY_EDITOR
    }
}