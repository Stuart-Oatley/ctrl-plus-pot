using UnityEngine;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;

[BurstCompile]
struct LimitMovement : IJobParallelFor
{
    public NativeArray<Vector3> oldPositions;
    public NativeArray<Vector3> newPositions;
    public float maxMovement;

    public void Execute(int index) {
        Vector3 movement = newPositions[index] - oldPositions[index];
        if(movement.sqrMagnitude <= maxMovement * maxMovement) {
            return;
        }
        newPositions[index] = oldPositions[index] + Vector3.ClampMagnitude(movement, maxMovement);
    }
}
