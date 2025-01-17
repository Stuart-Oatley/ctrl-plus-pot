﻿
using UnityEngine;
using Unity.Jobs;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Collections;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Class to handle moving verts on the clay
/// </summary>
[RequireComponent(typeof(MeshFilter))]
public class ClayMesh : MonoBehaviour
{
    private Mesh newMesh;
    private int[] originalTris;
    private Vector3[] originalVerts;
    private Vector3[] originalNorms;
    private Vector2[] originalUVs;
    private MeshRenderer mRenderer;
    [SerializeField]
    private Transform centre;

    private Vector3 middle;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    private int targetIndex;
    private Vector3 targetVertex;

    private Vector3[] modifiedVertices;

    [SerializeField]
    private float radiusOfEffect = 3f;

    [SerializeField]
    private float movePower = 50f;

    private bool movingVerts = false;
    private bool moveJobActive = false;

    [SerializeField]
    private float maxVertMovement = 0.5f;

    private Dictionary<int, List<int>> connectedNormals;

    private JobHandle vertsJob;
    private MoveVertsJob moveVerts;
    NativeArray<Vector3> vertArray;
    NativeArray<Vector3> normalsArray;
    NativeArray<Vector3> lastVertPositions;
    private JobHandle limitMoveJob;
    private LimitMovement limitMove;
    private bool isActive = false;

    /// <summary>
    /// Initialisation
    /// </summary>
    private void Start() {
        InitMesh();
        middle = meshFilter.transform.InverseTransformPoint(centre.position);
        mRenderer = GetComponent<MeshRenderer>();
        DiscardButton.Discard += Reset;
        PaintDiscard.Discard += Reset;
        PotSaveManager.Saved += Reset;
        AnimationStateManager.MovingCam += SetPotteryActive;
    }

    /// <summary>
    /// sets whether the mesh can be changed based on the camera position
    /// </summary>
    /// <param name="e"></param>
    private void SetPotteryActive(CamMoveEventArgs e) {
        if (e.MovingTo == Position.pottery) {
            StartCoroutine(StartPottery(e.AnimationLength));
            return;
        }
        if (isActive) {
            SetActive(false);
            JobHandle.CompleteAll(ref vertsJob, ref limitMoveJob);
        }
        if (e.MovingTo == Position.painting) {
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            gameObject.GetComponent<MeshCollider>().convex = false;
        }
    }

    /// <summary>
    /// sets the active bool
    /// </summary>
    /// <param name="active"></param>
    private void SetActive(bool active) {
        isActive = active;
    }

    /// <summary>
    /// returns the mesh to it's original state
    /// </summary>
    private void Reset() {
        for (int i = 0; i < originalVerts.Length; i++) {
            modifiedVertices[i] = originalVerts[i];
        }
        RefreshMesh();
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        gameObject.GetComponent<MeshCollider>().convex = true;
        gameObject.GetComponent<Painter>().InitTexture();
    }

    /// <summary>
    /// Initialises the variables from the mesh
    /// </summary>
    private void InitMesh() {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        newMesh = meshFilter.mesh;
        originalNorms = new Vector3[meshFilter.mesh.normals.Length];
        for(int i = 0; i < meshFilter.mesh.normals.Length; i++) {
            originalNorms[i] = meshFilter.mesh.normals[i];
        }

        originalTris = new int[meshFilter.mesh.triangles.Length];
        for (int i = 0; i < meshFilter.mesh.triangles.Length; i++) {
            originalTris[i] = meshFilter.mesh.triangles[i];
        }
        originalUVs = new Vector2[meshFilter.mesh.uv.Length];
        for(int i = 0; i < meshFilter.mesh.uv.Length; i++) {
            originalUVs[i] = meshFilter.mesh.uv[i];
        }
        originalVerts = new Vector3[meshFilter.mesh.vertices.Length];
        for (int i = 0; i < meshFilter.mesh.vertices.Length; i++) {
            originalVerts[i] = meshFilter.mesh.vertices[i];
        }
        modifiedVertices = new Vector3[originalVerts.Length];
        for(int i = 0; i < originalVerts.Length; i++) {
            modifiedVertices[i] = originalVerts[i];
        }
    }

    /// <summary>
    /// Finishes the moving verts job if necessary and applies changes to variables
    /// </summary>
    private void Update() {
        if(!isActive) {
            return;
        }
        if (movingVerts) {
            if (moveJobActive) {
                FinishMoveJob();
            }
            vertArray = VectorArrayToNativeArray(modifiedVertices, Allocator.Persistent);
            lastVertPositions = VectorArrayToNativeArray(newMesh.vertices, Allocator.Persistent);
            limitMove = new LimitMovement {
                oldPositions = lastVertPositions,
                newPositions = vertArray,
                maxMovement = maxVertMovement
            };
            limitMoveJob = limitMove.Schedule(modifiedVertices.Length, modifiedVertices.Length / 5);
        }
    }

