using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Circle
{
    private const float GIZMO_DISK_THICKNESS = 0.01f;
    public static void DrawGizmoDisk(this Transform t, float radius)
    {
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(t.position, t.rotation, new Vector3(1, GIZMO_DISK_THICKNESS, 1));
        Gizmos.DrawWireSphere(Vector3.zero, radius);
        Gizmos.matrix = oldMatrix;
    }
}
