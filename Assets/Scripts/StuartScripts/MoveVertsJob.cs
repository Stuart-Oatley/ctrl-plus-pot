using UnityEngine;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;

/// <summary>
/// Unity Job to multithread moving verts
/// </summary>
[BurstCompile]
struct MoveVertsJob : IJobParallelFor {
    public NativeArray<Vector3> modifiedVerts;
    public NativeArray<Vector3> normals;
    public Vector3 direction;
    public Vector3 targetVertexPos;
    public Vector3 centrePoint;
    public Bounds meshBounds;
    public float radius;
    public float power;
    
    /// <summary>
    /// Execute is run on each vert in index
    /// </summary>
    /// <param name="index"></param>
    public void Execute(int index) {
        float sqrMag = (modifiedVerts[index] - targetVertexPos).sqrMagnitude;
        if(sqrMag > radius * radius) {
            return; // Not close enough to need moving
        }
        float distance = Mathf.Sqrt(sqrMag);
        float falloff = GaussFalloff(distance, radius);
        Vector3 targetPos = modifiedVerts[index] + direction * falloff * power;
        modifiedVerts[index] = modifiedVerts[index] + direction * falloff * power;
    }

    /// <summary>
    /// checks whether the vert is being pushed - don't want to move it if it's being pulled
    /// </summary>
    /// <param name="i"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    private bool Pushing(int i, Vector3 direction) {
        Vector3 normalisedDirection = direction / direction.magnitude;
        if (Vector3.Dot(normalisedDirection, normals[i]) <= 0.1) {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Gives a falling off amount to be moved based on distance from the collision point.
    /// </summary>
    /// <param name="dist"></param>
    /// <param name="inRadius"></param>
    /// <returns></returns>
    private static float GaussFalloff(float dist, float inRadius) {
        return Mathf.Clamp01(Mathf.Pow(360, -Mathf.Pow(dist / inRadius, 2.5f) - 0.01f));
    }
}
