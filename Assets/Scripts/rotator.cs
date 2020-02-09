using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotator : MonoBehaviour
{
    [SerializeField]
    private float yRotation;

    [SerializeField]
    bool rotate = false;

    private Vector3 rotation;
    private Rigidbody rigidbody;
    private void Start() {
        rotation = new Vector3(0, yRotation, 0);
        rigidbody = GetComponent<Rigidbody>();
    }

    //private void FixedUpdate() {
    //    if (rotate) {
    //        rigidbody.AddTorque(rotation, ForceMode.Force);
    //    }
    //}

    void Update() {
        if (rotate) {
            transform.Rotate(rotation * Time.deltaTime);
        }
    }
}
