
using UnityEngine;
using Unity.Jobs;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Collections;

[RequireComponent(typeof(MeshFilter))]
public class ClayMesh : MonoBehaviour
{
    private Mesh originalMesh;
    private Mesh changedMesh;

    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    private int targetIndex;
    private Vector3 targetVertex;

    private Vector3[] originalVertices;
    private Vector3[] modifiedVertices;

    [SerializeField]
    private float radiusOfEffect = 3f;

    [SerializeField]
    private float movePower = 50f;

    private bool movingVerts = false;
    private bool jobActive = false;

    private JobHandle vertsJob;
    private MoveVertsJob moveVerts;
    NativeArray<Vector3> vertArray;
    NativeArray<Vector3> normalsArray;

    private void Start() {
        InitMesh();
    }

    private void InitMesh() {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        originalMesh = meshFilter.mesh;
        changedMesh = originalMesh;
        originalVertices = originalMesh.vertices;
        modifiedVertices = new Vector3[originalVertices.Length];
        for(int i = 0; i < originalVertices.Length; i++) {
            modifiedVertices[i] = originalVertices[i];
        }

    }

    private void Update() {
        if (movingVerts) {
            if (jobActive) {
                FinishJob();
            }
            originalMesh.vertices = modifiedVertices;
            originalMesh.RecalculateBounds();
            originalMesh.RecalculateNormals();
            meshCollider.sharedMesh = originalMesh;
            movingVerts = false;
        }
    }

    private void DisplaceVertices(Vector3 pos, Vector3 directionToMove) {

        if (jobActive) {
            FinishJob();
        }
        Vector3 meshPos = meshFilter.transform.InverseTransformPoint(pos);
        vertArray = VectorArrayToNativeArray(modifiedVertices, Allocator.TempJob);
        normalsArray = VectorArrayToNativeArray(originalMesh.normals, Allocator.TempJob);
        moveVerts = new MoveVertsJob {
            modifiedVerts = vertArray,
            normals = normalsArray,
            direction = directionToMove,
            targetVertexPos = meshPos,
            radius = radiusOfEffect,
            power = movePower
        };
        vertsJob = moveVerts.Schedule(vertArray.Length, vertArray.Length / 5);
        jobActive = true;
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.relativeVelocity == Vector3.zero) {
            return;
        }

        movingVerts = true;
        for(int i = 0; i < collision.contacts.Length; i++) {
            DisplaceVertices(collision.contacts[i].point, collision.relativeVelocity);
        }
    }

    private void FinishJob() {
        vertsJob.Complete();
        NativeArrayToVectorArray(moveVerts.modifiedVerts, modifiedVertices);
        vertArray.Dispose();
        normalsArray.Dispose();
        jobActive = false;
    }

    //native arrays constructor that takes an array and it's toArray functions are both costly and create garbage so the following functions
    //use pointers and memcopy to remove both issues

    /// <summary>
    /// Creates a new NativeArray and copies the memory from the Vector3 array to it. As NativeArray's dispose method handles deallocating memory
    /// without creating garbage we don't have to worry about allocating the nativearray creating garbage 
    /// </summary>
    /// <param name="vectorArray"></param>
    /// <param name="allocator"></param>
    /// <returns></returns>
    private unsafe NativeArray<Vector3> VectorArrayToNativeArray(Vector3[] vectorArray, Allocator allocator) {
        NativeArray<Vector3> nativeVectors = new NativeArray<Vector3>(vectorArray.Length, allocator);

        fixed (void* vectorPointer = vectorArray) { // Fix the vector array in place whilst we copy so Unity doesn't move it and get a pointer to it
            UnsafeUtility.MemCpy(NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(nativeVectors), vectorPointer,
                vectorArray.Length * (long)UnsafeUtility.SizeOf<Vector3>());
        }

        return nativeVectors;
    }

    /// <summary>
    /// copies the memory from the NativeArray to the Vector3 Array. They must be the same size for the copy to happen. Passing in the premade array avoids 
    /// allocating the vector3 array which would create a lot of garbage
    /// </summary>
    /// <param name="nativeVectors">Native array to copy</param>
    /// <param name="vectorArray">Array to populate.</param>
    /// <returns></returns>
    private unsafe void NativeArrayToVectorArray(NativeArray<Vector3> nativeVectors, Vector3[] vectorArray) {
        if(nativeVectors.Length != vectorArray.Length) {
            return;
        }

        fixed (void* vectorPointer = vectorArray) { // Fix the vector array in place whilst we copy so Unity doesn't move it and get a pointer to it
            UnsafeUtility.MemCpy(vectorPointer, NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(nativeVectors),
                vectorArray.Length * (long)UnsafeUtility.SizeOf<Vector3>());
        }

    }

    /// <summary>
    /// Copies the memory from one Vector3 array to another to avoid having to allocate and create garbage.
    /// Arrays must be the same size for copy to take place
    /// </summary>
    /// <param name="copyFrom">Array to copy from</param>
    /// <param name="copyTo">Array to copy to</param>
    private unsafe void CopyVectorArray(Vector3[] copyFrom, Vector3[] copyTo) {
        if(copyFrom.Length != copyTo.Length) {
            return;
        }

        fixed(void* sourcePointer = copyFrom) { //Fix copyFrom in place
            fixed(void* targetPointer = copyTo) { //Fix copyTo in place
                UnsafeUtility.MemCpy(targetPointer, sourcePointer, copyFrom.Length * (long)UnsafeUtility.SizeOf<Vector3>());
            }
        }
    }

}

