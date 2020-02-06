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

    private void Start() {
        rotation = new Vector3(0, yRotation, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (rotate) {
            transform.Rotate(rotation * Time.deltaTime);
        }
    }
}
