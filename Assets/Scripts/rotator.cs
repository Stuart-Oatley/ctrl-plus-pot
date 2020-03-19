using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{

    [SerializeField]
    public float yRotation;

    [SerializeField]
    private bool rotating = false;
    public bool Rotating {
        get { return rotating; }
    }

    private Vector3 rotation;
    public Vector3 Rotation {
        get { return rotation * Time.deltaTime; }
    }
    private Rigidbody rigidbody;
    private void Start()
    {
        rotation = new Vector3(0, yRotation, 0);
        rigidbody = GetComponent<Rigidbody>();
    }

    //private void FixedUpdate() {
    //    if (rotate) {
    //        rigidbody.AddTorque(rotation, ForceMode.Force);
    //    }
    //}

    void Update()
    {
        if (rotating)
        {
            transform.Rotate(rotation * Time.deltaTime);
        }
    }
}
