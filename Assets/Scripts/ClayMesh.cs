
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
        normalsArray = VectorArrayToNativeArray(originalMesh.normals, Allocator.Persistent);
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
            originalMesh.RecalculateNormals();
            normalsArray.Dispose();
            normalsArray = VectorArrayToNativeArray(originalMesh.normals, Allocator.Persistent);
            meshCollider.sharedMesh = originalMesh;
            movingVerts = false;
        }
    }

    private void DisplaceVertices(Vector3 pos, Vector3 directionToMove) {

        if (jobActive) {
            FinishJob();
        }
        pos = meshFilter.transform.InverseTransformPoint(pos);
        vertArray = VectorArrayToNativeArray(modifiedVertices, Allocator.TempJob);
        moveVerts = new MoveVertsJob {
            modifiedVerts = vertArray,
            normals = normalsArray,
            direction = directionToMove,
            targetVertexPos = pos,
            radius = radiusOfEffect,
            power = movePower
        };
        vertsJob = moveVerts.Schedule(vertArray.Length, vertArray.Length / 5);
        jobActive = true;
    }

    private void OnCollisionEnter(Collision collision) {
        Debug.Log(collision.relativeVelocity);
        if (collision.relativeVelocity == Vector3.zero) {
            return;
        }

        ContactPoint[] contacts = new ContactPoint[collision.contactCount];
        collision.GetContacts(contacts);
        movingVerts = true;
        for(int i = 0; i < contacts.Length; i++) {
            DisplaceVertices(contacts[i].point, collision.relativeVelocity);
        }
    }

    private void FinishJob() {
        vertsJob.Complete();
        modifiedVertices = NativeArrayToVectorArray(moveVerts.modifiedVerts);
        vertArray.Dispose();
        jobActive = false;
    }

    private unsafe NativeArray<Vector3> VectorArrayToNativeArray(Vector3[] vectorArray, Allocator allocator) {
        NativeArray<Vector3> nativeVectors = new NativeArray<Vector3>(vectorArray.Length, allocator);

        fixed (void* vectorPointer = vectorArray) { // Fix the vector array in place so Unity doesn't move it and get a pointer to it
            UnsafeUtility.MemCpy(NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(nativeVectors), vectorPointer,
                vectorArray.Length * (long)UnsafeUtility.SizeOf<Vector3>());
        }

        return nativeVectors;
    }

    private unsafe Vector3[] NativeArrayToVectorArray(NativeArray<Vector3> nativeVectors) {
        Vector3[] vectorArray = new Vector3[nativeVectors.Length];

        fixed (void* vectorPointer = vectorArray) { // Fix the vector array in place so Unity doesn't move it and get a pointer to it
            UnsafeUtility.MemCpy(vectorPointer, NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(nativeVectors),
                vectorArray.Length * (long)UnsafeUtility.SizeOf<Vector3>());
        }

        return vectorArray;
    }

    private void OnApplicationQuit() {
        normalsArray.Dispose();
    }
}

