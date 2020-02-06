using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Mesh))]
public class Hand : MonoBehaviour
{
    private Mesh lastMesh;
    public Mesh LastMesh {
        get { return lastMesh; }
    }

    private Mesh currentMesh;
    public Mesh CurrentMesh {
        get { return currentMesh; }
    }

    private Mesh actualMesh;

    private void Start() {
        actualMesh = GetComponent<Mesh>();
        lastMesh = actualMesh;
        currentMesh = actualMesh;
    }
    // Update is called once per frame
    void Update()
    {
        lastMesh = currentMesh;
        currentMesh = actualMesh;
    }
}
