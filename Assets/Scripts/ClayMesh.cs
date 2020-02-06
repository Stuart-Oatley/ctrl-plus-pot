
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class ClayMesh : MonoBehaviour
{
    private Mesh originalMesh;
    private Mesh changedMesh;

    private MeshFilter meshFilter;

    private int targetIndex;
    private Vector3 targetVertex;

    private Vector3[] originalVertices;
    private Vector3[] modifiedVertices;

    [SerializeField]
    private float radiusOfEffect = 3f;

    [SerializeField]
    private float power = 50f;

    private void Start() {
        InitMesh();
    }

    private void InitMesh() {
        meshFilter = GetComponent<MeshFilter>();
        originalMesh = meshFilter.mesh;
        changedMesh = originalMesh;
        originalVertices = originalMesh.vertices;
        modifiedVertices = new Vector3[originalVertices.Length];
        for(int i = 0; i < originalVertices.Length; i++) {
            modifiedVertices[i] = originalVertices[i];
        }

    }

    private void Update() {
        //if(InputManager.LastPos != null && InputManager.NewPos != null) {
        //    Vector3 lastPos = (Vector3)InputManager.LastPos;
        //    Vector3 newPos = (Vector3)InputManager.NewPos;
        //    if (lastPos == newPos) {
        //        return;
        //    }
            
        //    Vector3 direction = lastPos - newPos;
        //    targetIndex = GetTargetVertexIndex(newPos);
        //    DisplaceVertices(lastPos, direction);
        //    RemakeCollider();
        //}
    }

    private void RemakeCollider() {
        GetComponent<MeshCollider>().sharedMesh = originalMesh;
    }

    private int GetTargetVertexIndex(Vector3 newPos) {
        float smallestDistance = float.MaxValue;
        int closestVertex = 0;
        for (int i = 0; i < modifiedVertices.Length; i++) {
            if (Vector3.Distance(newPos, modifiedVertices[i]) < smallestDistance) {
                closestVertex = i;
            }
        }
        return closestVertex;
    }

    private void DisplaceVertices(Vector3 pos, Vector3 direction) {
        float sqrRadius = radiusOfEffect * radiusOfEffect;
        float force = direction.magnitude;
        Vector3 currentVertexPos = Vector3.zero;
        Vector3 targetVertexPos = meshFilter.transform.InverseTransformPoint(pos);
        for (int i = 0; i < modifiedVertices.Length; i++) {
            currentVertexPos = modifiedVertices[i];
            float sqrMag = (currentVertexPos - targetVertexPos).sqrMagnitude;
            if (sqrMag > sqrRadius) {
                continue;
            }
            if (!Pushing(i, direction)) {
                continue;
            }
            float distance = Mathf.Sqrt(sqrMag);
            float falloff = GaussFalloff(distance, radiusOfEffect);
            modifiedVertices[i] = currentVertexPos + (direction * falloff * power);
        }

        originalMesh.vertices = modifiedVertices;
        originalMesh.RecalculateNormals();

    }

    private bool Pushing(int i, Vector3 direction) {
        Vector3 normalisedDirection = direction / direction.magnitude;
        if(Vector3.Dot(normalisedDirection, originalMesh.normals[i]) <= 0) {
            return true;
        }
        return false;
    }

    private static float GaussFalloff(float dist, float inRadius) {
        return Mathf.Clamp01(Mathf.Pow(360, -Mathf.Pow(dist / inRadius, 2.5f) - 0.01f));
    }

    private void OnCollisionEnter(Collision collision) {

        int vertexCount = modifiedVertices.Length;
        ContactPoint[] contacts = collision.contacts;
        for(int i = 0; i < contacts.Length; i++) {
            DisplaceVertices(contacts[i].point, collision.relativeVelocity);
        }
        originalMesh.vertices = modifiedVertices;
        originalMesh.RecalculateNormals();
        RemakeCollider();


        //List<int> toMove = new List<int>();
        //Dictionary<int, float> closeToMovingVert = new Dictionary<int, float>();
        //Vector3 direction = collision.relativeVelocity;
        //Vector3 normalDirection = direction / direction.magnitude; // Unity's built in normalise functions return a vector3.zero if the vector is below a certain size, so we'll do it ourselves
        //for (int i = 0; i < modifiedVertices.Length; i++) {
        //    if(collision.collider.bounds.Contains(modifiedVertices[i])) {
        //        //if(Vector3.Dot(normalDirection, originalMesh.normals[i]) >= 0) {
        //            toMove.Add(i);
        //            FindClose(modifiedVertices[i], closeToMovingVert);
        //        //}
        //    }
        //}
        //List<int> toRemove = closeToMovingVert.Keys.Except(toMove).ToList();
        //for(int i = 0; i < toRemove.Count; i++) {
        //    closeToMovingVert.Remove(toRemove[i]);
        //}
        //MoveVerts(toMove, direction);
        //MoveCloseVerts(closeToMovingVert, direction);
        //originalMesh.vertices = modifiedVertices;
        //originalMesh.RecalculateNormals();
        //RemakeCollider();

    }

    private void MoveCloseVerts(Dictionary<int, float> closeToMovingVert, Vector3 direction) {
        Vector3 movement = Vector3.zero;
        float distance = .0f;
        float falloff = .0f;
        foreach (KeyValuePair<int, float> pair in closeToMovingVert) {
            distance = Mathf.Sqrt(pair.Value);
            falloff = GaussFalloff(distance, radiusOfEffect);
            modifiedVertices[pair.Key] += direction * falloff * power;
        }
    }

    private void MoveVerts(List<int> toMove, Vector3 direction) {
        foreach(int i in toMove) {
            modifiedVertices[i] += direction;
        }
    }

    private void FindClose(Vector3 position, Dictionary<int, float> closeToMovingVert) {
        float sqRadius = radiusOfEffect * radiusOfEffect;
        for(int i = 0; i < modifiedVertices.Length; i++) {
            float sqMag = (modifiedVertices[i] - position).sqrMagnitude;
            if(sqMag <= sqRadius) {
                if (closeToMovingVert.ContainsKey(i)) {
                    if(closeToMovingVert[i] < sqMag) {
                        closeToMovingVert[i] = sqMag;
                    }
                    continue;
                }
                closeToMovingVert.Add(i, sqMag);
            }
        }
    }
}