    /// <summary>
    /// finishes the job that limits vert movement and applies changes to the mesh
    /// </summary>
    private void LateUpdate() {
        if(!isActive) {
            return;
        }
        if (movingVerts) {
            FinishLimitJob();
            RefreshMesh();
        }
    }

    /// <summary>
    /// Applies changes to mesh
    /// </summary>
    private void RefreshMesh() {
        newMesh.vertices = modifiedVertices;
        newMesh.RecalculateNormals();
        newMesh.RecalculateBounds();
        meshCollider.sharedMesh = newMesh;
        movingVerts = false;
    }

    /// <summary>
    /// sets up and starts the job to move the verts
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="directionToMove"></param>
    private void DisplaceVertices(Vector3 pos, Vector3 directionToMove) {

        if (moveJobActive) {
            FinishMoveJob();
        }
        Vector3 meshPos = meshFilter.transform.InverseTransformPoint(pos);
        directionToMove = meshFilter.transform.InverseTransformDirection(directionToMove);
        vertArray = VectorArrayToNativeArray(modifiedVertices, Allocator.Persistent);
        normalsArray = VectorArrayToNativeArray(newMesh.normals, Allocator.Persistent);
        moveVerts = new MoveVertsJob {
            modifiedVerts = vertArray,
            normals = normalsArray,
            direction = directionToMove,
            targetVertexPos = meshPos,
            radius = radiusOfEffect,
            power = movePower,
            centrePoint = middle,

        };
        vertsJob = moveVerts.Schedule(vertArray.Length, vertArray.Length / 5);
        moveJobActive = true;
    }

    /// <summary>
    /// Detects collisions with the mesh, applies the rotation velocity to the collision and calls displaceverts
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision) {
        if(!isActive) {
            return;
        }
		if(collision.gameObject.tag == "Wheel")
		{
			return;
		}
		rotator Rotator = GetComponentInParent<rotator>();
		Vector3 velocity = collision.collider.GetComponent<Rigidbody>().velocity;
        velocity = meshFilter.transform.TransformDirection(velocity);
        if (Rotator && Rotator.Rotating) {
            velocity += Vector3.back * movePower * Rotator.Rotation.magnitude;
        }
        if (velocity == Vector3.zero) {
            return;
        }

        movingVerts = true;
        velocity = Vector3.Normalize(velocity);
        for(int i = 0; i < collision.contacts.Length; i++) {
            if (!IsPushing(collision.contacts[i].point, velocity)) {
                continue;
            }
            DisplaceVertices(collision.contacts[i].point, velocity);
        }
    }

    /// <summary>
    /// checks if the collision if pushing against the clay
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    private bool IsPushing(Vector3 pos, Vector3 dir) {
        Vector3 localPos = meshFilter.transform.TransformPoint(pos);
        Vector3 nearestNorm = newMesh.normals[FindNearestVert(localPos)];
        if (Vector3.Dot(dir, nearestNorm) <= 0.1) {
            return true;
        }
        return false;
    }

    /// <summary>
    /// finds the nearest vert to a position
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private int FindNearestVert(Vector3 position) {
       
        int index = 0;
        float smallestDistance = float.MaxValue;
        for(int i = 0; i < modifiedVertices.Length; i++) {
            float distance = Vector3.Distance(position, modifiedVertices[i]);
            if ( distance < smallestDistance) {
                index = i;
                smallestDistance = distance;
            }
        }
        return index;
    }

    /// <summary>
    /// finishes the vert movement job
    /// </summary>
    private void FinishMoveJob() {
        vertsJob.Complete();
        NativeArrayToVectorArray(moveVerts.modifiedVerts, modifiedVertices);
        vertArray.Dispose();
        normalsArray.Dispose();
        moveJobActive = false;
    }

    /// <summary>
    /// finishes the limit movement job
    /// </summary>
    private void FinishLimitJob() {
        limitMoveJob.Complete();
        NativeArrayToVectorArray(limitMove.newPositions, modifiedVertices);
        if (vertArray.IsCreated) {
            vertArray.Dispose();
        }
        if (lastVertPositions.IsCreated) {
            lastVertPositions.Dispose();
        }

    }

    /// <summary>
    /// sets active after given time
    /// </summary>
    /// <param name="delayTime"></param>
    /// <returns></returns>
    private IEnumerator StartPottery(float delayTime) {
        yield return new WaitForSeconds(delayTime);
        SetActive(true);
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

    /// <summary>
    /// disposes of the normals array 
    /// </summary>
    private void OnApplicationQuit() {
        if (normalsArray.IsCreated) {
            normalsArray.Dispose();
        }
    }
}

