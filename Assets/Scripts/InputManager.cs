using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static Vector3? lastPos;
    public static Vector3? LastPos {
        get { return lastPos; }
    }

    private static Vector3? newPos;
    public static Vector3? NewPos {
        get { return newPos; }
    }

    private int frameCount;
    private int framesBetweenDrags = 10;
    private float zValue;
    private bool dragging;
    private ClayMesh clay;

    private void Start() {
        clay = FindObjectOfType<ClayMesh>();
    }

    private void Update() {
        if(Input.GetMouseButtonDown(0)) {
            SetZPos();
            newPos = GetMouseWorldPosition();
        } else if (Input.GetMouseButton(0) && dragging) {
                lastPos = newPos;
                newPos = GetMouseWorldPosition();
        } else if(dragging) {
            newPos = null;
            lastPos = null;
            dragging = false;
        }
        if(lastPos == null && newPos == null) {
            return;
        }
    }

    private void SetZPos() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit)) {
            zValue = hit.point.z;
            dragging = true;
        }
    }

    private Vector3? GetMouseWorldPosition() {
        if (!dragging) {
            return null;
        }
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, zValue));
        return worldPos;
    }
}

