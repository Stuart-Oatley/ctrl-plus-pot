using UnityEngine;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;

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
    
    public void Execute(int index) {
        float sqrMag = (modifiedVerts[index] - targetVertexPos).sqrMagnitude;
        if(sqrMag > radius * radius) {
            return;
        }
        float distance = Mathf.Sqrt(sqrMag);
        float falloff = GaussFalloff(distance, radius);
        Vector3 targetPos = modifiedVerts[index] + direction * falloff * power;
        //if (!Pushing(index, direction * falloff * power)) {
        //    return;
        //}
        //if (Vector3.Distance(modifiedVerts[index], centrePoint) < Vector3.Distance(targetPos, centrePoint)) {
        //    return;
        //}
        modifiedVerts[index] = modifiedVerts[index] + direction * falloff * power;
    }

    private bool Pushing(int i, Vector3 direction) {
        Vector3 normalisedDirection = direction / direction.magnitude;
        if (Vector3.Dot(normalisedDirection, normals[i]) <= 0.1) {
            return true;
        }
        return false;
    }

    private static float GaussFalloff(float dist, float inRadius) {
        return Mathf.Clamp01(Mathf.Pow(360, -Mathf.Pow(dist / inRadius, 2.5f) - 0.01f));
    }
}
